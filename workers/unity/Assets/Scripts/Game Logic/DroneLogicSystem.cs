using BetaApartUranus.DroneCommands;
using Improbable;
using Unity.Entities;
using UnityEngine;

namespace BetaApartUranus
{
    /// <summary>
    /// Updates the active command on the drone when there is not an active command and
    /// there are commands in the queue.
    /// </summary>
    public class DroneLogicSystem : ComponentSystem
    {
        private EntityQuery _query;

        protected override void OnCreate()
        {
            base.OnCreate();

            _query = GetEntityQuery(
                ComponentType.ReadWrite<Position.Component>(),
                ComponentType.ReadWrite<Drone.Component>());
        }

        protected override void OnUpdate()
        {
            Entities.With(_query).ForEach((ref Position.Component position, ref Drone.Component drone) =>
            {
                if (drone.ActiveCommand.HasValue || drone.CommandQueue.Count == 0)
                {
                    return;
                }

                // Move the first command in the queue to be active.
                var command = drone.CommandQueue[0];
                drone.CommandQueue.RemoveAt(0);
                drone.ActiveCommand = command;

                // Deserialize the command and add the appropriate component type to
                // the entity.
                switch (command.Type)
                {
                    case CommandType.MoveToPosition:
                        var moveToPosition = JsonUtility.FromJson<MoveToPosition>(command.Data);

                        // TODO: Add a component for the MoveToPosition command.

                        break;

                    // If the active command is somehow invalid (i.e. an invalid enum
                    // variant), discard it to prevent the system from getting stuck.
                    default:
                        Debug.LogWarning(
                            $"Unable to perform unknown command {command.Type}, " +
                            $"discarding active command");
                        drone.ActiveCommand = null;
                        break;
                }
            });
        }
    }
}
