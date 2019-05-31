using Improbable.Gdk.Subscriptions;
using UnityEngine;

namespace BetaApartUranus
{
    public class SpawnControllerCommandHandler : MonoBehaviour
    {
        [Require]
        private SpawnControllerCommandReceiver _commandReceiver = null;

        private void OnEnable()
        {
            _commandReceiver.OnSpawnDroneRequestReceived += OnSpawnDroneRequest;
        }

        private void OnSpawnDroneRequest(SpawnController.SpawnDrone.ReceivedRequest request)
        {
            Debug.Log("Handling spawn drone request!");
            _commandReceiver.SendSpawnDroneResponse(request.RequestId, new SpawnDroneResponse());
        }
    }
}
