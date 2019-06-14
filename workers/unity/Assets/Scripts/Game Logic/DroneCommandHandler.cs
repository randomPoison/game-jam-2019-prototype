using System.Collections.Generic;
using Improbable.Gdk.Core;
using Improbable.Gdk.Subscriptions;
using UnityEngine;

namespace BetaApartUranus
{
    public class DroneCommandHandler : MonoBehaviour
    {
        [Require]
        private DroneWriter _droneWriter = null;

        [Require]
        private DroneCommandReceiver _commandReceiver = null;

        private void OnAddCommandRequest(Drone.AddCommand.ReceivedRequest request)
        {
            // Reject attempts to add commands to a drone not owned by the player.
            if (_droneWriter.Data.Owner != request.CallerWorkerId)
            {
                Debug.Log($"Rejecting add command request {request.EntityId} from {request.CallerWorkerId}");

                _commandReceiver.SendAddCommandFailure(
                    request.RequestId,
                    "Cannot add command to non-owned drone");
                return;
            }

            // TODO: Validate the received command.

            Debug.Log($"Handling add command request {request.EntityId} from {request.CallerWorkerId}");

            // Update the list of commands on the drone.
            var updatedQueue = new List<Command>(_droneWriter.Data.CommandQueue);
            updatedQueue.Add(request.Payload);
            _droneWriter.SendUpdate(new Drone.Update
            {
                CommandQueue = new Option<List<Command>>(updatedQueue),
            });

            // Send the success response.
            _commandReceiver.SendAddCommandResponse(request.RequestId, new AddCommandResponse());
        }

        #region Unity Lifecycle Methods
        private void OnEnable()
        {
            _commandReceiver.OnAddCommandRequestReceived += OnAddCommandRequest;
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawCube(transform.position, Vector3.one);
        }
        #endregion
    }
}
