using Improbable;
using Improbable.Gdk.Core;
using Improbable.Gdk.PlayerLifecycle;
using UnityEngine;
using Snapshot = Improbable.Gdk.Core.Snapshot;

namespace BetaApartUranus.Editor
{
    internal static class SnapshotGenerator
    {
        public struct Arguments
        {
            public string OutputPath;
        }

        public static void Generate(Arguments arguments)
        {
            Debug.Log("Generating snapshot.");
            var snapshot = CreateSnapshot();

            Debug.Log($"Writing snapshot to: {arguments.OutputPath}");
            snapshot.WriteToFile(arguments.OutputPath);
        }

        private static Snapshot CreateSnapshot()
        {
            var snapshot = new Snapshot();

            AddPlayerSpawner(snapshot);
            AddSpawnController(snapshot);
            AddResourceNode(snapshot);

            return snapshot;
        }

        private static void AddResourceNode (Snapshot snapshot)
        {
            var template = new EntityTemplate();

            var pos = HexTools.HexUtils.GridToWorld(new HexTools.AxialCoordinate(5, 5));
            template.AddComponent(new Position.Snapshot(new Coordinates (pos.x, 0, pos.y)), WorkerUtils.UnityGameLogic);
            template.AddComponent(
                new Metadata.Snapshot { EntityType = "ResourceNode" },
                WorkerUtils.UnityGameLogic);
            template.AddComponent(new Persistence.Snapshot(), WorkerUtils.UnityGameLogic);
            template.AddComponent(new ResourceNode.Snapshot(), WorkerUtils.UnityGameLogic);
            template.AddComponent(new GridPosition.Snapshot(new GridCoordinate(5, 5)), WorkerUtils.UnityGameLogic);

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
