using Unity.Mathematics;

namespace DOTS
{
    public struct ReachabilityType : IReachability
    {
        public bool IsReachable(int2 targetPos)
        {
            throw new System.NotImplementedException();
        }

        public bool IsReachable(int2 fromPos, int2 targetPos)
        {
            throw new System.NotImplementedException();
        }

        public float GetWeight(int2 fromPos, int2 targetPos)
        {
            throw new System.NotImplementedException();
        }
    }
}