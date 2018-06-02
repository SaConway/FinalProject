using System;
using System.Threading.Tasks;
using Plugin.Connectivity;

namespace MsorLi.Utilities
{
    public static class Connection
    {

        public static async Task<bool> IsServerReachableAndRunning()
        {

            var connectivity = CrossConnectivity.Current;
            if (!connectivity.IsConnected)
                return false;

            var reachable = await connectivity.IsRemoteReachable(Constants.ApplicationURL);

            return reachable;
        }
    }
}
