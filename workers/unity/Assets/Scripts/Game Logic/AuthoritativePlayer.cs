using System;
using HexTools;
using Improbable.Gdk.Core;
using Improbable.Gdk.Subscriptions;
using Improbable.Worker.CInterop;
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

        public event Action<ClientDrone> SelectedDroneChanged;

        public void SelectDrone(ClientDrone drone)
        {
            _selectedDrone = drone;
            _canvas.SetActiveDrone(drone);

            SelectedDroneChanged?.Invoke(drone);
        }

        #region Unity Lifecycle Methods
        private void Awake()
        {
            // Initialize references to objects in the scene.
            _camera = FindObjectOfType<Camera>();
            _canvas = FindObjectOfType<MainCanvasController>();
            _cursor = Instantiate(_cursorPrefab);
        }

        private void Start()
        {
            if (_commandSender != null)
            {
                _commandSender.SendSpawnDroneCommand(
                    new EntityId(2), // TODO: Don't hard-code the entity ID.
                    new SpawnDroneRequest(),
                    response =>
                    {
                        if (response.StatusCode == StatusCode.Success)
                        {
                            Debug.Log("Successfully spawned initial drone for player");
                        }
                        else
                        {
                            Debug.Log($"Spawn drone request failed, status: {response.StatusCode}, message: {response.Message}");
                        }
                    });
            }
            else
            {
                Debug.LogWarning("_commandSender is null, unable to request drone spawn");
            }
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
                _droneSender.SendAddCommandCommand(
                    _selectedDrone.LinkedEntity.EntityId,
                    new AddCommandRequest(),
                    response =>
                    {
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
