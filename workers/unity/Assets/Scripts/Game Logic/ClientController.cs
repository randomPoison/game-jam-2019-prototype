using Singleton;

namespace BetaApartUranus
{
    public class ClientController : MonoBehaviourSingleton<ClientController>
    {
        private ClientDrone _selectedDrone = null;

        public static void SelectDrone(ClientDrone drone)
        {
            Instance._selectedDrone = drone;
        }
    }
}
