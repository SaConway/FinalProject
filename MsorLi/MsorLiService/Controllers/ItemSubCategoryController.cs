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
    public class ItemSubCategoryController : TableController<ItemSubCategory>
    {
        protected override void Initialize(HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext);
            MsorLiContext context = new MsorLiContext();

            DomainManager = new EntityDomainManager<ItemSubCategory>(context, Request);
        }

        // GET tables/ItemSubCategory
        public IQueryable<ItemSubCategory> GetAllItemSubCategories()
        {
            return Query();
        }

        // GET tables/ItemSubCategory
        public SingleResult<ItemSubCategory> GetItemSubCategory(string id)
        {
            return Lookup(id);
        }

        // PATCH tables/ItemSubCategory
        public Task<ItemSubCategory> PatchItemSubCategory(string id, Delta<ItemSubCategory> patch)
        {
            return UpdateAsync(id, patch);
        }

        // POST tables/ItemSubCategory
        //insert into the data base
        public async Task<IHttpActionResult> PostItemSubCategory(ItemSubCategory itemSubCategory)
        {
            ItemSubCategory current = await InsertAsync(itemSubCategory);
            return CreatedAtRoute("Tables", new { id = current.Id }, current);
        }

        // DELETE tables/ItemSubCategory
        public Task DeleteItemSubCategory(string id)
        {
            return DeleteAsync(id);
        }
    }
}