using System;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;

namespace Classic
{
    public class ClassicFinder : MonoBehaviour
    {
        public Map Map;
        public PathFinding PathFinding;
        private AStarNode _goalNode;
        private Camera _camera;

        private void Awake()
        {
            _camera = Camera.main;
        }

        private void Update()
        {
            var goalPos = _camera.ScreenToWorldPoint(Input.mousePosition)+new Vector3(0.5f, 0.5f);
            var newGoal = Map.GetNode(new int2((int)goalPos.x, (int)goalPos.y));
            if (newGoal != null && newGoal != _goalNode &&
                newGoal.NodeType!=AStarNode.AStarNodeType.Start &&
                newGoal.NodeType!=AStarNode.AStarNodeType.Obstacle)
            {
                if (_goalNode != null)
                {
                    _goalNode.NodeType = AStarNode.AStarNodeType.Empty;
                    _goalNode.OnChangeNodeType();
                }
                
                _goalNode = newGoal;
                _goalNode.NodeType = AStarNode.AStarNodeType.Goal;
                _goalNode.OnChangeNodeType();
                
                FindPath();
            }
        }

        private void FindPath()
        {
            if (_goalNode == null) return;
            
            var path = PathFinding.Find(Map, Map.startNode, _goalNode);
            foreach (var node in Map.map)
            {
                switch (node.Value.NodeType)
                {
                    case AStarNode.AStarNodeType.Start:
                    case AStarNode.AStarNodeType.Obstacle:
                    case AStarNode.AStarNodeType.Goal:
                        continue;
                    default:
                        if (path.Contains(node.Value))
                        {
                            node.Value.NodeType = AStarNode.AStarNodeType.Path;
                        }
                        else
                        {
                            node.Value.NodeType = AStarNode.AStarNodeType.Empty;
                        }
                        node.Value.OnChangeNodeType();
                        break;
                }
            }
        }
    }
}