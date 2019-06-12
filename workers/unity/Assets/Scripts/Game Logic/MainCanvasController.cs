using TMPro;
using UnityEngine;

namespace BetaApartUranus
{
    [DisallowMultipleComponent]
    public class MainCanvasController : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI _droneText = null;

        public void SetActiveDrone(ClientDrone drone)
        {
            _droneText.gameObject.SetActive(true);
            _droneText.text = drone.name;
        }
    }
}
