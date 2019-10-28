using System;
using Sirenix.OdinInspector;
using Unity.Entities;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;
using Zephyr.Define.Runtime;
using Zephyr.DOTSAStar.Hybrid;
using Zephyr.DOTSAStar.Hybrid.AuthoringComponent;
using Zephyr.DOTSAStar.Hybrid.System;
using Zephyr.DOTSAStar.Runtime;
using Zephyr.DOTSAStar.Runtime.DefineComponent;

#if UNITY_EDITOR

#endif

namespace DOTS
{
    public class Map : MonoBehaviour
    {
        public GameObject nodePrefab;
        
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
                    ((GameObject)node).GetComponent<AStarNode>().Init(Utils.PosToId(new int2(i, j)));
                }
            }
#endif
        }

        private void Start()
        {
            var pathMaterials = nodePrefab.GetComponent<AStarNode>().pathMaterials;

            var pathViewSystem = World.Active.GetOrCreateSystem<UpdatePathViewSystem>();
            pathViewSystem.Init(pathMaterials);
        }
    }
}