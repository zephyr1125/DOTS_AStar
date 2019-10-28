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
                ComponentType.ReadOnly<PathFindingRequest>(),
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

            var resultRequests =
                _pathResultQuery.ToComponentDataArray<PathFindingRequest>(Allocator.TempJob);

            for (var indexNode = 0; indexNode < viewEntities.Length; indexNode++)
            {
                var pathEntity = viewEntities[indexNode];
                var renderMesh = EntityManager.GetSharedComponentData<RenderMesh>(pathEntity);
                
                //reset material to empty
                renderMesh.material = _pathMaterials[0];
                
                var nodeId = viewComponents[indexNode].Id;

                for (var indexResult = 0; indexResult < resultEntities.Length; indexResult++)
                {
                    //start node
                    var requestStartId = resultRequests[indexResult].StartId;
                    if (nodeId == requestStartId)
                    {
                        renderMesh.material =_pathMaterials[1];
                        break;
                    }
                    
                    //route node
                    var routeBuffer = EntityManager.GetBuffer<PathRoute>(resultEntities[indexResult]);
                    for (var indexRoute = 0; indexRoute < routeBuffer.Length; indexRoute++)
                    {
                        var pathRoute = routeBuffer[indexRoute];
                        if (pathRoute.Id == nodeId)
                        {
                            renderMesh.material =
                                indexRoute == 0 ? _pathMaterials[3] : _pathMaterials[2];
                        }
                    }
                }

                EntityManager.SetSharedComponentData(pathEntity, renderMesh);
            }

            viewEntities.Dispose();
            viewComponents.Dispose();
            resultRequests.Dispose();
            resultEntities.Dispose();
        }
    }
}