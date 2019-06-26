using Improbable;
using Improbable.Gdk.Core;
using Improbable.Gdk.Subscriptions;
using UnityEngine;
using UnityEngine.EventSystems;

namespace BetaApartUranus
{
    /// <summary>
    /// Client-side logic for displaying and managing drones.
    /// </summary>
    public class ClientDrone : MonoBehaviour, IPointerDownHandler
    {
        [SerializeField]
        [Tooltip("The material to use when the drone is owned by the current player.")]
        private Material _playerMaterial = null;

        [SerializeField]
        [Tooltip("The material to use when the drone is selected.")]
        private Material _selectedMaterial = null;

        [Require]
        private EntityId _entityId = new EntityId();

        [Require]
        private DroneReader _droneReader = null;

        [Require]
        private PositionReader _positionReader = null;

        private MeshRenderer _display;

        // Global state objects.
        private LinkedEntityComponent _linkedEntity;
        private AuthoritativePlayer _player;

        // State data.
        private Material _defaultMaterial = null;

        public EntityId EntityId
        {
            get { return _entityId; }
        }

        public Drone.Component Data
        {
            get { return _droneReader.Data; }
        }

        public bool IsOwned
        {
            // TODO: Is there a better way to check if the drone is owned by the
            //       current worker? It seems odd to have a reference to the connector
            //       object just to fetch the worker ID.
            get { return Owner == _linkedEntity.Worker.Connection.GetWorkerId(); }
        }

        public string Owner
        {
            get { return _droneReader.Data.Owner; }
        }

        private void OnSelectedDroneChanged(ClientDrone selected)
        {
            if (this == selected)
            {
                _display.sharedMaterial = _selectedMaterial;
            }
            else
            {
                _display.sharedMaterial = IsOwned ? _playerMaterial : _defaultMaterial;
            }
        }

        #region Unity Lifecycle Methods
        private void OnEnable()
        {
            _display = GetComponentInChildren<MeshRenderer>();
            _linkedEntity = GetComponent<LinkedEntityComponent>();
            _player = FindObjectOfType<AuthoritativePlayer>();

            _player.SelectedDroneChanged += OnSelectedDroneChanged;

            // Setup initial material.
            _defaultMaterial = _display.sharedMaterial;
            if (IsOwned)
            {
                _display.sharedMaterial = _playerMaterial;
            }
        }

        private void Update()
        {
            transform.position = _positionReader.Data.Coords.ToUnityVector() + _linkedEntity.Worker.Origin;
        }

        private void OnDestroy()
        {
            _player.SelectedDroneChanged -= OnSelectedDroneChanged;
        }
        #endregion

        #region IPointerDownHandler
        public void OnPointerDown(PointerEventData eventData)
        {
            _player.SelectDrone(this);
        }
        #endregion
    }
}
