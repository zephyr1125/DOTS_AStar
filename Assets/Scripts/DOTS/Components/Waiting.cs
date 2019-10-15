using Unity.Entities;

namespace DOTS.Components
{
    public struct Waiting : IComponentData
    {
        public bool done;
    }
}