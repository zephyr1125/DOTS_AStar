using Unity.Collections;

namespace Zephyr.DOTSAStar.Runtime.ComponentInterface
{
    public interface IPathFindingNode
    {
        int GetCost();
        
        float Heuristic(int targetNodeId);
        
        void GetNeighbours(ref NativeList<int> neighboursId);
    }
}