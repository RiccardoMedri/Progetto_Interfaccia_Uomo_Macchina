using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;
using Medri.Services.Medri.Application;

namespace Medri.Web.Features.Comparison
{
    public partial class ComparisonController : Controller
    {
        private readonly ComparisonQuery comparisonQuery;

        public ComparisonController(ComparisonQuery comparisonQuery)
        {
            this.comparisonQuery = comparisonQuery;
        }

        [HttpGet]
        public virtual async Task<IActionResult> Index(string ids)
        {
            var result = await comparisonQuery.ExecuteAsync(ids, HttpContext.RequestAborted);
            if (result.Count < 2)
            {
                return RedirectToAction("Index", "Search");
            }

            return View(ComparisonViewModelMapper.Create(result));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual IActionResult Remove(ComparisonRemoveInputModel input)
        {
            var remainingIds = (input.Ids ?? string.Empty)
                .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Where(id => !string.Equals(id, input.RemoveId, StringComparison.OrdinalIgnoreCase));

            var remainingIdList = remainingIds.ToList();
            if (remainingIdList.Count < 2)
            {
                return RedirectToAction("Index", "Search");
            }

            var ids = string.Join(",", remainingIdList);
            return RedirectToAction(nameof(Index), new { ids });
        }
    }
}
