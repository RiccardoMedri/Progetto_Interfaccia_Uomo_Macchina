using System.Collections.Generic;
using System;

namespace Medri.Web.Features.Comparison
{
    public class ComparisonIndexViewModel
    {
        public IReadOnlyList<ComparisonPropertyViewModel> Properties { get; set; } =
            new List<ComparisonPropertyViewModel>();

        public IReadOnlyList<ComparisonGroupViewModel> Groups { get; set; } =
            new List<ComparisonGroupViewModel>();

        public string SelectedIdsQueryValue { get; set; }

        public int SelectedCount => Properties.Count;

        public int CardColumns => Math.Clamp(SelectedCount, 1, 4);
    }
}
