//using Unity.Entities;
//using Unity.Jobs;
//using Unity.Rendering;
//using Zephyr.DOTSAStar.Hybrid.Component;
//using Zephyr.DOTSAStar.Runtime.Component;
//using NotImplementedException = System.NotImplementedException;
//
//namespace Zephyr.DOTSAStar.Hybrid.System
//{
//    public class ShowPathSystem : JobComponentSystem
//    {
//        private EntityQuery _pathResultQuery;
//        private EntityQuery _pathViewQuery;
//
//        protected override void OnCreate()
//        {
//            _pathResultQuery = GetEntityQuery(
//                ComponentType.ReadOnly<PathResult>());
//            _pathViewQuery = GetEntityQuery(
//                ComponentType.ReadOnly<AStarNodePathView>());
//        }
//        
//        private struct CleanAllPathJob : IJobForEachWithEntity<AStarNodePathView>
//        {
//            public void Execute(Entity entity, int index, ref AStarNodePathView pathView)
//            {
//                
//            }
//        }
//
//        protected override JobHandle OnUpdate(JobHandle inputDeps)
//        {
//            //no path result, not update
//            if (_pathResultQuery.CalculateEntityCount() == 0)
//            {
//                return inputDeps;
//            }
//
//            return inputDeps;
//        }
//    }
//}