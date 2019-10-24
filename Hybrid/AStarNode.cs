using Zephyr.DOTSAStar.Runtime;
using Sirenix.OdinInspector;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Zephyr.DOTSAStar.Hybrid
{
    public class AStarNode : MonoBehaviour, IConvertGameObjectToEntity
    {
        public int2 position;
        
        [OnValueChanged("OnChangeTerrainType")]
        public TerrainType terrainType;

        [OnValueChanged("OnChangePathPart")]
        public PathPart pathPart;

        [HideInInspector]
        public int costCount;

        #region Render

        public Material[] terrainMaterials;

        public MeshRenderer terrainRenderer;

        public Material[] pathMaterials;

        public MeshRenderer pathSpriteRender;

        #endregion
        

        public void Init(int x, int y)
        {
            position = new int2(x, y);
            terrainType = TerrainType.Empty;
        }

        public void OnChangeTerrainType()
        {
            terrainRenderer.material = terrainMaterials[(int)terrainType];
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
            dstManager.AddComponentData(entity, new Runtime.Component.AStarNode
            {
                Position = position,
                Cost = Const.TerrainCosts[(int)terrainType],
                TerrainType = terrainType,
                PathPart = pathPart
            });
        }
    }
}