using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

namespace Classic
{
    public class Map : MonoBehaviour
    {
        public PathFinding PathFinding;
        
        private Dictionary<int2, AStarNode> _map;

        private void Start()
        {
            _map = new Dictionary<int2, AStarNode>();
            for (var i = 0; i < 9; i++)
            {
                for (var j = 0; j < 9; j++)
                {
                    var pos = new int2(i, j);
                    _map[pos] = new AStarNode(pos);
                }
            }
            
            PathFinding.Find(this, _map[new int2(2,3)]);
        }

        public AStarNode[] GetNeighbours(AStarNode node)
        {
            var neighbours = new List<AStarNode>();
            var nodePos = node.Position;
            int2[] neighbourIds =
            {
                nodePos + new int2(-1, 0),
                nodePos + new int2(1, 0),
                nodePos + new int2(0, -1),
                nodePos + new int2(0, 1),
            };
            foreach (var neighbourId in neighbourIds)
            {
                if (_map.ContainsKey(neighbourId))
                {
                    neighbours.Add(_map[neighbourId]);
                }
            }

            return neighbours.ToArray();
        }
    }
}