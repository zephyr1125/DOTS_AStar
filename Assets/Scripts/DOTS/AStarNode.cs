using Unity.Mathematics;

namespace DOTS
{
    public struct AStarNode
    {
        public int2 position;
        public int index;
        public int parent;

        public float H { get; set; }
        public float G { get; set; }

        public AStarNode(int index, int2 position, int parent, float g, float h)
        {
            this.index = index;
            this.position = position;
            this.parent = parent;
            this.G = g;
            this.H = h;
        }

    }
}