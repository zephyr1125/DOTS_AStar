using Unity.Entities;

namespace Zephyr.DOTSAStar.Runtime.Component
{
    public struct PathResult : IComponentData
    {
        public bool Success;
        public NativeString64 Log;
    }
}