using System;
using Sirenix.OdinInspector;
using Unity.Entities;
using UnityEngine;
using Zephyr.Define.Runtime;
using Zephyr.DOTSAStar.Runtime;
using Zephyr.DOTSAStar.Runtime.DefineComponent;
using Zephyr.DOTSAStar.Runtime.Lib;

namespace Zephyr.DOTSAStar.Hybrid.AuthoringComponent
{
    public class AStarNode : MonoBehaviour, IConvertGameObjectToEntity
    {
        [ReadOnly]
        public int id;
        
        [ReadOnly]
        public string nodeTypeName;

        [OnValueChanged("OnChangePathPart")]
        public PathPart pathPart;

#if UNITY_EDITOR
        [AssetList(CustomFilterMethod = "CustomFilter")]
        [OnValueChanged("UpdateNodeType")]
        public Define.Runtime.Define NodeDefine;

        private void UpdateNodeType()
        {
            nodeTypeName = NodeDefine.GetName();
            nodeTypeRenderer.material = nodeTypeMaterials[nodeTypeName];
        }
        
        protected virtual bool CustomFilter(Define.Runtime.Define define)
        {
            return define.GetComponent<PathFindingNode>()!=null;
        }
#endif
        
        #region Render

        [Button("更新NodeTypes")]
        public void UpdateNodeTypes()
        {
            var nodeTypeDefines = DefineCenter.Instance().GetDefinesOf<PathFindingNode>();
            
            if(nodeTypeMaterials == null)
                nodeTypeMaterials = new MaterialDictionary();
            
            foreach (var nodeTypeDefine in nodeTypeDefines)
            {
                if (!nodeTypeMaterials.ContainsKey(nodeTypeDefine.GetName()))
                {
                    nodeTypeMaterials.Add(nodeTypeDefine.GetName(), null);
                }
            }
        }
        
        [ShowInInspector]
        [InfoBox("增减NodeType后点击按钮刷新材质库")]
        public MaterialDictionary nodeTypeMaterials;

        public MeshRenderer nodeTypeRenderer;

        public Material[] pathMaterials;

        public MeshRenderer pathSpriteRender;

        #endregion
        
        

        public void Init(int id)
        {
            this.id = id;
            nodeTypeName = "empty";

            transform.GetChild(0).GetComponent<NodePathView>().id = id;
        }

        public void OnChangePathPart()
        {
            pathSpriteRender.material = pathMaterials[(int)pathPart];
        }

        private void OnMouseDown()
        {
            Debug.Log("click");
        }
        
        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            var define = DefineCenter.Instance().GetDefine(nodeTypeName).GetComponent<PathFindingNode>();
            dstManager.AddComponentData(entity, new Runtime.Component.AStarNode
            {
                Id = id,
                Cost = define.Cost
            });
        }
    }
    
    [Serializable]
    public class MaterialDictionary : UnitySerializedDictionary<string, Material> { }
}