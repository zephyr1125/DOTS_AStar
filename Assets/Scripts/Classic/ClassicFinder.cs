using System;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UIElements;

namespace Classic
{
    public class ClassicFinder : MonoBehaviour
    {
        public Map Map;
        public PathFinding PathFinding;
        private AStarNode _goalNode;
        private Camera _camera;
        public CostUI costUI;

        private void Awake()
        {
            _camera = Camera.main;
        }

        private void Update()
        {
//            if (!Input.GetMouseButtonUp(0)) return;
            
            var goalPos = _camera.ScreenToWorldPoint(Input.mousePosition)+new Vector3(0.5f, 0.5f);
            var newGoal = Map.GetNode(new int2((int)goalPos.x, (int)goalPos.y));
            if (newGoal != null && newGoal != _goalNode &&
                newGoal.pathPart!=AStarNode.PathPart.Start &&
                newGoal.terrainType!=AStarNode.TerrainType.Obstacle)
            {
                if (_goalNode != null)
                {
                    _goalNode.pathPart = AStarNode.PathPart.None;
                    _goalNode.OnChangePathPart();
                }
                    
                _goalNode = newGoal;
                _goalNode.pathPart = AStarNode.PathPart.Goal;
                _goalNode.OnChangePathPart();
                    
                FindPath();
            }
        }

        private void FindPath()
        {
            if (_goalNode == null) return;
            
            var path = PathFinding.Find(Map, Map.startNode, _goalNode);
            foreach (var node in Map.map)
            {
                if (node.Value.terrainType == AStarNode.TerrainType.Obstacle) continue;
                
                switch (node.Value.pathPart)
                {
                    case AStarNode.PathPart.Start:
                    case AStarNode.PathPart.Goal:
                        continue;
                    default:
                        if (path.Contains(node.Value))
                        {
                            node.Value.pathPart = AStarNode.PathPart.Route;
                        }
                        else
                        {
                            node.Value.pathPart = AStarNode.PathPart.None;
                        }
                        node.Value.OnChangePathPart();
                        break;
                }
            }
            costUI.UpdateCosts();
        }
    }
}