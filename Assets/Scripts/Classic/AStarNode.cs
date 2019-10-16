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

        public enum TerrainType
        {
            Empty, Obstacle, Swamp
        }
        public static readonly int[] Costs = {1,int.MaxValue,3};

        public Sprite[] terrainSprites;

        [OnValueChanged("OnChangeTerrainType")]
        public TerrainType terrainType;

        public SpriteRenderer terrainRenderer;

        public SpriteRenderer pathSpriteRender;

        public Sprite[] pathSprites;
        
        public enum PathPart
        {
            None, Start, Route, Goal
        }

        [OnValueChanged("OnChangePathPart")]
        public PathPart pathPart;

        public int costCount;
        
        public Text text;

        public void Init(int x, int y)
        {
            position = new int2(x, y);
            terrainType = TerrainType.Empty;
        }

        public void OnChangeTerrainType()
        {
            terrainRenderer.sprite = terrainSprites[(int)terrainType];
        }

        public void OnChangePathPart()
        {
            pathSpriteRender.sprite = pathSprites[(int)pathPart];
        }

        private void OnMouseDown()
        {
            Debug.Log("click");
        }
    }
}