using Microsoft.EntityFrameworkCore;
using System.Linq;
using Medri.Services.Medri.Identity;

namespace Medri.Services.Medri.Application
{
    internal static class StaffUserQueries
    {
        public static IQueryable<User> Assignable(MedriDbContext dbContext)
        {
            return dbContext.Users
                .AsNoTracking()
                .Where(user => user.Role == UserRoles.Admin || user.Role == UserRoles.Operator);
        }

        public static string AgencyRoleLabel(string agencyRole, string accountRole)
        {
            if (!string.IsNullOrWhiteSpace(agencyRole))
            {
                return agencyRole;
            }

            return accountRole == UserRoles.Admin
                ? AgencyStaffRoles.Manager
                : AgencyStaffRoles.Operator;
        }
    }
}
