using System;
using Zephyr.DOTSAStar.Runtime.Component;
using Zephyr.DOTSAStar.Runtime.Lib;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;

namespace Zephyr.DOTSAStar.Runtime.System
{
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public abstract class PathFindingSystem<T> : JobComponentSystem
        where T : unmanaged, IComponentData, IPathFindingNode, IComparable<T>
    {
        private int _mapSize, _iterationLimit, _pathNodeLimit;
        
        private EntityCommandBufferSystem _resultECB;
        private EntityCommandBufferSystem _cleanECB;

        protected abstract int GetMapSize();
        protected abstract int GetIterationLimit();
        protected abstract int GetPathNodeLimit();

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
        private struct PrepareNodesJob : IJobForEachWithEntity<T>
        {
            public NativeArray<T> Nodes;
            public void Execute(Entity entity, int index, [ReadOnly] ref T node)
            {
                Nodes[index] = node;
            }
        }

        [BurstCompile]
        [ExcludeComponent(typeof(PathResult))]
        private struct PathFindingJob : IJobForEachWithEntity<PathFindingRequest>
        {
            public int MapSize;
            
            [ReadOnly][DeallocateOnJobCompletion]
            public NativeArray<T> Nodes;

            public EntityCommandBuffer.Concurrent ResultECB;

            public EntityCommandBuffer.Concurrent CleanECB;

            public int IterationLimit;

            public int PathNodeLimit;
            
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
                    Nodes[currentId].GetNeighbours(ref neighboursId);

                    foreach (var neighbourId in neighboursId)
                    {
                        //if cost == -1 means obstacle, skip
                        if (Nodes[neighbourId].GetCost() == -1) continue;

                        var currentCost = costCount[currentId] == int.MaxValue
                            ? 0
                            : costCount[currentId];
                        var newCost = currentCost + Nodes[neighbourId].GetCost();
                        //not better, skip
                        if (costCount[neighbourId] <= newCost) continue;
                        
                        var priority = newCost + Nodes[neighbourId].Heuristic(goalId);
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
                while (PathNodeLimit>0 && !nodeId.Equals(startId))
                {
                    buffer.Add(new PathRoute {Id = nodeId});
                    nodeId = cameFrom[nodeId];
                    PathNodeLimit--;
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
                }else if (PathNodeLimit <= 0 && !nodeId.Equals(startId))
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
            _mapSize = GetMapSize();
            _iterationLimit = GetIterationLimit();
            _pathNodeLimit = GetPathNodeLimit();
            
            _resultECB = World.Active.GetOrCreateSystem<EndInitializationEntityCommandBufferSystem>();
            _cleanECB = World.Active.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            //Prepare maps
            var nodes = new NativeArray<T>(_mapSize, Allocator.TempJob);
            
            var prepareNodesJob = new PrepareNodesJob
            {
                Nodes = nodes,
            };

            var prepareMapHandle = prepareNodesJob.Schedule(this, inputDeps);

            var sortNodesHandle = nodes.SortJob(prepareMapHandle);
            
            //Path Finding

            var pathFindingJob = new PathFindingJob
            {
                MapSize = _mapSize,
                Nodes = nodes,
                ResultECB = _resultECB.CreateCommandBuffer().ToConcurrent(),
                CleanECB = _cleanECB.CreateCommandBuffer().ToConcurrent(),
                IterationLimit = 2000,
                PathNodeLimit = 1000
            };
            
            var pathFindingHandle = pathFindingJob.Schedule(this, sortNodesHandle);
            _resultECB.AddJobHandleForProducer(pathFindingHandle);
            return pathFindingHandle;
        }
    }
}