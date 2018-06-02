using MsorLi.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MsorLi.Utilities
{
    class LocationStorage
    {
        //---------------------------------
        // MEMBERS
        //---------------------------------

        private static List<Models.Location> _locations = null;

        //---------------------------------
        // FUNCTIONS
        //---------------------------------

        public static async Task<List<Models.Location>> GetLocations()
        {
            if (await Connection.IsServerReachableAndRunning())
            {
                if (_locations == null)
                {
                    await LoadLocations();
                }
                return _locations;
            }
            else
                throw new NoConnectionException();
        }

        private static async Task LoadLocations()
        {
            _locations = new List<Models.Location>();
            if (await Connection.IsServerReachableAndRunning())
                _locations = await AzureLocationService.DefaultManager.GetLocations();
            
            else
                throw new NoConnectionException();
        }
    }
}