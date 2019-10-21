using System;
using Sirenix.OdinInspector;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

namespace Classic
{
    [ExecuteInEditMode]
    public class AStarNode : MonoBehaviour
    {
        public int2 position;
        
        [OnValueChanged("OnChangeTerrainType")]
        public TerrainType terrainType;

        [OnValueChanged("OnChangePathPart")]
        public PathPart pathPart;

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
    }
}