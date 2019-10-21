using Unity.Entities;

namespace DOTS
{
    public class AStarNodeDOTS : Classic.AStarNode, IConvertGameObjectToEntity
    {
        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            dstManager.AddComponentData(entity, new Components.AStarNode
            {
                Position = position,
                Cost = Const.TerrainCosts[(int)terrainType],
                TerrainType = terrainType,
                PathPart = pathPart
            });
        }
    }
}