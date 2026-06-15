using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Medri.Services.Medri.Application;

namespace Medri.Web.Areas.Admin.Dashboard
{
    [Area("Admin")]
    public partial class DashboardController : AdminAreaController
    {
        private readonly AdminDashboardQuery adminDashboardQuery;

        public DashboardController(AdminDashboardQuery adminDashboardQuery)
        {
            this.adminDashboardQuery = adminDashboardQuery;
        }

        [HttpGet]
        public virtual async Task<IActionResult> Index()
        {
            var result = await adminDashboardQuery.ExecuteAsync(HttpContext.RequestAborted);
            return View(AdminDashboardViewModelMapper.Create(result, User));
        }
    }
}
