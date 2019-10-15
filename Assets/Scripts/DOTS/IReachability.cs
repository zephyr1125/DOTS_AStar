using Unity.Mathematics;

namespace DOTS
{
    public interface IReachability
    {
        bool IsReachable(int2 targetPos);
    
        bool IsReachable(int2 fromPos, int2 targetPos);

        float GetWeight(int2 fromPos, int2 targetPos);
    }
}