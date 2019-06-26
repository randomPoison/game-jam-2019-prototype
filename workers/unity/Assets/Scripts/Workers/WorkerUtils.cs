namespace BetaApartUranus
{
    public static class WorkerUtils
    {
        public const string UnityClient = "UnityClient";
        public const string MobileClient = "MobileClient";
        public const string UnityGameLogic = "UnityGameLogic";

        public static readonly string[] AllWorkers = new string[]
        {
            UnityClient,
            MobileClient,
            UnityGameLogic,
        };
    }
}
