using Improbable.Gdk.Core;
using Improbable.Gdk.PlayerLifecycle;

namespace BetaApartUranus
{
    public class UnityGameLogicConnector : DefaultWorkerConnector
    {
        public const string WorkerType = WorkerUtils.UnityGameLogic;

        private async void Start()
        {
            PlayerLifecycleConfig.CreatePlayerEntityTemplate = CreatePlayerEntityTemplate;
            await Connect(WorkerType, new ForwardingDispatcher()).ConfigureAwait(false);
        }

        protected override void HandleWorkerConnectionEstablished()
        {
            Worker.World.GetOrCreateManager<MetricSendSystem>();
            PlayerLifecycleHelper.AddServerSystems(Worker.World);
        }

        private static EntityTemplate CreatePlayerEntityTemplate(string workerId, byte[] serializedArguments)
        {
            return EntityTemplates.Drone(new GridCoordinate(), workerId);
        }
    }
}
