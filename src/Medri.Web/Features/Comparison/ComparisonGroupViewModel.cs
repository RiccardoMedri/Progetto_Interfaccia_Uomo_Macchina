using System.Collections.Generic;

namespace Medri.Web.Features.Comparison
{
    public class ComparisonGroupViewModel
    {
        public string Id { get; set; }

        public string HeadingId { get; set; }

        public string Title { get; set; }

        public bool IsOpen { get; set; }

        public IReadOnlyList<ComparisonRowViewModel> Rows { get; set; } =
            new List<ComparisonRowViewModel>();
    }
}
