using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Medri.Services.Medri.Application;
using Medri.Web.Infrastructure;

namespace Medri.Web.Areas.Client.Requests
{
    [Area("Client")]
    public partial class RequestsController : ClientAreaController
    {
        private readonly ClientRequestsQuery clientRequestsQuery;
        private readonly ClaimClientRequestsCommand claimClientRequestsCommand;

        public RequestsController(
            ClientRequestsQuery clientRequestsQuery,
            ClaimClientRequestsCommand claimClientRequestsCommand)
        {
            this.clientRequestsQuery = clientRequestsQuery;
            this.claimClientRequestsCommand = claimClientRequestsCommand;
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

            await ClaimPendingRequestsAsync(userId.Value);

            var result = await clientRequestsQuery.ExecuteAsync(
                userId.Value,
                input.Page,
                input.PageSize,
                HttpContext.RequestAborted);
            return View(ClientRequestsViewModelMapper.Create(result));
        }

        private async Task ClaimPendingRequestsAsync(Guid userId)
        {
            var pendingRequestIds = PendingClientRequestSession.Read(HttpContext.Session);
            if (pendingRequestIds.Count == 0)
            {
                return;
            }

            await claimClientRequestsCommand.ExecuteAsync(
                userId,
                pendingRequestIds,
                HttpContext.RequestAborted);
            PendingClientRequestSession.Clear(HttpContext.Session);
        }
    }
}
