using System.Collections.Generic;
using BetaApartUranus.DroneCommands;
using HexTools;
using Improbable;
using Improbable.Gdk.Subscriptions;
using UnityEngine;

namespace BetaApartUranus
{
    public class DroneCommandHandler : MonoBehaviour
    {
        // Movement speed in units per second.
        //
        // TODO: Make this into a component for configurability.
        private const float MOVE_SPEED = 1f;

        [Require]
        private PositionWriter _positionWriter = null;

        [Require]
        private DroneWriter _droneWriter = null;

        [Require]
        private DroneCommandReceiver _commandReceiver = null;

        private UnityGameLogicConnector _connector;
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
            _connector = FindObjectOfType<UnityGameLogicConnector>();

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
                        var moveToPosition = JsonUtility.FromJson<MoveToPosition>(command.Data);

                        // Determine offset to the target position.
                        var position = _positionWriter.Data.Coords.ToUnityVector();
                        var targetWorldPos = HexUtils.GridToWorld(moveToPosition.Target);
                        var offset = new Vector3(targetWorldPos.x, 0f, targetWorldPos.y) - position;

                        // Determine the distance to the target and the direction.
                        var distance = offset.magnitude;
                        var direction = offset.normalized;

                        // If we're within a frame's distance from the target, snap to the
                        // target and end movement. Otherwise, move towards the target.
                        var maxMovement = Time.deltaTime * MOVE_SPEED;
                        if (distance > maxMovement)
                        {
                            // Update the world position of the drone.
                            var newPosition = position + maxMovement * direction;
                            _positionWriter.SendUpdate(new Position.Update
                            {
                                Coords = new Coordinates(newPosition.x, 0f, newPosition.z),
                            });
                        }
                        else
                        {
                            // Snap the drone to its target position.
                            _positionWriter.SendUpdate(new Position.Update
                            {
                                Coords = new Coordinates(targetWorldPos.x, 0f, targetWorldPos.y),
                            });

                            // Clear the active command.
                            _droneWriter.SendUpdate(new Drone.Update
                            {
                                ActiveCommand = null,
                            });
                        }

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

            transform.position = _positionWriter.Data.Coords.ToUnityVector() + _connector.Worker.Origin;
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawCube(transform.position, Vector3.one);
        }
        #endregion
    }
}
