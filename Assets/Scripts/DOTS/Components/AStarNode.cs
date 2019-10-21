using Unity.Entities;
using Unity.Mathematics;

namespace DOTS.Components
{
    public struct AStarNode : IComponentData
    {
        public int2 Position;
        public int Cost;
        public TerrainType TerrainType;
        public PathPart PathPart;
    }
}