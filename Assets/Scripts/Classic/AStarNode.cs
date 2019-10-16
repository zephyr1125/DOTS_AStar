using System;
using Sirenix.OdinInspector;
using Unity.Mathematics;
using UnityEngine;

namespace Classic
{
    [ExecuteInEditMode]
    public class AStarNode : MonoBehaviour
    {
        public int2 Position;

        public enum AStarNodeType
        {
            Empty, Obstacle, Start, Goal, Path
        }

        public Sprite[] Sprites;

        [OnValueChanged("OnChangeNodeType")]
        public AStarNodeType NodeType;

        private SpriteRenderer _spriteRenderer;
        
        public void Init(int x, int y)
        {
            Position = new int2(x, y);
            NodeType = AStarNodeType.Empty;
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }

        public void OnChangeNodeType()
        {
            if (_spriteRenderer == null) _spriteRenderer = GetComponent<SpriteRenderer>();
            _spriteRenderer.sprite = Sprites[(int)NodeType];
        }

        private void OnMouseDown()
        {
            Debug.Log("click");
        }
    }
}