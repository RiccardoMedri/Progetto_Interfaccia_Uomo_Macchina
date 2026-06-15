namespace Medri.Web.Features.Property
{
    public class PropertySummaryCardViewModel
    {
        public string Id { get; set; }

        public string Title { get; set; }

        public string Slug { get; set; }

        public string Tag { get; set; }

        public string PriceLabel { get; set; }

        public string DisplayLocation { get; set; }

        public string ImageUrl { get; set; }

        public string DetailUrl => "/immobili/" + Slug;
    }
}
