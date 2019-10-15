using Unity.Mathematics;

namespace DOTS
{
    public interface IHeuristicCostCalculator
    {
        float ComputeCost(int2 startPos, int2 endPos);
    }
}