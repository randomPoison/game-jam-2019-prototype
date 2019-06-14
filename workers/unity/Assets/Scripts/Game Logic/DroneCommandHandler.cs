using Improbable.Gdk.Core;
using Improbable.Gdk.Core.Commands;
using Improbable.Gdk.Subscriptions;
using UnityEngine;

namespace BetaApartUranus
{
    public class DroneCommandHandler : MonoBehaviour
    {
        [Require]
        private DroneReader _droneReader = null;

        [Require]
        private DroneCommandReceiver _commandReceiver = null;

        private void OnAddCommandRequest(Drone.AddCommand.ReceivedRequest request)
        {
            // Reject attempts to add commands to a drone not owned by the player.
            if (_droneReader.Data.Owner != request.CallerWorkerId)
            {
                Debug.Log($"Rejecting add command request {request.EntityId} from {request.CallerWorkerId}");

                _commandReceiver.SendAddCommandFailure(
                    request.RequestId,
                    "Cannot add command to non-owned drone");
                return;
            }

            Debug.Log($"Handling add command request {request.EntityId} from {request.CallerWorkerId}");
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
