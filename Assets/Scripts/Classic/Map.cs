using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Unity.Mathematics;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Classic
{
    public class Map : MonoBehaviour
    {
        public Dictionary<int2, AStarNode> map;

        public GameObject nodePrefab;

        [HideInInspector]
        public AStarNode startNode;

        private void Start()
        {
            map = new Dictionary<int2, AStarNode>();
            foreach (var node in GetComponentsInChildren<AStarNode>())
            {
                map[node.position] = node;
                if (node.pathPart == PathPart.Start)
                {
                    startNode = node;
                }
            }
            Debug.Log(map);
        }
        
        [Button]
        public void Generate()
        {
#if UNITY_EDITOR
            var childCount = transform.childCount;
            for (var i = childCount-1; i >= 0; i--)
            {
                DestroyImmediate(transform.GetChild(i).gameObject);
            }
            
            for (var i = 0; i < Const.MapWidth; i++)
            {
                for (var j = 0; j < Const.MapHeight; j++)
                {
                    var node = PrefabUtility.InstantiatePrefab(nodePrefab, transform);
                    node.name = "node[" + i + "," + j + "]";
                    ((GameObject)node).transform.position = new Vector3(i, j);
                    ((GameObject)node).GetComponent<AStarNode>().Init(i, j);
                }
            }
#endif
        }

        public AStarNode GetNode(int2 Pos)
        {
            if (!map.ContainsKey(Pos)) return null;
            return map[Pos];
        }

        public AStarNode[] GetNeighbours(AStarNode node)
        {
            var neighbours = new List<AStarNode>();
            var nodePos = node.position;
            int2[] neighbourIds =
            {
                nodePos + new int2(-1, 0),
                nodePos + new int2(0, 1),
                nodePos + new int2(1, 0),
                nodePos + new int2(0, -1),
            };
            foreach (var neighbourId in neighbourIds)
            {
                if (map.ContainsKey(neighbourId) && map[neighbourId].terrainType!=TerrainType.Obstacle)
                {
                    neighbours.Add(map[neighbourId]);
                }
            }

            return neighbours.ToArray();
        }
    }
}