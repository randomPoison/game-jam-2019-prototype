using Improbable.Gdk.Core;
using Improbable.Gdk.Subscriptions;
using Improbable.Worker.CInterop;
using UnityEngine;

namespace BetaApartUranus
{
    public class AuthoritativePlayer : MonoBehaviour
    {
        [Require]
        private SpawnControllerCommandSender _commandSender = null;

        private void Start()
        {
            if (_commandSender != null)
            {
                _commandSender.SendSpawnDroneCommand(
                    new EntityId(2), // TODO: Don't hard-code the entity ID.
                    new SpawnDroneRequest(),
                    response =>
                    {
                        if (response.StatusCode == StatusCode.Success)
                        {
                            Debug.Log("Successfully spawned initial drone for player");
                        }
                        else
                        {
                            Debug.Log($"Spawn drone request failed, status: {response.StatusCode}, message: {response.Message}");
                        }
                    });
            }
            else
            {
                Debug.LogWarning("_commandSender is null, unable to request drone spawn");
            }
        }
    }
}
