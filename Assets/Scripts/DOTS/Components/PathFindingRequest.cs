using Unity.Entities;
using Unity.Mathematics;

namespace DOTS.Components
{
    public struct PathFindingRequest : IComponentData
    {
        public int2 StartPos, GoalPos;
    }
}