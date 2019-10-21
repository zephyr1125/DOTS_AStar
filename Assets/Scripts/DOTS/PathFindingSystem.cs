using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using AStarNode = DOTS.Components.AStarNode;

namespace DOTS
{
    public class PathFindingSystem : JobComponentSystem
    {
        [BurstCompile]
        private struct PrepareMapJob : IJobForEachWithEntity<AStarNode>
        {
            public NativeArray<int2> PositionMap;
            public NativeArray<TerrainType> TerrainMap;
            public NativeArray<int> CostMap;
            public void Execute(Entity entity, int index, [ReadOnly] ref AStarNode aStarNode)
            {
                PositionMap[index] = aStarNode.Position;
                TerrainMap[index] = aStarNode.TerrainType;
                CostMap[index] = aStarNode.Cost;
            }
        }
        
        protected override void OnCreate()
        {
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            //Prepare maps
            var positionMap = new NativeArray<int2>(
                Const.MapWidth*Const.MapHeight, Allocator.TempJob);
            var terrainMap = new NativeArray<TerrainType>(
                Const.MapWidth*Const.MapHeight, Allocator.TempJob);
            var costMap = new NativeArray<int>(
                Const.MapWidth*Const.MapHeight, Allocator.TempJob);
            
            var prepareMapJob = new PrepareMapJob
            {
                PositionMap = positionMap,
                TerrainMap = terrainMap,
                CostMap    = costMap
            };

            var prepareMapHandle = prepareMapJob.Schedule(this, inputDeps);
            
            //Path Finding
            var openSet = new NativeMinHeap(
                Const.MapHeight*Const.MapWidth, Allocator.TempJob);

            positionMap.Dispose();
            terrainMap.Dispose();
            costMap.Dispose();
            openSet.Dispose();

            return prepareMapHandle;
        }

        private static int PosToId(int2 position)
        {
            return position.y * Const.MapWidth + position.x;
        }
    }
}