using Unity.Entities;

namespace DOTS.Components
{
    public struct AStarPath : IComponentData
    {
        public int pathCount;
        public bool reachable;
        public AStarPath(int pathCount, bool reachable)
        {
            this.pathCount = pathCount;
            this.reachable = reachable;
        }
    }
}