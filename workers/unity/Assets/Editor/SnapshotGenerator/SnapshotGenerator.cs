using HexTools;
using Improbable;
using Improbable.Gdk.Core;
using Improbable.Gdk.PlayerLifecycle;
using UnityEngine;

namespace BetaApartUranus.Editor
{
    internal static class SnapshotGenerator
    {
        public struct Arguments
        {
            public string OutputPath;
            public int NumResourceNodes;
            public Vector2 WorldDimensions;
        }

        public static void Generate(Arguments arguments)
        {
            Debug.Log("Generating snapshot.");
            var snapshot = CreateSnapshot(arguments);

            Debug.Log($"Writing snapshot to: {arguments.OutputPath}");
            snapshot.WriteToFile(arguments.OutputPath);
        }

        private static Snapshot CreateSnapshot(Arguments arguments)
        {
            var snapshot = new Snapshot();

            AddPlayerSpawner(snapshot);
            AddSpawnController(snapshot);

            var halfWidth = arguments.WorldDimensions.x / 2f;
            var halfHeight = arguments.WorldDimensions.y / 2f;

            for (var i = 0; i < arguments.NumResourceNodes; i += 1)
            {
                // Generate random grid position that is within the real world bounds.
                //
                // TODO: Use a better algorithm for picking random grid coordinates.
                // We're currently picking random spots in a skewed rectangle, and
                // it would better to more evenly distrubte the nodes around the
                // center of the map.
                AxialCoordinate gridPos;
                Vector2 worldPos;
                do
                {
                    gridPos = new AxialCoordinate(
                        (int)Random.Range(-halfWidth, halfWidth),
                        (int)Random.Range(-halfHeight, halfHeight));
                    worldPos = HexUtils.GridToWorld(gridPos);
                } while (Mathf.Abs(worldPos.x) >= halfWidth || Mathf.Abs(worldPos.y) >= halfHeight);

                var type = Random.value < 0.5f ? ResourceType.Coal : ResourceType.Copper;

                AddResourceNode(snapshot, gridPos, type);
            }

            return snapshot;
        }

        private static void AddResourceNode(Snapshot snapshot, AxialCoordinate position, ResourceType type)
        {
            var template = new EntityTemplate();

            var worldPos = HexUtils.GridToWorld(position);
            template.AddComponent(new Position.Snapshot(new Coordinates(worldPos.x, 0, worldPos.y)), WorkerUtils.UnityGameLogic);
            template.AddComponent(
                new Metadata.Snapshot { EntityType = "ResourceNode" },
                WorkerUtils.UnityGameLogic);
            template.AddComponent(new Persistence.Snapshot(), WorkerUtils.UnityGameLogic);
            template.AddComponent(new ResourceNode.Snapshot(type, 100), WorkerUtils.UnityGameLogic);
            template.AddComponent(new GridPosition.Snapshot(position.ToGridCoordinate()), WorkerUtils.UnityGameLogic);

            template.SetReadAccess(WorkerUtils.AllWorkers);
            template.SetComponentWriteAccess(EntityAcl.ComponentId, WorkerUtils.UnityGameLogic);

            snapshot.AddEntity(template);
        }

        private static void AddPlayerSpawner(Snapshot snapshot)
        {
            var serverAttribute = UnityGameLogicConnector.WorkerType;

            var template = new EntityTemplate();
            template.AddComponent(new Position.Snapshot(), serverAttribute);
            template.AddComponent(
                new Metadata.Snapshot { EntityType = "PlayerCreator" },
                serverAttribute);
            template.AddComponent(new Persistence.Snapshot(), serverAttribute);
            template.AddComponent(new PlayerCreator.Snapshot(), serverAttribute);

            template.SetReadAccess(WorkerUtils.AllWorkers);
            template.SetComponentWriteAccess(EntityAcl.ComponentId, serverAttribute);

            snapshot.AddEntity(template);
        }

        private static void AddSpawnController(Snapshot snapshot)
        {
            var template = new EntityTemplate();
            template.AddComponent(new Position.Snapshot(), WorkerUtils.UnityGameLogic);
            template.AddComponent(
                new Metadata.Snapshot { EntityType = "SpawnController" },
                WorkerUtils.UnityGameLogic);
            template.AddComponent(new Persistence.Snapshot(), WorkerUtils.UnityGameLogic);
            template.AddComponent(new SpawnController.Snapshot(), WorkerUtils.UnityGameLogic);

            template.SetReadAccess(WorkerUtils.AllWorkers);
            template.SetComponentWriteAccess(EntityAcl.ComponentId, WorkerUtils.UnityGameLogic);

            snapshot.AddEntity(template);
        }
    }
}
