using Unity.Collections;

namespace Zephyr.DOTSAStar.Runtime.Component
{
    public interface IPathFindingNode
    {
        int GetCost();
        
        float Heuristic(int targetNodeId);
        
        void GetNeighbours(ref NativeList<int> neighboursId);
    }
}