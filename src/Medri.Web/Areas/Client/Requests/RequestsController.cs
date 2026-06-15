using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Medri.Services.Medri.Application;
using Medri.Web.Infrastructure;

namespace Medri.Web.Areas.Client.Requests
{
    [Area("Client")]
    public partial class RequestsController : ClientAreaController
    {
        private readonly ClientRequestsQuery clientRequestsQuery;

        public RequestsController(ClientRequestsQuery clientRequestsQuery)
        {
            this.clientRequestsQuery = clientRequestsQuery;
        }

        [HttpGet]
        public virtual async Task<IActionResult> Index(ClientRequestsInputModel input)
        {
            input ??= new ClientRequestsInputModel();

            var userId = AuthenticatedUserId.Get(User);
            if (!userId.HasValue)
            {
                return Challenge();
            }

            var result = await clientRequestsQuery.ExecuteAsync(
                userId.Value,
                input.Page,
                input.PageSize,
                HttpContext.RequestAborted);
            return View(ClientRequestsViewModelMapper.Create(result));
        }
    }
}
