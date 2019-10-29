using System;
using Zephyr.DOTSAStar.Runtime.Component;
using Zephyr.DOTSAStar.Runtime.Lib;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using AStarNode = Zephyr.DOTSAStar.Runtime.Component.AStarNode;

namespace Zephyr.DOTSAStar.Runtime.System
{
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public class PathFindingSystem : JobComponentSystem
    {
        private int _mapSize;
        private EntityCommandBufferSystem _resultECB;
        private EntityCommandBufferSystem _cleanECB;

        private struct OpenSetNode : IComparable<OpenSetNode>, IEquatable<OpenSetNode>
        {
            public int Id;
            public float Priority;

            public OpenSetNode(int id, float priority)
            {
                Id = id;
                Priority = priority;
            }
            
            public int CompareTo(OpenSetNode other)
            {
                return -Priority.CompareTo(other.Priority);
            }

            public bool Equals(OpenSetNode other)
            {
                return Id.Equals(other.Id);
            }
        }
        
        [BurstCompile]
        private struct PrepareNodesJob : IJobForEachWithEntity<AStarNode>
        {
            public NativeArray<AStarNode> AStarNodes;
            public void Execute(Entity entity, int index, [ReadOnly] ref AStarNode aStarNode)
            {
                AStarNodes[index] = aStarNode;
            }
        }

        [BurstCompile]
        [ExcludeComponent(typeof(PathResult))]
        private struct PathFindingJob : IJobForEachWithEntity<PathFindingRequest>
        {
            public int MapSize;
            
            [ReadOnly][DeallocateOnJobCompletion]
            public NativeArray<AStarNode> AStarNodes;

            public EntityCommandBuffer.Concurrent ResultECB;

            public EntityCommandBuffer.Concurrent CleanECB;

            public int IterationLimit;

            public int StepsLimit;
            
            public void Execute(Entity entity, int index, ref PathFindingRequest request)
            {
                //Generate Working Containers
                var openSet = new NativeMinHeap(MapSize, Allocator.Temp);
                var cameFrom = new NativeArray<int>(MapSize, Allocator.Temp);
                var costCount = new NativeArray<int>(MapSize, Allocator.Temp);
                for (var i = 0; i < MapSize; i++)
                {
                    costCount[i] = int.MaxValue;
                }
                
                // Path finding
                var startId = request.StartId;
                var goalId = request.GoalId;
                
                openSet.Push(new MinHeapNode(startId, 0));
                costCount[startId] = 0;

                var currentId = -1;
                while (IterationLimit>0 && openSet.HasNext())
                {
                    var currentNode = openSet[openSet.Pop()];
                    currentId = currentNode.Id;
                    if (currentId == goalId)
                    {
                        break;
                    }
                    
                    var neighboursId = new NativeList<int>(4, Allocator.Temp);
                    AStarNodes[currentId].GetNeighbours(ref neighboursId);

                    foreach (var neighbourId in neighboursId)
                    {
                        //if cost == -1 means obstacle, skip
                        if (AStarNodes[neighbourId].Cost == -1) continue;

                        var currentCost = costCount[currentId] == int.MaxValue
                            ? 0
                            : costCount[currentId];
                        var newCost = currentCost + AStarNodes[neighbourId].Cost;
                        //not better, skip
                        if (costCount[neighbourId] <= newCost) continue;
                        
                        var priority = newCost + AStarNodes[neighbourId].Heuristic(goalId);
                        openSet.Push(new MinHeapNode(neighbourId, priority));
                        cameFrom[neighbourId] = currentId;
                        costCount[neighbourId] = newCost;
                    }

                    IterationLimit--;
                    neighboursId.Dispose();
                }
                
                //Construct path
                var buffer = ResultECB.AddBuffer<PathRoute>(index, entity);
                var nodeId = goalId;
                while (StepsLimit>0 && !nodeId.Equals(startId))
                {
                    buffer.Add(new PathRoute {Id = nodeId});
                    nodeId = cameFrom[nodeId];
                    StepsLimit--;
                }
                
                //Construct Result
                var success = true;
                var log = new NativeString64("Path finding success");
                if (!openSet.HasNext() && currentId != goalId)
                {
                    success = false;
                    log = new NativeString64("Out of openset");
                }
                if (IterationLimit <= 0 && currentId != goalId)
                {
                    success = false;
                    log = new NativeString64("Iteration limit reached");
                }else if (StepsLimit <= 0 && !nodeId.Equals(startId))
                {
                    success = false;
                    log = new NativeString64("Step limit reached");
                }
                ResultECB.AddComponent(index, entity, new PathResult
                {
                    Success = success,
                    Log = log
                });
                
                //Clean result at end of simulation
                CleanECB.DestroyEntity(index, entity);
                
                //Clear
                openSet.Dispose();
                cameFrom.Dispose();
                costCount.Dispose();
            }
            
        }
        
        protected override void OnCreate()
        {
            _mapSize = Const.MapWidth * Const.MapHeight;
            _resultECB = World.Active.GetOrCreateSystem<EndInitializationEntityCommandBufferSystem>();
            _cleanECB = World.Active.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            //Prepare maps
            var aStarNodes = new NativeArray<AStarNode>(_mapSize, Allocator.TempJob);
            
            var prepareNodesJob = new PrepareNodesJob
            {
                AStarNodes = aStarNodes,
            };

            var prepareMapHandle = prepareNodesJob.Schedule(this, inputDeps);

            var sortNodesHandle = aStarNodes.SortJob(prepareMapHandle);
            
            //Path Finding

            var pathFindingJob = new PathFindingJob
            {
                MapSize = _mapSize,
                AStarNodes = aStarNodes,
                ResultECB = _resultECB.CreateCommandBuffer().ToConcurrent(),
                CleanECB = _cleanECB.CreateCommandBuffer().ToConcurrent(),
                IterationLimit = 2000,
                StepsLimit = 1000
            };
            
            var pathFindingHandle = pathFindingJob.Schedule(this, sortNodesHandle);
            _resultECB.AddJobHandleForProducer(pathFindingHandle);
            return pathFindingHandle;
        }
        
        public static readonly NativeArray<int> NeighbourOffset = 
            new NativeArray<int>(4, Allocator.Persistent)
            {
                [0] = -1,
                [1] = Const.MapWidth,
                [2] = 1,
                [3] = -Const.MapWidth,
            };
    }
}