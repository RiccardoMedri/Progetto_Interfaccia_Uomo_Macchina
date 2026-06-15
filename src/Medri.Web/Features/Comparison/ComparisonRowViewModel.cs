using System.Collections.Generic;

namespace Medri.Web.Features.Comparison
{
    public class ComparisonRowViewModel
    {
        public string Criterion { get; set; }

        public IReadOnlyList<ComparisonValueViewModel> Values { get; set; } =
            new List<ComparisonValueViewModel>();
    }
}
