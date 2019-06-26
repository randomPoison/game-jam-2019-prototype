using Improbable;
using Improbable.Gdk.Core;
using Improbable.Gdk.GameObjectCreation;
using Improbable.Gdk.PlayerLifecycle;
using Improbable.Gdk.QueryBasedInterest;

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
            Worker.World.GetOrCreateSystem<MetricSendSystem>();
            PlayerLifecycleHelper.AddServerSystems(Worker.World);
            GameObjectCreationHelper.EnableStandardGameObjectCreation(Worker.World);
        }

        private static EntityTemplate CreatePlayerEntityTemplate(string workerId, byte[] serializedArguments)
        {
            var clientAttribute = EntityTemplate.GetWorkerAccessAttribute(workerId);

            var resourceQuery = InterestQuery.Query(Constraint.RelativeCylinder(50.0));
            var interestTemplate = InterestTemplate.Create();
            interestTemplate.AddQueries<Position.Component>(resourceQuery);

            var template = new EntityTemplate();

            // TODO: Don't give the client authority over the Position component.
            template.AddComponent(new Position.Snapshot(), clientAttribute);
            template.AddComponent(new Metadata.Snapshot("Player"), WorkerType);
            template.AddComponent(interestTemplate.ToSnapshot(), WorkerType);
            PlayerLifecycleHelper.AddPlayerLifecycleComponents(template, workerId, WorkerType);

            template.SetReadAccess(WorkerUtils.AllWorkers);
            template.SetComponentWriteAccess(EntityAcl.ComponentId, WorkerType);

            return template;
        }
    }
}
