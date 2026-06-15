using System;
using System.Collections.Generic;

namespace Medri.Services.Medri.Application
{
    public sealed class ClientRequestsResultDto
    {
        public int FavoriteCount { get; set; }

        public int TotalItems { get; set; }

        public int Page { get; set; } = 1;

        public int PageSize { get; set; } = 10;

        public IReadOnlyList<ClientRequestDto> Requests { get; set; } =
            Array.Empty<ClientRequestDto>();
    }

    public sealed class ClientRequestDto
    {
        public Guid Id { get; set; }

        public string RequestType { get; set; }

        public string Status { get; set; }

        public string DesiredLocation { get; set; }

        public int? MinimumRooms { get; set; }

        public string SustainableBudgetLabel { get; set; }

        public string PropertyType { get; set; }

        public string PropertyTitle { get; set; }

        public string PropertySlug { get; set; }

        public string Notes { get; set; }

        public DateTime CreatedAtUtc { get; set; }
    }
}
