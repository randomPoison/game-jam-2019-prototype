using Improbable.Gdk.Core;
using Improbable.Gdk.Core.Commands;
using Improbable.Gdk.Subscriptions;
using UnityEngine;

namespace BetaApartUranus
{
    public class ResourceNodeHandler : MonoBehaviour
    {
        [Require]
        private EntityId _entityId = new EntityId();

        [Require]
        private ResourceNodeReader _nodeReader = null;

        [Require]
        private WorldCommandSender _worldSender = null;

        private void LateUpdate ()
        {
            if (_nodeReader.Data.Quantity <= 0)
            {
                // Delete the resource node if there isn't any resources left
                _worldSender.SendDeleteEntityCommand(
                    new WorldCommands.DeleteEntity.Request(_entityId)
                );
            }
        }
    }
}
