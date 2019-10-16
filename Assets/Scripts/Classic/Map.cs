using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Unity.Mathematics;
using UnityEngine;

namespace Classic
{
    public class Map : MonoBehaviour
    {
        public PathFinding pathFinding;
        
        public Dictionary<int2, AStarNode> map;

        public int2 mapSize;

        public GameObject nodePrefab;

        public AStarNode startNode;

        private void Start()
        {
            map = new Dictionary<int2, AStarNode>();
            foreach (var node in GetComponentsInChildren<AStarNode>())
            {
                map[node.Position] = node;
                if (node.NodeType == AStarNode.AStarNodeType.Start)
                {
                    startNode = node;
                }
            }
            Debug.Log(map);
        }
        
        [Button]
        public void Generate()
        {
            var childCount = transform.childCount;
            for (var i = childCount-1; i >= 0; i--)
            {
                DestroyImmediate(transform.GetChild(i).gameObject);
            }
            
            for (var i = 0; i < mapSize.x; i++)
            {
                for (var j = 0; j < mapSize.y; j++)
                {
                    var node = Instantiate(nodePrefab, new Vector3(i, j), Quaternion.identity, transform);
                    node.GetComponent<AStarNode>().Init(i, j);
                }
            }
        }

        public AStarNode GetNode(int2 Pos)
        {
            if (!map.ContainsKey(Pos)) return null;
            return map[Pos];
        }

        public AStarNode[] GetNeighbours(AStarNode node)
        {
            var neighbours = new List<AStarNode>();
            var nodePos = node.Position;
            int2[] neighbourIds =
            {
                nodePos + new int2(-1, 0),
                nodePos + new int2(0, 1),
                nodePos + new int2(1, 0),
                nodePos + new int2(0, -1),
            };
            foreach (var neighbourId in neighbourIds)
            {
                if (map.ContainsKey(neighbourId) && map[neighbourId].NodeType!=AStarNode.AStarNodeType.Obstacle)
                {
                    neighbours.Add(map[neighbourId]);
                }
            }

            return neighbours.ToArray();
        }
    }
}