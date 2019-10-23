using Unity.Entities;
using Unity.Mathematics;

namespace Zephyr.DOTSAStar.Core.Component
{
    public struct PathRoute : IBufferElementData
    {
        public int2 Position;
    }
}