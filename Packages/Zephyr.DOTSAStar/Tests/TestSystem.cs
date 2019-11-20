using Unity.Collections;
using Zephyr.DOTSAStar.Runtime.System;

namespace Zephyr.DOTSAStar.Tests
{
    public class TestSystem : PathFindingSystem<TestNode>
    {
        public static NativeArray<int> GetNeighbourOffset(Allocator allocator)
        {
            return new NativeArray<int>(4, allocator)
            {
                [0] = -1,
                [1] = Const.MapWidth,
                [2] = 1,
                [3] = -Const.MapWidth,
            };
        }

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
    }
}