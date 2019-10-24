using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using Zephyr.DOTSAStar.Runtime.Component;

namespace DOTS
{
    public class DOTSFinder : MonoBehaviour
    {
        public Map Map;
        private int2 _goalNode;
        private Camera _camera;
        public CostUI costUI;

        private EntityManager _entityManager;

        private void Awake()
        {
            _camera = Camera.main;
            _entityManager = World.Active.EntityManager;
        }

        private void Update()
        {
//            if (!Input.GetMouseButtonUp(0)) return;
            
            var goalPos = _camera.ScreenToWorldPoint(Input.mousePosition)+new Vector3(0.5f, 0.5f);
            var newGoal = new int2((int)goalPos.x, (int)goalPos.y);
            if (IsPosInMap(newGoal) && !newGoal.Equals(_goalNode))
            {
                _goalNode = newGoal;
                FindPath();
            }
        }

        private bool IsPosInMap(int2 pos)
        {
            if (pos.x < 0 || pos.x >= Const.MapWidth) return false;
            if (pos.y < 0 || pos.y >= Const.MapHeight) return false;
            return true;
        }

        private void FindPath()
        {
            var requestEntity = _entityManager.CreateEntity();
            _entityManager.SetName(requestEntity, "pf[0,0]->["+_goalNode.x+","+_goalNode.y+"]");
            _entityManager.AddComponentData(requestEntity, new PathFindingRequest
            {
                GoalPos = _goalNode,
                //todo temperate set fix position
                StartPos = new int2(0,9)
            });
        }
    }
}