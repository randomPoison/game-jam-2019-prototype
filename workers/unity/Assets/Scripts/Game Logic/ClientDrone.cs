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
        [Tooltip("The material to use when the drone is owned by another player.")]
        private Material _otherMaterial = null;

        [SerializeField]
        [Tooltip("The material to use when the drone is selected.")]
        private Material _selectedMaterial = null;

        [Require]
        private DroneReader _droneReader = null;

        private MeshRenderer _display;
        private UnityClientConnector _connector;
        private ClientController _clientController;

        public bool IsOwned
        {
            // TODO: Is there a better way to check if the drone is owned by the
            // current worker? It seems odd to have a reference to the connector
            // object just to fetch the worker ID.
            get { return Owner == _connector.Worker.WorkerId; }
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
                _display.sharedMaterial = IsOwned ? _playerMaterial : _otherMaterial;
            }
        }

        #region Unity Lifecycle Methods
        private void Awake()
        {
            _display = GetComponentInChildren<MeshRenderer>();
            _connector = FindObjectOfType<UnityClientConnector>();
            _clientController = FindObjectOfType<ClientController>();
        }

        private void Start()
        {
            _clientController.SelectedDroneChanged += OnSelectedDroneChanged;

            if (IsOwned)
            {
                _display.sharedMaterial = _playerMaterial;
            }
        }

        private void OnDestroy()
        {
            _clientController.SelectedDroneChanged -= OnSelectedDroneChanged;
        }
        #endregion

        #region IPointerDownHandler
        public void OnPointerDown(PointerEventData eventData)
        {
            _clientController.SelectDrone(this);
        }
        #endregion
    }
}
