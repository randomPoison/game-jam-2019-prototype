namespace BetaApartUranus
{
    public static class WorkerUtils
    {
        public const string UnityClient = "UnityClient";
        public const string UnityGameLogic = "UnityGameLogic";

        public static readonly string[] AllWorkers = new string[]
        {
            UnityClient,
            UnityGameLogic
        };
    }
}
