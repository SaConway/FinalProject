using System;
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
    public class CategoryController : TableController<ItemCategory>
    {
        protected override void Initialize(HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext);
            MsorLiContext context = new MsorLiContext();

            DomainManager = new EntityDomainManager<ItemCategory>(context, Request);
        }

        // GET tables/ItemCategory
        public IQueryable<ItemCategory> GetAllCategories()
        {
            return Query();
        }

        // GET tables/ItemCategory
        public SingleResult<ItemCategory> GetCategory(string id)
        {
            return Lookup(id);
        }

        // PATCH tables/ItemCategory
        public Task<ItemCategory> PatchCategory(string id, Delta<ItemCategory> patch)
        {
            return UpdateAsync(id, patch);
        }

        // POST tables/ItemCategory
        //insert into the data base
        public async Task<IHttpActionResult> PostCategory(ItemCategory category)
        {
            ItemCategory current = await InsertAsync(category);
            return CreatedAtRoute("Tables", new { id = current.Id }, current);
        }

        // DELETE tables/ItemCategory
        public Task DeleteCategory(string id)
        {
            return DeleteAsync(id);
        }
    }
}