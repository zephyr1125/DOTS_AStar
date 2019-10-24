using Unity.Entities;
using Unity.Mathematics;

namespace Zephyr.DOTSAStar.Runtime.Component
{
    public struct PathFindingRequest : IComponentData
    {
        public int2 StartPos, GoalPos;
    }
}