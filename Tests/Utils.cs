using Unity.Mathematics;

namespace Zephyr.DOTSAStar.Tests
{
    public class Utils
    {
        public static int PosToId(int2 position)
        {
            return position.y * Const.MapWidth + position.x;
        }
            
        public static int2 IdToPos(int id)
        {
            return new int2(id%Const.MapWidth, id/Const.MapWidth);
        }
    }
}