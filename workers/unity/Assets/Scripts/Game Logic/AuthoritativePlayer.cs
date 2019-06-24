using System;
using System.Linq;
using BetaApartUranus.DroneCommands;
using HexTools;
using Improbable.Gdk.Core;
using Improbable.Gdk.Subscriptions;
using Improbable.Worker.CInterop;
using Improbable.Worker.CInterop.Query;
using UnityEngine;

namespace BetaApartUranus
{
    public class AuthoritativePlayer : MonoBehaviour
    {
        [SerializeField]
        private GameObject _cursorPrefab = null;

        private Camera _camera = null;
        private MainCanvasController _canvas = null;
        private GameObject _cursor = null;

        private ClientDrone _selectedDrone = null;
        private AxialCoordinate _cursorGridPosition;

        [Require]
        private DroneCommandSender _droneSender = null;

        [Require]
        private SpawnControllerCommandSender _commandSender = null;

        private readonly EntityQuery _query = new EntityQuery()
        {
            Constraint = new ComponentConstraint(SpawnController.ComponentId),
            ResultType = new SnapshotResultType(),
        };

        [Require]
        private WorldCommandSender _worldSender = null;

        public event Action<ClientDrone> SelectedDroneChanged;

        private EntityId? _spawnControllerId = null;

        public void SelectDrone(ClientDrone drone)
        {
            _selectedDrone = drone;
            _canvas.SetActiveDrone(drone);

            SelectedDroneChanged?.Invoke(drone);
        }

        #region Unity Lifecycle Methods
        private void OnEnable()
        {
            // Initialize references to objects in the scene.
            _camera = FindObjectOfType<Camera>();
            _canvas = FindObjectOfType<MainCanvasController>();
            _cursor = Instantiate(_cursorPrefab);

            _worldSender.SendEntityQueryCommand(
                new Improbable.Gdk.Core.Commands.WorldCommands.EntityQuery.Request(_query), 
                response =>
                {
                    if (response.StatusCode == StatusCode.Success)
                    {
                        _spawnControllerId = response.Result.Keys.First();

                        _commandSender.SendSpawnDroneCommand(
                            _spawnControllerId.Value,
                            new SpawnDroneRequest(),
                            spawnResponse =>
                            {
                                if (spawnResponse.StatusCode == StatusCode.Success)
                                {
                                    Debug.Log("Successfully spawned initial drone for player");
                                }
                                else
                                {
                                    Debug.Log($"Spawn drone request failed, status: {spawnResponse.StatusCode}, message: {spawnResponse.Message}");
                                }
                            });
                    }
                });
        }

        private void Update()
        {
            // TODO: Use a better source of pointer input than Input.mousePosition.
            var viewportPosition = Input.mousePosition;
            viewportPosition.z = _camera.transform.position.y;
            var worldPosition = _camera.ScreenToWorldPoint(viewportPosition);

            // Snap the cursor display to the cell the mouse is over.
            _cursorGridPosition = HexUtils.WorldToGrid(new Vector2(worldPosition.x, worldPosition.z));
            worldPosition = HexUtils.GridToWorld(_cursorGridPosition);
            _cursor.transform.position = new Vector3(worldPosition.x, 0f, worldPosition.y);

            // TODO: Use a better source of input than Input. It would be nice to
            // use the EventSystem, but it's not clear how use it when clicking on
            // empty space.
            if (Input.GetMouseButtonDown(0) && _selectedDrone != null)
            {
                var command = new MoveToPosition
                {
                    Target = _cursorGridPosition,
                };

                _droneSender.SendAddCommandCommand(
                    _selectedDrone.EntityId,
                    new Command(CommandType.MoveToPosition, JsonUtility.ToJson(command)),
                    response =>
                    {
                        if (response.StatusCode != StatusCode.Success)
                        {
                            Debug.Log("Failed to add drone command");
                            return;
                        }

                        Debug.Log("Added drone command successfully!");
                    });
            }
        }

        private void OnGUI()
        {
            GUILayout.Label($"Cursor is at grid position {_cursorGridPosition}");
        }
        #endregion
    }
}
