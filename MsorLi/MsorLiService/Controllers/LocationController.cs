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
    public class LocationController : TableController<Location>
    {
        protected override void Initialize(HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext);
            MsorLiContext context = new MsorLiContext();

            DomainManager = new EntityDomainManager<Location>(context, Request);
        }

        // GET tables/Location
        public IQueryable<Location> GetAllLocationCategories()
        {
            return Query();
        }

        // GET tables/Location
        public SingleResult<Location> GetLocation(string id)
        {
            return Lookup(id);
        }

        // PATCH tables/Location
        public Task<Location> PatchLocation(string id, Delta<Location> patch)
        {
            return UpdateAsync(id, patch);
        }

        // POST tables/Location
        //insert into the data base
        public async Task<IHttpActionResult> PostLocation(Location Location)
        {
            Location current = await InsertAsync(Location);
            return CreatedAtRoute("Tables", new { id = current.Id }, current);
        }

        // DELETE tables/Location
        public Task DeleteLocation(string id)
        {
            return DeleteAsync(id);
        }
    }
}