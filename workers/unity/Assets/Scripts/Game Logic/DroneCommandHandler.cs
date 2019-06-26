using System.Collections.Generic;
using System.Linq;
using BetaApartUranus.DroneCommands;
using HexTools;
using Improbable;
using Improbable.Gdk.Core;
using Improbable.Gdk.Subscriptions;
using Improbable.Worker.CInterop;
using UnityEngine;
using Entity = Unity.Entities.Entity;

namespace BetaApartUranus
{
    public class DroneCommandHandler : MonoBehaviour
    {
        // Movement speed in units per second.
        //
        // TODO: Make this into a component for configurability.
        private const float MOVE_SPEED = 3f;

        /// <summary>
        /// Interval at which we send update data to SpatialOS. Value in seconds.
        /// </summary>
        private const float UPDATE_INTERVAL = 1f;

        [Require]
        private EntityId _entityId;

        [Require]
        private PositionWriter _positionWriter = null;

        [Require]
        private DroneWriter _droneWriter = null;

        [Require]
        private DroneCommandReceiver _commandReceiver = null;

        [Require]
        private PlayerInventoryWriter _inventoryWriter = null;

        private LinkedEntityComponent _linkedEntity;

        private Vector2 _position;
        private Command? _activeCommand = null;
        private float _timeToNextUpdate;
        float? resourceCollectionTimer = null;

        private PlayerInventory.Component _inventory;
        private List<Command> _commandQueue;

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
            _commandQueue.Add(request.Payload);

            // Send the success response.
            _commandReceiver.SendAddCommandResponse(request.RequestId, new AddCommandResponse());
        }

        #region Unity Lifecycle Methods
        private void OnEnable()
        {
            _linkedEntity = GetComponent<LinkedEntityComponent>();

            _commandReceiver.OnAddCommandRequestReceived += OnAddCommandRequest;

            var worldPosition = _positionWriter.Data.Coords.ToUnityVector();
            _position = new Vector2(worldPosition.x, worldPosition.z);
            _activeCommand = _droneWriter.Data.ActiveCommand;
            _commandQueue = new List<Command>(_droneWriter.Data.CommandQueue);
            _timeToNextUpdate = UPDATE_INTERVAL;
        }

        private void Update()
        {
            // If the drone has a command it's actively working on, perform any work
            // related to that command. Otherwise, if there's a command in the queue,
            // make it the active command.
            if (_activeCommand.HasValue)
            {
                var command = _activeCommand.Value;
                switch (command.Type)
                {
                    case CommandType.MoveToPosition:
                        var moveToPosition = JsonUtility.FromJson<MoveToPosition>(command.Data);

                        // Determine offset to the target position.
                        var targetWorldPos = HexUtils.GridToWorld(moveToPosition.Target);
                        var offset = targetWorldPos - _position;

                        // Determine the distance to the target and the direction.
                        var distance = offset.magnitude;
                        var direction = offset.normalized;

                        // If we're within a frame's distance from the target, snap to the
                        // target and end movement. Otherwise, move towards the target.
                        var maxMovement = Time.deltaTime * MOVE_SPEED;
                        if (distance > maxMovement)
                        {
                            // Update the world position of the drone.
                            _position = _position + maxMovement * direction;
                        }
                        else
                        {
                            // Snap to the target position and clear the active command.
                            _position = targetWorldPos;
                            _activeCommand = null;
                        }

                        break;

                    case CommandType.HarvestResourceNode:
                        var harvestResourceNode = JsonUtility.FromJson<HarvestResourceNode>(command.Data);

                        if (!resourceCollectionTimer.HasValue)
                        {
                            resourceCollectionTimer = 0;
                        }
                        else if (resourceCollectionTimer.Value < 1f)
                        {
                            resourceCollectionTimer += Time.deltaTime;
                        }
                        else
                        {
                            resourceCollectionTimer = null;

                            Entity targetEntity;
                            if (_linkedEntity.Worker.TryGetEntity(new EntityId(harvestResourceNode.Target), out targetEntity))
                            {
                                var entityManager = _linkedEntity.World.EntityManager;
                                var resourceNodeComponent = entityManager.GetComponentData<ResourceNode.Component>(targetEntity);

                                uint harvestedQuantity = System.Math.Min(1, resourceNodeComponent.Quantity);
                                resourceNodeComponent.Quantity -= harvestedQuantity;
                                entityManager.SetComponentData<ResourceNode.Component>(targetEntity, resourceNodeComponent);

                                switch (resourceNodeComponent.Type)
                                {
                                    case ResourceType.Coal:
                                        _inventory.Coal += harvestedQuantity;
                                        break;

                                    case ResourceType.Copper:
                                        _inventory.Copper += harvestedQuantity;
                                        break;
                                }

                                if (resourceNodeComponent.Quantity <= 0)
                                {
                                    // Clear the active command.
                                    _activeCommand = null;
                                }
                            }
                            else
                            {
                                Debug.Log($"Target resource node {harvestResourceNode.Target} doesn't exist, cancelling command");

                                // Clear the active command.
                                _activeCommand = null;
                            }
                        }

                        break;

                    default:
                        Debug.LogWarning($"Unable to perform unknown command {command.Type}, discarding active command");

                        // Discard the active command,
                        _activeCommand = null;

                        break;
                }
            }
            else if (_commandQueue.Count > 0)
            {
                _activeCommand = _commandQueue[0];
                _commandQueue.RemoveAt(0);

                Debug.Log($"Making command {_activeCommand} active for drone {_linkedEntity.EntityId}");
            }

            var worldPosition = new Vector3(_position.x, 0f, _position.y);
            transform.position = worldPosition + _linkedEntity.Worker.Origin;

            // Periodically send updates to components that are rate-limited by time.
            _timeToNextUpdate -= Time.deltaTime;
            if (_timeToNextUpdate < 0f || _droneWriter.Authority == Authority.AuthorityLossImminent)
            {
                // Check for updates to the position component.
                if (_timeToNextUpdate < 0f && !worldPosition.Equals(_positionWriter.Data.Coords))
                {
                    _positionWriter.SendUpdate(new Position.Update
                    {
                        Coords = new Coordinates(worldPosition.x, worldPosition.y, worldPosition.z),
                    });
                }

                // Check for updates to the inventory.
                if (_timeToNextUpdate < 0f && _inventory.IsDataDirty())
                {
                    _inventoryWriter.SendUpdate(new PlayerInventory.Update
                    {
                        Coal = _inventory.Coal,
                        Copper = _inventory.Copper,
                    });
                    _inventory.MarkDataClean();
                }

                // Track drone updates.
                var droneUpdate = new Drone.Update();
                var droneDirty = false;

                // TODO: Use regular == operator once SpatialOS generates the necessary code.
                if (!_activeCommand.Equals(_droneWriter.Data.ActiveCommand))
                {
                    droneUpdate.ActiveCommand = _activeCommand;
                    droneDirty = true;
                }

                if (!_commandQueue.SequenceEqual(_droneWriter.Data.CommandQueue))
                {
                    droneUpdate.CommandQueue = _commandQueue;
                    droneDirty = true;
                }

                if (droneDirty)
                {
                    _droneWriter.SendUpdate(droneUpdate);
                }

                // Reset the time to the next update.
                _timeToNextUpdate = UPDATE_INTERVAL;
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawCube(transform.position, Vector3.one);
        }
        #endregion
    }
}
