using UnityEngine;
using UnityEngine.EventSystems;

namespace BetaApartUranus
{
    public class ClientDrone : MonoBehaviour, IPointerDownHandler
    {
        #region IPointerDownHandler
        public void OnPointerDown(PointerEventData eventData)
        {
            Debug.Log("I done did get clicked on!", this);
        }
        #endregion
    }
}
