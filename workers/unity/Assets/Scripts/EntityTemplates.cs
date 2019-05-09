using BetaApartUranus;
using Improbable;
using Improbable.Gdk.Core;
using UnityEngine;

namespace BlankProject
{
    public static class EntityTemplates
    {
        public static EntityTemplate Drone(GridCoordinate position, uint playerId)
        {
            // Create a HealthPickup component snapshot which is initially active and grants "heathValue" on pickup.
            var droneComponent = new Drone.Snapshot(playerId);
            var gridPositionComponent = new GridPosition.Snapshot(position);
            var worldPosition = new Coordinates()
            {
                X = Mathf.Sqrt(3f) * position.Col + Mathf.Sqrt(3f) / 2f * position.Row,
                Y = 3f / 2f * position.Row,
                Z = 0f,
            };

            var entityTemplate = new EntityTemplate();
            entityTemplate.AddComponent(new Position.Snapshot(worldPosition), WorkerUtils.UnityGameLogic);
            entityTemplate.AddComponent(new Metadata.Snapshot("HealthPickup"), WorkerUtils.UnityGameLogic);
            entityTemplate.AddComponent(new Persistence.Snapshot(), WorkerUtils.UnityGameLogic);
            entityTemplate.AddComponent(droneComponent, WorkerUtils.UnityGameLogic);
            entityTemplate.AddComponent(gridPositionComponent, WorkerUtils.UnityGameLogic);
            entityTemplate.SetReadAccess(WorkerUtils.UnityGameLogic, WorkerUtils.UnityClient);
            entityTemplate.SetComponentWriteAccess(EntityAcl.ComponentId, WorkerUtils.UnityGameLogic);

            return entityTemplate;
        }
    }
}

