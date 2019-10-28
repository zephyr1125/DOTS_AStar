using Sirenix.OdinInspector;
using Unity.Entities;
using UnityEngine;
using Zephyr.DOTSAStar.Hybrid.Component;

namespace Zephyr.DOTSAStar.Hybrid.AuthoringComponent
{
    public class NodePathView : MonoBehaviour, IConvertGameObjectToEntity
    {
        [ReadOnly]
        public int id;
        
        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            dstManager.AddComponentData(entity, new AStarNodePathView{Id = id});
        }
    }
}