using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using Zephyr.DOTSAStar.Core;
using Zephyr.DOTSAStar.Hybrid;

#if UNITY_EDITOR

#endif

namespace Zephyr.DOTSAStar.Example
{
    public class Map : MonoBehaviour
    {
        public GameObject nodePrefab;

        [HideInInspector]
        public AStarNode startNode;
        
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
    }
}