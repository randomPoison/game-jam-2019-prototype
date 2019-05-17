using Improbable.Gdk.Core;
using Improbable.Gdk.Mobile;
#if UNITY_IOS
using Improbable.Gdk.Mobile.iOS;
#endif
using System;
using Improbable.Gdk.PlayerLifecycle;
using UnityEngine;

namespace BetaApartUranus
{
    public class iOSClientWorkerConnector : MobileWorkerConnector
    {
        public const string WorkerType = "iOSClient";

        [SerializeField] private string ipAddress = null;
        [SerializeField] private bool shouldConnectLocally = false;

        private async void Start()
        {
            await Connect(WorkerType, new ForwardingDispatcher()).ConfigureAwait(false);
        }

        protected override ConnectionService GetConnectionService()
        {
            return shouldConnectLocally ? ConnectionService.Receptionist : ConnectionService.AlphaLocator;
        }

        protected override void HandleWorkerConnectionEstablished()
        {
            PlayerLifecycleHelper.AddClientSystems(Worker.World);
        }

        protected override string GetHostIp()
        {
            if (Application.platform != RuntimePlatform.IPhonePlayer)
            {
                throw new PlatformNotSupportedException(
                    $"{nameof(iOSClientWorkerConnector)} can only be used for the iOS platform. Please check your build settings.");
            }

            if (!string.IsNullOrEmpty(ipAddress))
            {
                return ipAddress;
            }

            return RuntimeConfigDefaults.ReceptionistHost;
        }
    }
}
