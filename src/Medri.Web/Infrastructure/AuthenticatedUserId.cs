using System;
using System.Security.Claims;

namespace Medri.Web.Infrastructure
{
    internal static class AuthenticatedUserId
    {
        public static Guid? Get(ClaimsPrincipal user)
        {
            var value = user?.FindFirstValue(ClaimTypes.NameIdentifier);
            return Guid.TryParse(value, out var userId) ? userId : null;
        }
    }
}
