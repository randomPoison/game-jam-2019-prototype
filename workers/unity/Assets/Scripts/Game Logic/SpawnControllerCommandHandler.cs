using Improbable.Gdk.Core;
using Improbable.Gdk.Core.Commands;
using Improbable.Gdk.Subscriptions;
using UnityEngine;

namespace BetaApartUranus
{
    public class SpawnControllerCommandHandler : MonoBehaviour
    {
        [Require]
        private SpawnControllerCommandReceiver _commandReceiver = null;

        [Require]
        private WorldCommandSender _worldSender = null;

        private void OnEnable()
        {
            _commandReceiver.OnSpawnDroneRequestReceived += OnSpawnDroneRequest;
        }

        private void OnSpawnDroneRequest(SpawnController.SpawnDrone.ReceivedRequest request)
        {
            Debug.Log($"Handling spawn drone request {request.EntityId} from {request.CallerWorkerId}");

            // Generate a random starting point for the drone.
            var position = new GridCoordinate(
                Random.Range(-20, 20),
                Random.Range(-20, 20));

            // Create an entity for the drone, and once it's been made, respond to the player.
            //
            // TODO: Can we do this with async/await, rather than callbacks?
            _worldSender.SendCreateEntityCommand(
                new WorldCommands.CreateEntity.Request(EntityTemplates.Drone(position, request.CallerWorkerId)),
                response =>
                {
                    _commandReceiver.SendSpawnDroneResponse(request.RequestId, new SpawnDroneResponse());
                });
        }
    }
}
