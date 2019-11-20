using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Zephyr.DOTSAStar.Runtime.ComponentInterface;

namespace Zephyr.DOTSAStar.Tests
{
    public struct TestNode : IComponentData, IComparable<TestNode>, IPathFindingNode
    {
        public int Id;
        public int Cost;
        
        public int CompareTo(TestNode other)
        {
            return Id.CompareTo(other.Id);
        }

        public int GetCost()
        {
            return Cost;
        }

        public float Heuristic(int targetNodeId)
        {
            var startPos = Utils.IdToPos(Id);
            var endPos = Utils.IdToPos(targetNodeId);
            var x   = endPos.x - startPos.x;
            var y   = endPos.y - startPos.y;
            var sqr = x * x + y * y;
            return math.sqrt(sqr);
        }
        
        public void GetNeighbours(ref NativeList<int> neighboursId)
        {
            var neighbours = TestSystem.GetNeighbourOffset(Allocator.Temp);
            foreach (var offset in neighbours)
            {
                var currentPos = Utils.IdToPos(Id);
                var offsetPos = Utils.IdToPos(offset);
                var neighbourPos = currentPos+offsetPos;
                if (neighbourPos.x < 0 || neighbourPos.x >= Const.MapWidth) continue;
                if (neighbourPos.y < 0 || neighbourPos.y >= Const.MapHeight) continue;
                neighboursId.Add(Utils.PosToId(neighbourPos));
            }
            neighbours.Dispose();
        }
    }
}