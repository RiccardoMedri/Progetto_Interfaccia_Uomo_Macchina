namespace Medri.Web.Features.Search
{
    public class SearchResultCardViewModel
    {
        public string Id { get; set; }

        public string Slug { get; set; }

        public string Title { get; set; }

        public string Tag { get; set; }

        public string PriceLabel { get; set; }

        public string DisplayLocation { get; set; }

        public string FactsLabel { get; set; }

        public string MobileFactsLabel { get; set; }

        public string ImageUrl { get; set; }

        public string DetailUrl => "/immobili/" + Slug;

        public bool IsSaved { get; set; }
    }
}
