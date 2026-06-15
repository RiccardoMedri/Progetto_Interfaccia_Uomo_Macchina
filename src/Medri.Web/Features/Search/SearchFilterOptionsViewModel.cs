using System.Collections.Generic;

namespace Medri.Web.Features.Search
{
    public class SearchFilterOptionViewModel
    {
        public SearchFilterOptionViewModel(string value, string label)
        {
            Value = value;
            Label = label;
        }

        public string Value { get; }

        public string Label { get; }
    }

    public class SearchFilterOptionsViewModel
    {
        public IReadOnlyList<SearchFilterOptionViewModel> Contracts { get; } =
            new List<SearchFilterOptionViewModel>
            {
                new SearchFilterOptionViewModel("Vendita", "Vendita"),
                new SearchFilterOptionViewModel("Affitto", "Affitto"),
                new SearchFilterOptionViewModel("Nuove costruzioni", "Nuove costruzioni")
            };

        public IReadOnlyList<SearchFilterOptionViewModel> PropertyTypes { get; } =
            new List<SearchFilterOptionViewModel>
            {
                new SearchFilterOptionViewModel("Appartamento", "Appartamento"),
                new SearchFilterOptionViewModel("Casa indipendente", "Casa indipendente"),
                new SearchFilterOptionViewModel("Villetta", "Villetta"),
                new SearchFilterOptionViewModel("Attico", "Attico"),
                new SearchFilterOptionViewModel("Monolocale", "Monolocale"),
                new SearchFilterOptionViewModel("Bilocale", "Bilocale"),
                new SearchFilterOptionViewModel("Trilocale", "Trilocale"),
                new SearchFilterOptionViewModel("Quadrilocale", "Quadrilocale"),
                new SearchFilterOptionViewModel("Loft", "Loft"),
                new SearchFilterOptionViewModel("Porzione", "Porzione")
            };

        public IReadOnlyList<SearchFilterOptionViewModel> Rooms { get; } =
            new List<SearchFilterOptionViewModel>
            {
                new SearchFilterOptionViewModel("2", "2 locali"),
                new SearchFilterOptionViewModel("3", "3 locali"),
                new SearchFilterOptionViewModel("4-plus", "4 o piu locali")
            };

        public IReadOnlyList<SearchFilterOptionViewModel> Zones { get; } =
            new List<SearchFilterOptionViewModel>
            {
                new SearchFilterOptionViewModel("Centro", "Centro"),
                new SearchFilterOptionViewModel("Centro storico", "Centro storico"),
                new SearchFilterOptionViewModel("Fiorenzuola", "Fiorenzuola"),
                new SearchFilterOptionViewModel("Fiorita", "Fiorita"),
                new SearchFilterOptionViewModel("Ponte Abbadesse", "Ponte Abbadesse"),
                new SearchFilterOptionViewModel("Case Finali", "Case Finali"),
                new SearchFilterOptionViewModel("San Mauro", "San Mauro"),
                new SearchFilterOptionViewModel("Stazione", "Stazione"),
                new SearchFilterOptionViewModel("Oltresavio", "Oltresavio"),
                new SearchFilterOptionViewModel("Diegaro", "Diegaro"),
                new SearchFilterOptionViewModel("Sant'Egidio", "Sant'Egidio"),
                new SearchFilterOptionViewModel("Cesena Nord", "Cesena Nord")
            };

        public IReadOnlyList<SearchFilterOptionViewModel> PriceRanges { get; } =
            new List<SearchFilterOptionViewModel>
            {
                new SearchFilterOptionViewModel("sale-up-to-150000", "Vendita: fino a EUR 150.000"),
                new SearchFilterOptionViewModel("sale-150000-250000", "Vendita: EUR 150.000 - 250.000"),
                new SearchFilterOptionViewModel("sale-250000-350000", "Vendita: EUR 250.000 - 350.000"),
                new SearchFilterOptionViewModel("sale-350000-500000", "Vendita: EUR 350.000 - 500.000"),
                new SearchFilterOptionViewModel("sale-over-500000", "Vendita: oltre EUR 500.000"),
                new SearchFilterOptionViewModel("rent-up-to-500", "Affitto: fino a EUR 500 / mese"),
                new SearchFilterOptionViewModel("rent-500-800", "Affitto: EUR 500 - 800 / mese"),
                new SearchFilterOptionViewModel("rent-800-1200", "Affitto: EUR 800 - 1.200 / mese"),
                new SearchFilterOptionViewModel("rent-over-1200", "Affitto: oltre EUR 1.200 / mese")
            };

        public IReadOnlyList<SearchFilterOptionViewModel> Features { get; } =
            new List<SearchFilterOptionViewModel>
            {
                new SearchFilterOptionViewModel("garden", "Giardino"),
                new SearchFilterOptionViewModel("terrace", "Balcone / terrazzo"),
                new SearchFilterOptionViewModel("garage", "Garage"),
                new SearchFilterOptionViewModel("parking", "Posto auto"),
                new SearchFilterOptionViewModel("elevator", "Ascensore"),
                new SearchFilterOptionViewModel("furnished", "Arredato"),
                new SearchFilterOptionViewModel("move-in-ready", "Pronto da abitare"),
                new SearchFilterOptionViewModel("renovation-needed", "Da ristrutturare"),
                new SearchFilterOptionViewModel("high-energy-class", "Classe energetica alta"),
                new SearchFilterOptionViewModel("near-services", "Vicino ai servizi"),
                new SearchFilterOptionViewModel("near-public-transport", "Vicino ai mezzi pubblici")
            };

        public IReadOnlyList<SearchFilterOptionViewModel> Bathrooms { get; } =
            new List<SearchFilterOptionViewModel>
            {
                new SearchFilterOptionViewModel("1", "1 bagno"),
                new SearchFilterOptionViewModel("2", "2 bagni"),
                new SearchFilterOptionViewModel("3-plus", "3 o piu bagni")
            };

        public IReadOnlyList<SearchFilterOptionViewModel> SurfaceRanges { get; } =
            new List<SearchFilterOptionViewModel>
            {
                new SearchFilterOptionViewModel("up-to-60", "Fino a 60 mq"),
                new SearchFilterOptionViewModel("60-100", "60 - 100 mq"),
                new SearchFilterOptionViewModel("100-150", "100 - 150 mq"),
                new SearchFilterOptionViewModel("over-150", "Oltre 150 mq")
            };

        public IReadOnlyList<SearchFilterOptionViewModel> EnergyClasses { get; } =
            new List<SearchFilterOptionViewModel>
            {
                new SearchFilterOptionViewModel("A", "Classe A"),
                new SearchFilterOptionViewModel("B", "Classe B"),
                new SearchFilterOptionViewModel("C", "Classe C"),
                new SearchFilterOptionViewModel("D", "Classe D"),
                new SearchFilterOptionViewModel("E", "Classe E")
            };
    }
}
