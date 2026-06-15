using System.Collections.Generic;
using Medri.Web.Features.Search;

namespace Medri.Web.Features.Home
{
    public class HomeIndexViewModel
    {
        public GoogleReviewsSummaryViewModel Reviews { get; set; } = GoogleReviewsSummaryViewModel.Unconfigured();

        public IReadOnlyList<HomeFeaturedListingCardViewModel> FeaturedListings { get; set; } =
            new List<HomeFeaturedListingCardViewModel>();

        public IReadOnlyList<HomeSearchFilterGroupViewModel> SearchFilterGroups { get; set; } =
            BuildSearchFilterGroups();

        public IReadOnlyList<HomeServiceIntentViewModel> ServiceIntents { get; set; } =
            new List<HomeServiceIntentViewModel>
            {
                new HomeServiceIntentViewModel
                {
                    Href = "/richieste/comprare",
                    Icon = "ph-house-line",
                    Title = "Voglio comprare casa",
                    Text = "Raccontaci budget, zone e priorita per orientare meglio la ricerca."
                },
                new HomeServiceIntentViewModel
                {
                    Href = "/richieste/affittare",
                    Icon = "ph-key",
                    Title = "Voglio affittare casa",
                    Text = "Indica canone, tempi e garanzie per ricevere un contatto utile."
                },
                new HomeServiceIntentViewModel
                {
                    Href = "/richieste/vendere",
                    Icon = "ph-tag",
                    Title = "Voglio vendere casa",
                    Text = "Prepara il primo confronto su immobile, obiettivi e tempi."
                },
                new HomeServiceIntentViewModel
                {
                    Href = "/richieste/affidare-affitto",
                    Icon = "ph-buildings",
                    Title = "Voglio mettere in affitto",
                    Text = "Imposta pubblicazione, selezione dell'inquilino e gestione del contratto."
                }
            };

        public IReadOnlyList<HomeNeighborhoodViewModel> Neighborhoods { get; set; } =
            new List<HomeNeighborhoodViewModel>
            {
                new HomeNeighborhoodViewModel
                {
                    Name = "Centro storico",
                    Text = "Servizi, scuole e collegamenti a pochi passi."
                },
                new HomeNeighborhoodViewModel
                {
                    Name = "Oltresavio",
                    Text = "Soluzioni familiari e contesto residenziale."
                },
                new HomeNeighborhoodViewModel
                {
                    Name = "S. Egidio",
                    Text = "Case indipendenti, verde e tranquillita."
                },
                new HomeNeighborhoodViewModel
                {
                    Name = "Diegaro",
                    Text = "Nuove costruzioni e collegamenti rapidi."
                }
            };

        public IReadOnlyList<HomeWhyCardViewModel> WhyCards { get; set; } =
            new List<HomeWhyCardViewModel>
            {
                new HomeWhyCardViewModel
                {
                    Icon = "ph-map-pin",
                    Title = "Esperienza sul territorio",
                    Text = "Conosciamo quartieri, valori e tempi reali del mercato locale."
                },
                new HomeWhyCardViewModel
                {
                    Icon = "ph-target",
                    Title = "Metodo su misura",
                    Text = "Ogni percorso parte da obiettivi, budget e tempistiche concrete."
                },
                new HomeWhyCardViewModel
                {
                    Icon = "ph-handshake",
                    Title = "Supporto completo",
                    Text = "Visite, trattative, documenti e passaggi finali coordinati."
                }
            };

        private static IReadOnlyList<HomeSearchFilterGroupViewModel> BuildSearchFilterGroups()
        {
            var options = new SearchFilterOptionsViewModel();
            return new List<HomeSearchFilterGroupViewModel>
            {
                new HomeSearchFilterGroupViewModel
                {
                    Id = "home-contracts",
                    Name = "contracts",
                    Label = "Contratto",
                    Options = options.Contracts
                },
                new HomeSearchFilterGroupViewModel
                {
                    Id = "home-property-types",
                    Name = "propertyTypes",
                    Label = "Tipologia",
                    Options = options.PropertyTypes
                },
                new HomeSearchFilterGroupViewModel
                {
                    Id = "home-rooms",
                    Name = "rooms",
                    Label = "Locali",
                    Options = options.Rooms
                }
            };
        }
    }

    public class HomeSearchFilterGroupViewModel
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string Label { get; set; }

        public IReadOnlyList<SearchFilterOptionViewModel> Options { get; set; } =
            new List<SearchFilterOptionViewModel>();
    }

    public class HomeServiceIntentViewModel
    {
        public string Href { get; set; }

        public string Icon { get; set; }

        public string Title { get; set; }

        public string Text { get; set; }
    }

    public class HomeNeighborhoodViewModel
    {
        public string Name { get; set; }

        public string Text { get; set; }
    }

    public class HomeWhyCardViewModel
    {
        public string Icon { get; set; }

        public string Title { get; set; }

        public string Text { get; set; }
    }
}
