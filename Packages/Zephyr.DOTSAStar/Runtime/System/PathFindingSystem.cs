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
    [UpdateBefore(typeof(PathFindingECBuffer))]
    public class PathFindingSystem : JobComponentSystem
    {
        private int _mapSize;
        private PathFindingECBuffer _cmdBuffer;

        private struct OpenSetNode : IComparable<OpenSetNode>, IEquatable<OpenSetNode>
        {
            public int2 Position;
            public float Priority;

            public OpenSetNode(int2 position, float priority)
            {
                this.Position = position;
                this.Priority = priority;
            }
            
            public int CompareTo(OpenSetNode other)
            {
                return -Priority.CompareTo(other.Priority);
            }

            public bool Equals(OpenSetNode other)
            {
                return Position.Equals(other.Position);
            }
        }
        
        [BurstCompile]
        private struct PrepareNodesJob : IJobForEachWithEntity<Component.AStarNode>
        {
            public NativeArray<AStarNode> AStarNodes;
            public void Execute(Entity entity, int index, [ReadOnly] ref AStarNode aStarNode)
            {
                AStarNodes[index] = aStarNode;
            }
        }

        [BurstCompile]
        private struct PathFindingJob : IJobForEachWithEntity<PathFindingRequest>
        {
            public int MapSize;
            
            [ReadOnly][DeallocateOnJobCompletion]
            public NativeArray<AStarNode> AStarNodes;
            
            [ReadOnly][DeallocateOnJobCompletion]
            public NativeArray<int2> NeighboursOffset;

            public EntityCommandBuffer.Concurrent CmdBuffer;

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
                var startPos = request.StartPos;
                var goalPos = request.GoalPos;
                
                openSet.Push(new MinHeapNode(startPos, 0));
                costCount[Utils.PosToId(startPos)] = 0;

                var currentId = -1;
                while (IterationLimit>0 && openSet.HasNext())
                {
                    var currentNode = openSet[openSet.Pop()];
                    var currentPos = currentNode.Position;
                    currentId = Utils.PosToId(currentPos);
                    if (currentId == Utils.PosToId(goalPos))
                    {
                        break;
                    }
                    
                    var neighboursPos = new NativeList<int2>(4, Allocator.Temp);
                    GetNeighbours(currentPos, ref neighboursPos, NeighboursOffset);

                    foreach (var neighbourPos in neighboursPos)
                    {
                        var neighbourId = Utils.PosToId(neighbourPos);
                        
                        //if cost == -1 means obstacle, skip
                        if (AStarNodes[neighbourId].Cost == -1) continue;

                        var currentCost = costCount[currentId] == int.MaxValue
                            ? 0
                            : costCount[currentId];
                        var newCost = currentCost + AStarNodes[neighbourId].Cost;
                        //not better, skip
                        if (costCount[neighbourId] <= newCost) continue;
                        
                        var priority = newCost + Heuristic(neighbourPos, goalPos);
                        openSet.Push(new MinHeapNode(neighbourPos, priority));
                        cameFrom[neighbourId] = currentId;
                        costCount[neighbourId] = newCost;
                    }

                    IterationLimit--;
                    neighboursPos.Dispose();
                }
                
                //Construct path
                var pathEntity = CmdBuffer.CreateEntity(index);
                var buffer = CmdBuffer.AddBuffer<PathRoute>(index, pathEntity);
                var nodePos = goalPos;
                while (StepsLimit>0 && !nodePos.Equals(startPos))
                {
                    buffer.Add(new PathRoute {Position = nodePos});
                    nodePos = Utils.IdToPos(cameFrom[Utils.PosToId(nodePos)]);
                    StepsLimit--;
                }
                
                //Construct Result
                var success = true;
                var log = new NativeString64("Path finding success");
                if (!openSet.HasNext() && currentId != Utils.PosToId(goalPos))
                {
                    success = false;
                    log = new NativeString64("Out of openset");
                }
                if (IterationLimit <= 0 && currentId != Utils.PosToId(goalPos))
                {
                    success = false;
                    log = new NativeString64("Iteration limit reached");
                }else if (StepsLimit <= 0 && !nodePos.Equals(startPos))
                {
                    success = false;
                    log = new NativeString64("Step limit reached");
                }
                CmdBuffer.AddComponent(index, pathEntity, new PathResult
                {
                    Success = success,
                    Log = log
                });
                
                //Remove request
                CmdBuffer.DestroyEntity(index, entity);
                
                //Clear
                openSet.Dispose();
                cameFrom.Dispose();
                costCount.Dispose();
            }

            private static void GetNeighbours(int2 pos, ref NativeList<int2> neighboursPos, NativeArray<int2> neighboursOffset)
            {
                foreach (var offset in neighboursOffset)
                {
                    var newPos = pos + offset;
                    if (newPos.x < 0 || newPos.x >= Const.MapWidth) continue;
                    if (newPos.y < 0 || newPos.y >= Const.MapHeight) continue;
                    neighboursPos.Add(newPos);
                }
            }

            private static float Heuristic(int2 posA, int2 posB)
            {
                var x   = posB.x - posA.x;
                var y   = posB.y - posA.y;
                var sqr = x * x + y * y;
                return sqr;
            }
        }
        
        protected override void OnCreate()
        {
            _mapSize = Const.MapWidth * Const.MapHeight;
            _cmdBuffer = World.Active.GetOrCreateSystem<PathFindingECBuffer>();
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
            var neighbourOffset = new NativeArray<int2>(4, Allocator.TempJob)
            {
                [0] = new int2(-1, 0),
                [1] = new int2(0, 1),
                [2] = new int2(1, 0),
                [3] = new int2(0, -1),
            };

            var pathFindingJob = new PathFindingJob
            {
                MapSize = _mapSize,
                AStarNodes = aStarNodes,
                NeighboursOffset = neighbourOffset,
                CmdBuffer = _cmdBuffer.CreateCommandBuffer().ToConcurrent(),
                IterationLimit = 2000,
                StepsLimit = 1000
            };
            
            var pathFindingHandle = pathFindingJob.Schedule(this, sortNodesHandle);
            _cmdBuffer.AddJobHandleForProducer(pathFindingHandle);
            return pathFindingHandle;
        }
    }
    
    public class PathFindingECBuffer : EntityCommandBufferSystem{}
}