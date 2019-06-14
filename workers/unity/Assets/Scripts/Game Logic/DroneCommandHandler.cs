using Improbable.Gdk.Core;
using Improbable.Gdk.Core.Commands;
using Improbable.Gdk.Subscriptions;
using UnityEngine;

namespace BetaApartUranus
{
    public class DroneCommandHandler : MonoBehaviour
    {
        [Require]
        private DroneCommandReceiver _commandReceiver = null;

        //[Require]
        //private WorldCommandSender _worldSender = null;

        private void OnAddCommandRequest(Drone.AddCommand.ReceivedRequest request)
        {
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
