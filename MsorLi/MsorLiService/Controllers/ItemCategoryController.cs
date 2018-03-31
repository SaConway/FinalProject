using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.OData;
using Microsoft.Azure.Mobile.Server;
using MsorLiService.DataObjects;
using MsorLiService.Models;

namespace MsorLiService.Controllers
{
    public class ItemCategoryController : TableController<ItemCategory>
    {
        protected override void Initialize(HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext);
            MsorLiContext context = new MsorLiContext();

            DomainManager = new EntityDomainManager<ItemCategory>(context, Request);
        }

        // GET tables/ItemCategory
        public IOrderedQueryable<ItemCategory> GetAllItemCategories()
        {
            return Query().OrderBy(category => category.Name);
        }

        // GET tables/ItemCategory
        public SingleResult<ItemCategory> GetItemCategory(string id)
        {
            return Lookup(id);
        }

        // PATCH tables/ItemCategory
        public Task<ItemCategory> PatchItemCategory(string id, Delta<ItemCategory> patch)
        {
            return UpdateAsync(id, patch);
        }

        // POST tables/ItemCategory
        //insert into the data base
        public async Task<IHttpActionResult> PostItemCategory(ItemCategory itemCategory)
        {
            ItemCategory current = await InsertAsync(itemCategory);
            return CreatedAtRoute("Tables", new { id = current.Id }, current);
        }

        // DELETE tables/ItemCategory
        public Task DeleteItemCategory(string id)
        {
            return DeleteAsync(id);
        }
    }
}