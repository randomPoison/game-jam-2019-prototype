using Improbable.Gdk.Subscriptions;
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
                _commandSender.SendSpawnDroneCommand(new SpawnController.SpawnDrone.Request(), response =>
                {
                    Debug.Log("Got a response!");
                });
            }
            else
            {
                Debug.LogWarning("_commandSender is null, unable to request drone spawn");
            }
        }
    }
}
