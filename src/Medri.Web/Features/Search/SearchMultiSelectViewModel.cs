using System.Collections.Generic;
using System.Linq;

namespace Medri.Web.Features.Search
{
    public class SearchMultiSelectViewModel
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string Placeholder { get; set; }

        public IReadOnlyList<SearchFilterOptionViewModel> Options { get; set; } =
            new List<SearchFilterOptionViewModel>();

        public IReadOnlyCollection<string> SelectedValues { get; set; } =
            new List<string>();

        public string SelectedLabel
        {
            get
            {
                var count = Items.Count(item => item.IsSelected);
                return count == 0
                    ? Placeholder
                    : count == 1
                        ? "1 selezione"
                        : count + " selezioni";
            }
        }

        public IReadOnlyList<SearchMultiSelectOptionItemViewModel> Items =>
            Options
                .Select((option, index) => new SearchMultiSelectOptionItemViewModel
                {
                    Id = Id + "-option-" + index,
                    Name = Name,
                    Value = option.Value,
                    Label = option.Label,
                    IsSelected = SelectedValues?.Contains(option.Value) == true
                })
                .ToList();
    }

    public class SearchMultiSelectOptionItemViewModel
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string Value { get; set; }

        public string Label { get; set; }

        public bool IsSelected { get; set; }
    }
}
