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
            if (_locations == null)
            {
                await LoadLocations();
            }
            return _locations;
        }

        private static async Task LoadLocations()
        {
            _locations = new List<Models.Location>();
            _locations = await AzureLocationService.DefaultManager.GetLocations();
        }
    }
}