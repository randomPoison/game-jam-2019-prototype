using System;
using Singleton;
using UnityEngine;
using HexTools;

namespace BetaApartUranus
{
    public class ClientController : MonoBehaviourSingleton<ClientController>
    {
        [SerializeField]
        private GameObject _cursorPrefab = null;

        private Camera _camera = null;
        private MainCanvasController _canvas = null;
        private GameObject _cursor = null;

        private ClientDrone _selectedDrone = null;
        private AxialCoordinate _cursorGridPosition;

        public event Action<ClientDrone> SelectedDroneChanged;

        public void SelectDrone(ClientDrone drone)
        {
            _selectedDrone = drone;
            _canvas.SetActiveDrone(drone);

            SelectedDroneChanged?.Invoke(drone);
        }

        #region Unity Lifecycle Methods
        protected override void Awake()
        {
            base.Awake();

            // Initialize references to objects in the scene.
            _camera = FindObjectOfType<Camera>();
            _canvas = FindObjectOfType<MainCanvasController>();
            _cursor = Instantiate(_cursorPrefab);
        }

        private void Start()
        {
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
        }

        private void OnGUI()
        {
            GUILayout.Label($"Cursor is at grid position {_cursorGridPosition}");
        }
        #endregion
    }
}
