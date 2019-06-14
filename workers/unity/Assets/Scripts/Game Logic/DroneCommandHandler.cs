﻿using System.Collections.Generic;
using Improbable.Gdk.Subscriptions;
using UnityEngine;

namespace BetaApartUranus
{
    public class DroneCommandHandler : MonoBehaviour
    {
        [Require]
        private DroneWriter _droneWriter = null;

        [Require]
        private DroneCommandReceiver _commandReceiver = null;

        private LinkedEntityComponent _linkedEntity;

        private void OnAddCommandRequest(Drone.AddCommand.ReceivedRequest request)
        {
            // Reject attempts to add commands to a drone not owned by the player.
            if (_droneWriter.Data.Owner != request.CallerWorkerId)
            {
                Debug.Log($"Rejecting add command request {request.EntityId} from {request.CallerWorkerId}");

                _commandReceiver.SendAddCommandFailure(
                    request.RequestId,
                    "Cannot add command to non-owned drone");
                return;
            }

            // TODO: Validate the received command.

            Debug.Log($"Handling add command request {request.EntityId} from {request.CallerWorkerId}");

            // Update the list of commands on the drone.
            var updatedQueue = new List<Command>(_droneWriter.Data.CommandQueue);
            updatedQueue.Add(request.Payload);
            _droneWriter.SendUpdate(new Drone.Update
            {
                CommandQueue = updatedQueue,
            });

            // Send the success response.
            _commandReceiver.SendAddCommandResponse(request.RequestId, new AddCommandResponse());
        }

        #region Unity Lifecycle Methods
        private void OnEnable()
        {
            _linkedEntity = GetComponent<LinkedEntityComponent>();

            _commandReceiver.OnAddCommandRequestReceived += OnAddCommandRequest;
        }

        private void Update()
        {
            // If the drone has a command it's actively working on, perform any work
            // related to that command. Otherwise, if there's a command in the queue,
            // make it the active command.
            if (_droneWriter.Data.ActiveCommand.HasValue)
            {
                var command = _droneWriter.Data.ActiveCommand.Value;
                switch (command.Type)
                {
                    case CommandType.MoveToPosition:

                        // TODO: Move towards the target position.

                        break;

                    default:
                        Debug.LogWarning($"Unable to perform unknown command {command.Type}, discarding active command");

                        // Discard the active command,
                        _droneWriter.SendUpdate(new Drone.Update
                        {
                            ActiveCommand = null,
                        });

                        break;
                }
            }
            else if (_droneWriter.Data.CommandQueue.Count > 0)
            {
                var active = _droneWriter.Data.CommandQueue[0];
                Debug.Log($"Making command {active.Type} active for drone {_linkedEntity.EntityId}");

                var commands = new List<Command>(_droneWriter.Data.CommandQueue);
                commands.RemoveAt(0);

                _droneWriter.SendUpdate(new Drone.Update
                {
                    ActiveCommand = active,
                    CommandQueue = commands,
                });
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawCube(transform.position, Vector3.one);
        }
        #endregion
    }
}
