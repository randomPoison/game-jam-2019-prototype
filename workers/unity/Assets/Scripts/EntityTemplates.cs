using System.Collections.Generic;
using HexTools;
using Improbable;
using Improbable.Gdk.Core;

namespace BetaApartUranus
{
    public static class EntityTemplates
    {
        public static EntityTemplate Drone(AxialCoordinate position, string playerId)
        {
            // Calculate the world position based off the grid position.
            var worldPosition = HexUtils.GridToWorld(position);

            var entityTemplate = new EntityTemplate();
            entityTemplate.AddComponent(new Drone.Snapshot(playerId, new List<Command>()), WorkerUtils.UnityGameLogic);
            entityTemplate.AddComponent(new GridPosition.Snapshot(new GridCoordinate(position.Q, position.R)), WorkerUtils.UnityGameLogic);

            entityTemplate.AddComponent(
                new Position.Snapshot(new Coordinates
                {
                    X = worldPosition.x,
                    Z = worldPosition.y
                }),
                WorkerUtils.UnityGameLogic);
            entityTemplate.AddComponent(new Metadata.Snapshot("Drone"), WorkerUtils.UnityGameLogic);
            entityTemplate.AddComponent(new Persistence.Snapshot(), WorkerUtils.UnityGameLogic);
            entityTemplate.SetReadAccess(WorkerUtils.UnityGameLogic, WorkerUtils.UnityClient);
            entityTemplate.SetComponentWriteAccess(EntityAcl.ComponentId, WorkerUtils.UnityGameLogic);

            return entityTemplate;
        }
    }
}
