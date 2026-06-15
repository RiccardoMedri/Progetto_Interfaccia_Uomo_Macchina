using Microsoft.AspNetCore.Authorization;
using Medri.Services.Medri.Identity;
using Medri.Web.Infrastructure;

namespace Medri.Web.Areas.Client
{
    [Authorize(Roles = UserRoles.Client)]
    public abstract class ClientAreaController : AuthenticatedBaseController
    {
    }
}
