using Unity.Entities;
using Unity.Mathematics;

namespace Zephyr.DOTSAStar.Core.Component
{
    public struct PathFindingRequest : IComponentData
    {
        public int2 StartPos, GoalPos;
    }
}