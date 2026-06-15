using Microsoft.AspNetCore.Authorization;
using Medri.Services.Medri.Identity;
using Medri.Web.Infrastructure;

namespace Medri.Web.Areas.Admin
{
    [Authorize(Roles = UserRoles.AdminArea)]
    public abstract class AdminAreaController : AuthenticatedBaseController
    {
    }
}
