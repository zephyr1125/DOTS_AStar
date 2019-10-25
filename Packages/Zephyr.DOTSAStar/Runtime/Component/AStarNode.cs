using System;
using Unity.Entities;
using Unity.Mathematics;

namespace Zephyr.DOTSAStar.Runtime.Component
{
    public struct AStarNode : IComponentData, IComparable<AStarNode>
    {
        public int2 Position;
        public int Cost;
        
        public int CompareTo(AStarNode other)
        {
            return Utils.PosToId(Position) - Utils.PosToId(other.Position);
        }
    }
}