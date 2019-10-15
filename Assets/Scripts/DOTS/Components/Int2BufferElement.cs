using Unity.Entities;
using Unity.Mathematics;

namespace DOTS.Components
{
    public struct Int2BufferElement : IBufferElementData
    {
        public int2 value;
        public Int2BufferElement(int2 value)
        {
            this.value = value;
        }
    }
}