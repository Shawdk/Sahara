namespace Sahara
{
    internal static class Sahara
    {
        private static SaharaServer _server;

        public static void Initialize()
        {
            _server = new SaharaServer();
            _server.Load();
        }

        public static SaharaServer GetServer()
        {
            return _server;
        }
    }
}
