using Microsoft.WindowsAzure.MobileServices;
using MsorLi.Utilities;
using System.Threading.Tasks;

namespace MsorLi.Services
{
    public class AzureService<TData>
    {
        //---------------------------------
        // MEMBERS
        //---------------------------------

        protected MobileServiceClient _client;
        protected IMobileServiceTable<TData> _table;

        //---------------------------------
        // FUNCTIONS
        //---------------------------------

        // C-tor
        public AzureService()
        {
            //connect to the Azure service
            _client = new MobileServiceClient(Constants.ApplicationURL);

            _table = _client.GetTable<TData>();
        }

        public MobileServiceClient CurrentClient
        {
            get { return _client; }
        }

        public async Task UploadToServer(TData newRow, string id)
        {
            if (id == null)
            {
                await _table.InsertAsync(newRow);
            }
            else
            {
                await _table.UpdateAsync(newRow);
            }
        }
    }
}