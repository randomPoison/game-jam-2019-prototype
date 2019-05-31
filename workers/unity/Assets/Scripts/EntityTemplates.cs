using Improbable;
using Improbable.Gdk.Core;
using UnityEngine;

namespace BetaApartUranus
{
    public static class EntityTemplates
    {
        public static EntityTemplate Drone(GridCoordinate position, string playerId)
        {
            // Calculate the world position based off the grid position.
            var worldPosition = new Coordinates()
            {
                X = Mathf.Sqrt(3f) * position.Col + Mathf.Sqrt(3f) / 2f * position.Row,
                Y = 0f,
                Z = 3f / 2f * position.Row,
            };

            var entityTemplate = new EntityTemplate();
            entityTemplate.AddComponent(new Drone.Snapshot(playerId), WorkerUtils.UnityGameLogic);
            entityTemplate.AddComponent(new GridPosition.Snapshot(position), WorkerUtils.UnityGameLogic);

            entityTemplate.AddComponent(new Position.Snapshot(worldPosition), WorkerUtils.UnityGameLogic);
            entityTemplate.AddComponent(new Metadata.Snapshot("Drone"), WorkerUtils.UnityGameLogic);
            entityTemplate.AddComponent(new Persistence.Snapshot(), WorkerUtils.UnityGameLogic);
            entityTemplate.SetReadAccess(WorkerUtils.UnityGameLogic, WorkerUtils.UnityClient);
            entityTemplate.SetComponentWriteAccess(EntityAcl.ComponentId, WorkerUtils.UnityGameLogic);

            return entityTemplate;
        }
    }
}

