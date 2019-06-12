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

            if (drone.IsOwned)
            {
                _droneText.text = $"Your drone";
            }
            else
            {
                _droneText.text = $"Drone belonging to {drone.Owner}";
            }
        }
    }
}
