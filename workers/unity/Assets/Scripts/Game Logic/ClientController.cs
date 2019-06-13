using System;
using Singleton;
using UnityEngine;

namespace BetaApartUranus
{
    public class ClientController : MonoBehaviourSingleton<ClientController>
    {
        [SerializeField]
        private GameObject _cursorPrefab = null;

        private Camera _camera = null;
        private MainCanvasController _canvas = null;
        private GameObject _cursor = null;
        private Grid _grid = null;

        private ClientDrone _selectedDrone = null;

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
            _grid = FindObjectOfType<Grid>();
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
            var gridPosition = _grid.WorldToCell(worldPosition);
            _cursor.transform.position = _grid.CellToWorld(gridPosition);
        }
        #endregion
    }
}
