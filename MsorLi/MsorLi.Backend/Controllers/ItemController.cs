using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.OData;
using Microsoft.Azure.Mobile.Server;
using MsorLi.DataObjects;
using MsorLi.Backend.Models;

namespace MsorLi.Backend.Controllers
{
    public class ItemController : TableController<Item>
    {
        protected override void Initialize(HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext);
            MasterDetailContext context = new MasterDetailContext();

            DomainManager = new EntityDomainManager<Item>(context, Request);

            //if we would like to do "soft delete" we need to use the next command:
            //DomainManager = new EntityDomainManager<Item>(context, Request, enableSoftDelete: true);
            //soft delete is a flag that we use for each column in the table.
            //this flag is used as a way to delete data without actually deleting it.
            //we use the flag as a way to know if the content is deleted or not.
            //this way we could also recover deleted items
        }

        // GET tables/Item
        public IQueryable<Item> GetAllItems()
        {
            return Query();
        } 

        // GET tables/Item/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public SingleResult<Item> GetItem(string id)
        {
            return Lookup(id);
        }

        // PATCH tables/Item/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public Task<Item> PatchItem(string id, Delta<Item> patch)
        {
            return UpdateAsync(id, patch);
        }

        // POST tables/Item
        //insert into the data base
        public async Task<IHttpActionResult> PostItem(Item item)
        {
            Item current = await InsertAsync(item);
            return CreatedAtRoute("Tables", new { id = current.Id }, current);
        }

        // DELETE tables/Item/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public Task DeleteItem(string id)
        {
            return DeleteAsync(id);
        }
    }
}