using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

namespace Classic
{
    public class PathFinding : MonoBehaviour
    {
        public AStarNode[] Find(Map map, AStarNode startNode, AStarNode goalNode)
        {
            foreach (var kv in map.map)
            {
                kv.Value.costCount = 0;
            }
            
            var frontier = new PriorityQueue<AStarNode>();
            frontier.Enqueue(startNode, 0);
            var cameFrom = new Dictionary<AStarNode, AStarNode> {[startNode] = null};
            var costCount = new Dictionary<AStarNode, int> {[startNode] = 0};

            while (frontier.Count>0)
            {
                var current = frontier.Dequeue();
                if (current == goalNode) break;
                
                var neighbours = map.GetNeighbours(current);
                for (var i = 0; i < neighbours.Length; i++)
                {
                    var neighbour = neighbours[i];
                    var newCost = costCount[current] + Const.TerrainCosts[(int)neighbour.terrainType];
                    if (costCount.ContainsKey(neighbour) && costCount[neighbour]<=newCost) continue;
                    frontier.Enqueue(neighbour, newCost + Heuristic(neighbour, goalNode));
                    cameFrom[neighbour] = current;
                    costCount[neighbour] = newCost;
                    neighbour.costCount = newCost;
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

        private double Heuristic(AStarNode a, AStarNode b)
        {
            return Mathf.Abs(a.position.x - b.position.x) + Mathf.Abs(a.position.y - b.position.y);
        }
    }
}