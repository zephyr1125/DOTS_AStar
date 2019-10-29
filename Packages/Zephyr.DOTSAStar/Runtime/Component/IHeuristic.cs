namespace Zephyr.DOTSAStar.Runtime.Component
{
    public interface IHeuristic
    {
        float Heuristic(int targetNodeId);
    }
}