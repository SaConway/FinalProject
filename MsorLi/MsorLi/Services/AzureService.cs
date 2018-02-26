using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.MobileServices;
using MsorLi.Services;
using Xamarin.Forms;
using MsorLi.Models;
using MsorLi.Utilities;


namespace MsorLi.Services
{
    public class AzureService
    {
        //---------------------------------
        // MEMBERS
        //---------------------------------

        static AzureService _defaultInstance = new AzureService();
        MobileServiceClient _client;

#if OFFLINE_SYNC_ENABLED
            IMobileServiceSyncTable<Item> _Table;
#else
        IMobileServiceTable<Item> _itemsTable;
#endif

        const string offlineDbPath = @"localstore.db";


        //---------------------------------
        // FUNCTIONS
        //---------------------------------

        //constractor function
        private AzureService()
        {
            //connect to the Azure service
            this._client = new MobileServiceClient(Constants.ApplicationURL);

#if OFFLINE_SYNC_ENABLED
            var store = new MobileServiceSQLiteStore(offlineDbPath);
            store.DefineTable<Item>();

            //Initializes the SyncContext using the default IMobileServiceSyncHandler.
            this.client.SyncContext.InitializeAsync(store);

            this._Table = client.GetSyncTable<Item>();
#else
           this._itemsTable = _client.GetTable<Item>();
#endif
        }

        public static AzureService DefaultManager
        {
            get
            {
                return _defaultInstance;
            }
            private set
            {
                _defaultInstance = value;
            }
        }

        public MobileServiceClient CurrentClient
        {
            get { return _client; }
        }

        public bool IsOfflineEnabled
        {
            get { return _itemsTable is Microsoft.WindowsAzure.MobileServices.Sync.IMobileServiceSyncTable<Item>; }
        }

        public async Task<ObservableCollection<Item>> GetItemsAsync(bool syncItems = false)
        {
            try
            {
#if OFFLINE_SYNC_ENABLED
                if (syncItems)
                {
                    await this.SyncAsync();
                }
#endif
                IEnumerable<Item> items = await _itemsTable
                    .ToEnumerableAsync();

                return new ObservableCollection<Item>(items);
            }
            catch (MobileServiceInvalidOperationException msioe)
            {
                Debug.WriteLine(@"Invalid sync operation: {0}", msioe.Message);
            }
            catch (Exception e)
            {
                Debug.WriteLine(@"Sync error: {0}", e.Message);
            }
            return null;
        }

        public async Task UploadItemToServer(Item item)
        {
            if (item.Id == null)
            {
                await _itemsTable.InsertAsync(item);
            }
            else
            {
                await _itemsTable.UpdateAsync(item);
            }
        }

#if OFFLINE_SYNC_ENABLED
        public async Task SyncAsync()
        {
            ReadOnlyCollection<MobileServiceTableOperationError> syncErrors = null;

            try
            {
                await this.client.SyncContext.PushAsync();

                await this._Table.PullAsync(
                    //The first parameter is a query name that is used internally by the client SDK to implement incremental sync.
                    //Use a different query name for each unique query in your program
                    "allTodoItems",
                    this._Table.CreateQuery());
            }
            catch (MobileServicePushFailedException exc)
            {
                if (exc.PushResult != null)
                {
                    syncErrors = exc.PushResult.Errors;
                }
            }

            // Simple error/conflict handling. A real application would handle the various errors like network conditions,
            // server conflicts and others via the IMobileServiceSyncHandler.
            if (syncErrors != null)
            {
                foreach (var error in syncErrors)
                {
                    if (error.OperationKind == MobileServiceTableOperationKind.Update && error.Result != null)
                    {
                        //Update failed, reverting to server's copy.
                        await error.CancelAndUpdateItemAsync(error.Result);
                    }
                    else
                    {
                        // Discard local change.
                        await error.CancelAndDiscardItemAsync();
                    }

                    Debug.WriteLine(@"Error executing sync operation. Item: {0} ({1}). Operation discarded.", error.TableName, error.Item["id"]);
                }
            }
        }
#endif

    }
}