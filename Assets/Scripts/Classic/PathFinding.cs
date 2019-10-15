using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

namespace Classic
{
    public class PathFinding : MonoBehaviour
    {
        public void Find(Map map, AStarNode startNode)
        {
            var frontier = new Queue<AStarNode>();
            frontier.Enqueue(startNode);
            var visited = new Dictionary<AStarNode, bool> {[startNode] = true};

            while (frontier.Count>0)
            {
                var current = frontier.Dequeue();
                var neighbours = map.GetNeighbours(current);
                foreach (var neighbour in neighbours)
                {
                    if (visited.ContainsKey(neighbour)) continue;
                    frontier.Enqueue(neighbour);
                    visited[neighbour] = true;
                }
            }
            
            Debug.Log(visited);
        }
    }
}