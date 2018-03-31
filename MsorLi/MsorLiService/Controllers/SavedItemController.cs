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
    public class SavedItemController : TableController<SavedItem>
    {
        protected override void Initialize(HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext);
            MsorLiContext context = new MsorLiContext();

            DomainManager = new EntityDomainManager<SavedItem>(context, Request);
        }

        // GET tables/SavedItem
        public IQueryable<SavedItem> GetAllSavedItems()
        {
            return Query();
        }

        // GET tables/SavedItem
        public SingleResult<SavedItem> GetSavedItem(string id)
        {
            return Lookup(id);
        }

        // PATCH tables/SavedItem
        public Task<SavedItem> PatchSavedItem(string id, Delta<SavedItem> patch)
        {
            return UpdateAsync(id, patch);
        }

        // POST tables/SavedItem
        //insert into the data base
        public async Task<IHttpActionResult> PostSavedItem(SavedItem itemCategory)
        {
            SavedItem current = await InsertAsync(itemCategory);
            return CreatedAtRoute("Tables", new { id = current.Id }, current);
        }

        // DELETE tables/SavedItem
        public Task DeleteSavedItem(string id)
        {
            return DeleteAsync(id);
        }
    }
}