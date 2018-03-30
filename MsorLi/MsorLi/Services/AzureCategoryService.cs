using MsorLi.Models;
using System.Threading.Tasks;

namespace MsorLi.Services
{
    public class AzureCategoryService : AzureService<ItemCategory>
    {
        //---------------------------------
        // MEMBERS
        //---------------------------------

        static AzureCategoryService _defaultInstance = new AzureCategoryService();

        public static AzureCategoryService DefaultManager
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

        //---------------------------------
        // FUNCTIONS
        //---------------------------------

        async public Task AddAllCategories()
        {
            //await UploadToServer(new ItemCategory { Name= "מוצרי חשמל" }, null);
            await _table.InsertAsync(new ItemCategory { Name = "מוצרי חשמל" });
        }
    }
}