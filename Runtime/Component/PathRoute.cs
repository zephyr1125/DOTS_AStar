using Unity.Entities;
using Unity.Mathematics;

namespace Zephyr.DOTSAStar.Runtime.Component
{
    public struct PathRoute : IBufferElementData
    {
        public int2 Position;
    }
}