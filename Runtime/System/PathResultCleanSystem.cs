//using Unity.Collections;
//using Unity.Entities;
//using Unity.Jobs;
//using Zephyr.DOTSAStar.Runtime.Component;
//
//namespace Zephyr.DOTSAStar.Runtime.System
//{
//    [UpdateInGroup(typeof(LateSimulationSystemGroup))]
//    public class PathResultCleanSystem : JobComponentSystem
//    {
//        private EntityCommandBufferSystem _ecbSystem;
//
//        protected override void OnCreate()
//        {
//            _ecbSystem = World.Active.GetOrCreateSystem<BeginPresentationEntityCommandBufferSystem>();
//        }
//
//        [RequireComponentTag(typeof(PathFindingRequest))]
//        private struct PathResultCleanJob : IJobForEachWithEntity<PathResult>
//        {
//            public EntityCommandBuffer.Concurrent ECBuffer;
//            
//            public void Execute(Entity entity, int index, [ReadOnly]ref PathResult result)
//            {
//                ECBuffer.DestroyEntity(index, entity);
//            }
//        }
//        
//        protected override JobHandle OnUpdate(JobHandle inputDeps)
//        {
//            var job = new PathResultCleanJob
//            {
//                ECBuffer = _ecbSystem.CreateCommandBuffer().ToConcurrent()
//            };
//            return job.Schedule(this, inputDeps);
//        }
//    }
//}