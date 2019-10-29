using DOTS.Component;
using Unity.Collections;
using Zephyr.DOTSAStar.Runtime.System;

namespace DOTS.System
{
    public class AStarPathFindingSystem : PathFindingSystem<AStarNode>
    {
        public static NativeArray<int> NeighbourOffset = 
            new NativeArray<int>(4, Allocator.Persistent)
            {
                [0] = -1,
                [1] = Const.MapWidth,
                [2] = 1,
                [3] = -Const.MapWidth,
            };

        protected override int GetMapSize()
        {
            return Const.MapWidth * Const.MapHeight;
        }

        protected override int GetIterationLimit()
        {
            return 2000;
        }

        protected override int GetPathNodeLimit()
        {
            return 1000;
        }

        protected override void OnDestroy()
        {
            NeighbourOffset.Dispose();
        }
    }
}