using UnityEngine;
using UnityEngine.EventSystems;

namespace BetaApartUranus
{
    /// <summary>
    /// Client-side logic for displaying and managing drones.
    /// </summary>
    public class ClientDrone : MonoBehaviour, IPointerDownHandler
    {
        #region IPointerDownHandler
        public void OnPointerDown(PointerEventData eventData)
        {
            ClientController.SelectDrone(this);
        }
        #endregion
    }
}
