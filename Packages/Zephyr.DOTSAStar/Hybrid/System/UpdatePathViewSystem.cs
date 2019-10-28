using Unity.Collections;
using Unity.Entities;
using Unity.Rendering;
using UnityEngine;
using Zephyr.DOTSAStar.Hybrid.Component;
using Zephyr.DOTSAStar.Runtime.Component;

namespace Zephyr.DOTSAStar.Hybrid.System
{
    public class UpdatePathViewSystem : ComponentSystem
    {
        private EntityQuery _pathViewQuery;
        private EntityQuery _pathResultQuery;

        private Material[] _pathMaterials;

        public void Init(Material[] pathMaterials)
        {
            _pathMaterials = pathMaterials;
        }

        protected override void OnCreate()
        {
            _pathViewQuery = GetEntityQuery(
                ComponentType.ReadOnly<AStarNodePathView>(),
                ComponentType.ReadWrite<RenderMesh>());
            _pathResultQuery = GetEntityQuery(
                ComponentType.ReadOnly<PathResult>(),
                ComponentType.ReadOnly<PathRoute>());
        }

        protected override void OnUpdate()
        {
            if (_pathMaterials == null) return;

            var resultEntities = _pathResultQuery.ToEntityArray(Allocator.TempJob);
            if (resultEntities.Length == 0)
            {
                resultEntities.Dispose();
                return;
            }
            
            var viewEntities = _pathViewQuery.ToEntityArray(Allocator.TempJob);
            var viewComponents =
                _pathViewQuery.ToComponentDataArray<AStarNodePathView>(Allocator.TempJob);
            
            DynamicBuffer<PathRoute>[] routeBuffers = new DynamicBuffer<PathRoute>[resultEntities.Length];
            for (var i = 0; i < resultEntities.Length; i++)
            {
                
            }

            for (var i = 0; i < viewEntities.Length; i++)
            {
                var pathEntity = viewEntities[i];
                var renderMesh = EntityManager.GetSharedComponentData<RenderMesh>(pathEntity);
                
                //reset material to empty
                renderMesh.material = _pathMaterials[0];
                
                var nodeId = viewComponents[i].Id;
                
                foreach (var resultEntity in resultEntities)
                {
                    var routeBuffer = EntityManager.GetBuffer<PathRoute>(resultEntity);
                    foreach (var pathRoute in routeBuffer)
                    {
                        if (pathRoute.Id == nodeId)
                        {
                            renderMesh.material = _pathMaterials[2];
                        }
                    }
                }

                EntityManager.SetSharedComponentData(pathEntity, renderMesh);
            }

            viewEntities.Dispose();
            viewComponents.Dispose();
            resultEntities.Dispose();
        }
    }
}