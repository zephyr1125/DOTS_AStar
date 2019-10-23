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
                newGoal.pathPart!=PathPart.Start &&
                newGoal.terrainType!=TerrainType.Obstacle)
            {
                if (_goalNode != null)
                {
                    _goalNode.pathPart = PathPart.None;
                    _goalNode.OnChangePathPart();
                }
                    
                _goalNode = newGoal;
                _goalNode.pathPart = PathPart.Goal;
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
                if (node.Value.terrainType == TerrainType.Obstacle) continue;
                
                switch (node.Value.pathPart)
                {
                    case PathPart.Start:
                    case PathPart.Goal:
                        continue;
                    default:
                        if (path.Contains(node.Value))
                        {
                            node.Value.pathPart = PathPart.Route;
                        }
                        else
                        {
                            node.Value.pathPart = PathPart.None;
                        }
                        node.Value.OnChangePathPart();
                        break;
                }
            }
            costUI.UpdateCosts();
        }
    }
}