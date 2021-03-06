﻿using Improbable.Gdk.Core;
using Improbable.Gdk.GameObjectCreation;
using Improbable.Gdk.Mobile;
using Improbable.Gdk.PlayerLifecycle;
using Improbable.Worker.CInterop;
using UnityEngine;

namespace BetaApartUranus
{
    public class MobileClientWorkerConnector : DefaultMobileWorkerConnector
    {
        public const string WorkerType = "MobileClient";

        [Header("Player Prefabs")]

        [SerializeField]
        private GameObject _authPlayerPrefab = null;

        [SerializeField]
        private GameObject _nonAuthPlayerPrefab = null;

        private async void Start()
        {
            await Connect(WorkerType, new ForwardingDispatcher()).ConfigureAwait(false);
        }

        protected override void HandleWorkerConnectionEstablished()
        {
            PlayerLifecycleHelper.AddClientSystems(Worker.World);
            PlayerLifecycleConfig.MaxPlayerCreationRetries = 0;

            var fallback = new GameObjectCreatorFromMetadata(
                Worker.WorkerType,
                Worker.Origin,
                Worker.LogDispatcher);

            var entityPipeline = new EntityPipeline(
                Worker,
                _authPlayerPrefab,
                _nonAuthPlayerPrefab,
                fallback);

            // Set the Worker gameObject to the ClientWorker so it can access PlayerCreater reader/writers.
            GameObjectCreationHelper.EnableStandardGameObjectCreation(
                Worker.World,
                entityPipeline,
                gameObject);
        }

        protected override string SelectDeploymentName(DeploymentList deployments)
        {
            return deployments.Deployments[0].DeploymentName;
        }
    }
}
