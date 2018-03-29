using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.OData;
using Microsoft.Azure.Mobile.Server;
using MsorLiService.Models;
using MsorLiService.DataObjects;

namespace MsorLiService.Controllers
{
    public class ItemImageController : TableController<ItemImage>
    {
        protected override void Initialize(HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext);
            MsorLiContext context = new MsorLiContext();

            DomainManager = new EntityDomainManager<ItemImage>(context, Request);

            //if we would like to do "soft delete" we need to use the next command:
            //DomainManager = new EntityDomainManager<Item>(context, Request, enableSoftDelete: true);
            //soft delete is a flag that we use for each column in the table.
            //this flag is used as a way to delete data without actually deleting it.
            //we use the flag as a way to know if the content is deleted or not.
            //this way we could also recover deleted items
        }

        public IQueryable<ItemImage> GetAllImages()
        {
            return Query()
                    .Where(itemImage => itemImage.IsPriorityImage == true)
                    .OrderByDescending(Item => Item.CreatedAt);
        }

        public SingleResult<ItemImage> GetImage(string id)
        {
            return Lookup(id);
        }

        public Task<ItemImage> PatchImage(string id, Delta<ItemImage> patch)
        {
            return UpdateAsync(id, patch);
        }

        public async Task<IHttpActionResult> PostImage(ItemImage image)
        {
            ItemImage current = await InsertAsync(image);
            return CreatedAtRoute("Tables", new { id = current.Id }, current);
        }

        public Task DeleteImage(string id)
        {
            return DeleteAsync(id);
        }
    }
}