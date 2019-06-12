using UnityEngine;
using UnityEngine.EventSystems;

namespace BetaApartUranus
{
    /// <summary>
    /// Client-side logic for displaying and managing drones.
    /// </summary>
    public class ClientDrone : MonoBehaviour, IPointerDownHandler
    {
        private ClientController _clientController;

        #region Unity Lifecycle Methods
        private void Awake()
        {
            _clientController = FindObjectOfType<ClientController>();
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
