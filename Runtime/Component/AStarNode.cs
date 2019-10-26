using System;
using Unity.Entities;
using Unity.Mathematics;

namespace Zephyr.DOTSAStar.Runtime.Component
{
    public struct AStarNode : IComponentData, IComparable<AStarNode>
    {
        public int Id;
        public int Cost;
        
        public int CompareTo(AStarNode other)
        {
            return Id.CompareTo(other.Id);
        }
    }
}