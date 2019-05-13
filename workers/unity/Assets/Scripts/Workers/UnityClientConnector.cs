using Improbable.Gdk.Core;
using Improbable.Gdk.GameObjectCreation;
using Improbable.Gdk.PlayerLifecycle;
using Improbable.Worker.CInterop;

namespace BetaApartUranus
{
    public class UnityClientConnector : DefaultWorkerConnector
    {
        public const string WorkerType = WorkerUtils.UnityClient;
        
        private async void Start()
        {
            await Connect(WorkerType, new ForwardingDispatcher()).ConfigureAwait(false);
        }

        protected override void HandleWorkerConnectionEstablished()
        {
            PlayerLifecycleHelper.AddClientSystems(Worker.World);
            GameObjectCreationHelper.EnableStandardGameObjectCreation(Worker.World);
        }

        protected override string SelectDeploymentName(DeploymentList deployments)
        {
            return deployments.Deployments[0].DeploymentName;
        }
    }
}
