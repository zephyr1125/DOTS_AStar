using Unity.Mathematics;

namespace Classic
{
    public class AStarNode
    {
        public int2 Position;
        public int Index;

        public AStarNode(int2 position)
        {
            this.Position = position;
        }
    }
}