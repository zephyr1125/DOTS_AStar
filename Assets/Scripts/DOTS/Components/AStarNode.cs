using System;
using Unity.Entities;
using Unity.Mathematics;

namespace DOTS.Components
{
    public struct AStarNode : IComponentData, IComparable<AStarNode>
    {
        public int2 Position;
        public int Cost;
        public TerrainType TerrainType;
        public PathPart PathPart;
        public int CompareTo(AStarNode other)
        {
            return Utils.PosToId(Position) - Utils.PosToId(other.Position);
        }
    }
}