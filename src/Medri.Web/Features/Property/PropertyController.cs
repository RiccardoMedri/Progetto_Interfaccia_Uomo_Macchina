using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;
using Medri.Services.Medri.Application;
using Medri.Services.Medri.Identity;
using Medri.Web.Infrastructure;

namespace Medri.Web.Features.Property
{
    public partial class PropertyController : Controller
    {
        private readonly PropertyDetailQuery propertyDetailQuery;
        private readonly AgencyContactOptions agencyContactOptions;

        public PropertyController(
            PropertyDetailQuery propertyDetailQuery,
            IOptions<AgencyContactOptions> agencyContactOptions)
        {
            this.propertyDetailQuery = propertyDetailQuery;
            this.agencyContactOptions = agencyContactOptions.Value;
        }

        [HttpGet]
        public virtual async Task<IActionResult> Detail(string slug, bool preview = false)
        {
            var includeUnpublished = preview &&
                (User.IsInRole(UserRoles.Admin) || User.IsInRole(UserRoles.Operator));

            var result = await propertyDetailQuery.ExecuteAsync(
                slug,
                AuthenticatedUserId.Get(User),
                includeUnpublished,
                HttpContext.RequestAborted);
            if (result == null)
            {
                return NotFound();
            }

            return View(PropertyViewModelMapper.Create(result, agencyContactOptions, includeUnpublished));
        }
    }
}
