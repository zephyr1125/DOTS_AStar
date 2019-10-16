using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

namespace Classic
{
    public class PathFinding : MonoBehaviour
    {
        public AStarNode[] Find(Map map, AStarNode startNode, AStarNode goalNode)
        {
            var frontier = new Queue<AStarNode>();
            frontier.Enqueue(startNode);
            var cameFrom = new Dictionary<AStarNode, AStarNode> {[startNode] = null};

            while (frontier.Count>0)
            {
                var current = frontier.Dequeue();
                var neighbours = map.GetNeighbours(current);
                foreach (var neighbour in neighbours)
                {
                    if (cameFrom.ContainsKey(neighbour)) continue;
                    frontier.Enqueue(neighbour);
                    cameFrom[neighbour] = current;
                }
            }

            var node = goalNode;
            var path = new List<AStarNode>();
            while (node != startNode)
            {
                path.Add(node);
                node = cameFrom[node];
            }
            path.Reverse();
            return path.ToArray();
        }
    }
}