using System;
using System.Linq;
using Medri.Services;
using Medri.Services.Medri;
using Medri.Services.Medri.Application;
using Medri.Services.Medri.Identity;

namespace Medri.Infrastructure
{
    public class DataGenerator
    {
        public static void InitializeUsers(MedriDbContext context)
        {
            if (context.Users.Any())
            {
                return;   
            }

            context.Users.AddRange(
                new User
                {
                    Id = Guid.Parse("40000000-0000-0000-0000-000000000001"),
                    Email = "elena.gori@email.it",
                    Password = "XohImNooBHFR0OVvjcYpJ3NgPQ1qq73WKhHvch0VQtg=", 
                    FirstName = "Elena",
                    LastName = "Gori",
                    NickName = "Elena",
                    Role = UserRoles.Client
                },
                new User
                {
                    Id = Guid.Parse("40000000-0000-0000-0000-000000000002"),
                    Email = "chiara.medri@email.it",
                    Password = "XohImNooBHFR0OVvjcYpJ3NgPQ1qq73WKhHvch0VQtg=", 
                    FirstName = "Chiara",
                    LastName = "Medri",
                    NickName = "Chiara",
                    Role = UserRoles.Admin
                });

            context.SaveChanges();
        }

        public static void InitializeMedriDemoData(MedriDbContext context)
        {
            if (context.AgencyUsers.Any())
            {
                return;
            }

            var seedDate = new DateTime(2026, 5, 26, 12, 0, 0, DateTimeKind.Utc);
            var agencyUserId = Guid.Parse("10000000-0000-0000-0000-000000000001");
            var martinaAgencyUserId = Guid.Parse("10000000-0000-0000-0000-000000000002");
            var chiaraAgencyUserId = Guid.Parse("10000000-0000-0000-0000-000000000003");
            var lorenzoAgencyUserId = Guid.Parse("10000000-0000-0000-0000-000000000004");
            var marcoAgencyUserId = Guid.Parse("10000000-0000-0000-0000-000000000005");
            var propertyListingId = Guid.Parse("20000000-0000-0000-0000-000000000001");
            var leadId = Guid.Parse("30000000-0000-0000-0000-000000000001");
            var clientBuyLeadId = Guid.Parse("30000000-0000-0000-0000-000000000101");
            var clientValuationLeadId = Guid.Parse("30000000-0000-0000-0000-000000000102");
            var paoloLeadId = Guid.Parse("30000000-0000-0000-0000-000000000201");
            var niccoloLeadId = Guid.Parse("30000000-0000-0000-0000-000000000202");
            var annaLeadId = Guid.Parse("30000000-0000-0000-0000-000000000203");
            var marcoLeadId = Guid.Parse("30000000-0000-0000-0000-000000000204");
            var clientUserId = Guid.Parse("40000000-0000-0000-0000-000000000001");
            var propertyListings = new[]
            {
                new PropertyListing
                {
                    Id = propertyListingId,
                    Title = "Villetta a schiera",
                    Slug = "villetta-a-schiera",
                    Location = "Cesena - S. Egidio",
                    DisplayLocation = "Cesena - S. Egidio",
                    Price = 285000m,
                    Rooms = 4,
                    Bathrooms = 2,
                    SurfaceSquareMeters = 120,
                    Status = "Vendita",
                    Contract = "Vendita",
                    PropertyType = "Villetta",
                    Zone = "Sant'Egidio",
                    FeatureKeys = "|garden|garage|parking|move-in-ready|renovation-needed|near-services|near-public-transport|",
                    ImageUrl = "/medri-reference/assets/properties/property-03.jpg",
                    Latitude = 44.1530,
                    Longitude = 12.2860,
                    SortOrder = 1,
                    Address = "Via S. Egidio 42, Cesena",
                    BedroomsLabel = "3",
                    FloorLabel = "Terra + primo",
                    GarageLabel = "Si",
                    OutdoorSpaceLabel = "Giardino privato",
                    EnergyClass = "D",
                    AvailabilityLabel = "Da concordare",
                    HeatingLabel = "Autonomo",
                    RequiredWorksLabel = "Leggeri",
                    ConstructionYearLabel = "1998",
                    CondoFeesLabel = "Nessuna",
                    BalconyLabel = "Si",
                    CellarLabel = "Si",
                    NearbyServicesLabel = "Scuole e negozi",
                    SummaryTitle = "Villetta a schiera con giardino privato",
                    SummaryParagraph1 = "Villetta a schiera in contesto residenziale, distribuita su piu livelli e pensata per chi cerca spazi funzionali, zona giorno separata, camere comode e un esterno privato da vivere nella bella stagione.",
                    SummaryParagraph2 = "La casa e abitabile subito e permette di programmare nel tempo piccoli interventi di aggiornamento. La planimetria e trattata come materiale fotografico nella gallery, cosi resta consultabile insieme alle immagini principali dell'immobile.",
                    ReadinessNote = "Abitabile subito, con finiture da aggiornare nel tempo.",
                    CostsNote = "Interventi leggeri su bagno, tinteggiatura e serramenti.",
                    ContextNote = "Zona residenziale tranquilla, servizi raggiungibili in auto.",
                    DecisionMarginNote = "Buona soluzione se si cerca indipendenza senza villa isolata.",
                    HumanFitNote = "Cerchi una casa per una famiglia o una coppia che vuole piu indipendenza, accetti qualche aggiornamento leggero e dai valore a giardino, garage e tranquillita della zona.",
                    ZoneComparisonLabel = "S. Egidio, contesto residenziale",
                    SurfaceRoomsComparisonLabel = "120 mq, 4 locali",
                    StatusWorksComparisonLabel = "Buono, lavori leggeri",
                    MainCompromise = "Lavori leggeri da pianificare",
                    AccessLabel = "Ingresso indipendente",
                    ManagementCostsLabel = "Nulle o molto basse",
                    EstimatedWorksLabel = "Finiture e manutenzioni leggere",
                    EnergyCostsLabel = "Medio-alti",
                    PersonalizationLabel = "Alta, piu autonomia sugli interventi",
                    TransportLabel = "Fermata bus nelle vicinanze",
                    PrivacyLabel = "Alta, ingresso indipendente",
                    NoiseLabel = "Bassa",
                    IdealTargetLabel = "Famiglia che cerca indipendenza"
                },
                new PropertyListing
                {
                    Id = Guid.Parse("20000000-0000-0000-0000-000000000002"),
                    Title = "Appartamento con giardino",
                    Slug = "appartamento-con-giardino",
                    Location = "Cesena - Centro",
                    DisplayLocation = "Cesena - Centro",
                    Price = 210000m,
                    Rooms = 3,
                    Bathrooms = 1,
                    SurfaceSquareMeters = 95,
                    Status = "Vendita",
                    Contract = "Vendita",
                    PropertyType = "Appartamento",
                    Zone = "Centro storico",
                    FeatureKeys = "|garden|parking|near-services|near-public-transport|renovation-needed|",
                    ImageUrl = "/medri-reference/assets/properties/property-01.jpg",
                    Latitude = 44.1396,
                    Longitude = 12.2431,
                    SortOrder = 2,
                    Address = "Via Mura Barriera Ponente 12, Cesena",
                    BedroomsLabel = "2",
                    FloorLabel = "Piano rialzato",
                    GarageLabel = "Posto auto condominiale",
                    OutdoorSpaceLabel = "Giardino condominiale",
                    EnergyClass = "C",
                    AvailabilityLabel = "Libera subito",
                    HeatingLabel = "Autonomo",
                    RequiredWorksLabel = "Aggiornamenti interni",
                    ConstructionYearLabel = "2005",
                    CondoFeesLabel = "Circa EUR 80 / mese",
                    BalconyLabel = "No",
                    CellarLabel = "Si",
                    NearbyServicesLabel = "Negozi e servizi a piedi",
                    SummaryTitle = "Appartamento con giardino",
                    SummaryParagraph1 = "Appartamento in zona centrale con spazi comodi, servizi vicini e giardino condominiale.",
                    SummaryParagraph2 = "Soluzione indicata per chi vuole contenere il budget senza rinunciare alla prossimita ai servizi principali.",
                    ReadinessNote = "Libera subito, con aggiornamenti interni programmabili.",
                    CostsNote = "Spese condominiali prevedibili e interventi circoscritti.",
                    ContextNote = "Centro, servizi raggiungibili a piedi.",
                    DecisionMarginNote = "Buon equilibrio tra prezzo, posizione e gestione.",
                    HumanFitNote = "Potrebbe essere adatta se cerchi servizi vicini e un prezzo piu contenuto rispetto a soluzioni indipendenti.",
                    ZoneComparisonLabel = "Centro, servizi raggiungibili a piedi",
                    SurfaceRoomsComparisonLabel = "95 mq, 3 locali",
                    StatusWorksComparisonLabel = "Buono, aggiornamenti interni",
                    MainCompromise = "Meno indipendenza",
                    AccessLabel = "Piano rialzato",
                    ManagementCostsLabel = "Circa EUR 80 / mese",
                    EstimatedWorksLabel = "Bagno e finiture da aggiornare",
                    EnergyCostsLabel = "Medi",
                    PersonalizationLabel = "Media, vincoli condominiali",
                    TransportLabel = "Centro e stazione piu accessibili",
                    PrivacyLabel = "Media, contesto condominiale",
                    NoiseLabel = "Media, zona piu centrale",
                    IdealTargetLabel = "Chi vuole budget e servizi"
                },
                new PropertyListing
                {
                    Id = Guid.Parse("20000000-0000-0000-0000-000000000003"),
                    Title = "Trilocale arredato",
                    Slug = "trilocale-arredato",
                    Location = "Cesena - Oltresavio",
                    DisplayLocation = "Cesena - Oltresavio",
                    Price = 750m,
                    Rooms = 3,
                    Bathrooms = 1,
                    SurfaceSquareMeters = 75,
                    Status = "Affitto",
                    Contract = "Affitto",
                    PropertyType = "Trilocale",
                    Zone = "Oltresavio",
                    FeatureKeys = "|terrace|parking|furnished|move-in-ready|near-services|near-public-transport|",
                    ImageUrl = "/medri-reference/assets/properties/property-07.jpg",
                    Latitude = 44.1285,
                    Longitude = 12.2370,
                    SortOrder = 3,
                    Address = "Via Savio 18, Cesena",
                    BedroomsLabel = "2",
                    FloorLabel = "Secondo piano",
                    GarageLabel = "Posto auto",
                    OutdoorSpaceLabel = "Balcone",
                    EnergyClass = "E",
                    AvailabilityLabel = "Da concordare",
                    HeatingLabel = "Autonomo",
                    RequiredWorksLabel = "Nessuno",
                    ConstructionYearLabel = "2010",
                    CondoFeesLabel = "Circa EUR 60 / mese",
                    BalconyLabel = "Si",
                    CellarLabel = "No",
                    NearbyServicesLabel = "Servizi principali vicini",
                    SummaryTitle = "Trilocale arredato",
                    SummaryParagraph1 = "Trilocale arredato in zona Oltresavio con spazi funzionali e canone mensile prevedibile.",
                    SummaryParagraph2 = "La casa permette un ingresso ordinato e conserva una gestione semplice per chi vuole tenere sotto controllo canone, spese e collegamenti.",
                    ReadinessNote = "Pronto per ingresso con arredi presenti.",
                    CostsNote = "Canone e spese contenute.",
                    ContextNote = "Oltresavio, comodo ai servizi principali.",
                    DecisionMarginNote = "Adatto a chi cerca un affitto ordinato.",
                    HumanFitNote = "Potrebbe essere adatto se cerchi una soluzione in affitto gia arredata.",
                    ZoneComparisonLabel = "Oltresavio, servizi principali vicini",
                    SurfaceRoomsComparisonLabel = "75 mq, 3 locali",
                    StatusWorksComparisonLabel = "Buono, pronto",
                    MainCompromise = "Meno superficie esterna",
                    AccessLabel = "Secondo piano",
                    ManagementCostsLabel = "Circa EUR 60 / mese",
                    EstimatedWorksLabel = "Nessun lavoro immediato",
                    EnergyCostsLabel = "Medi",
                    PersonalizationLabel = "Bassa, arredi presenti",
                    TransportLabel = "Fermate bus vicine",
                    PrivacyLabel = "Media",
                    NoiseLabel = "Media",
                    IdealTargetLabel = "Chi cerca affitto arredato"
                },
                new PropertyListing
                {
                    Id = Guid.Parse("20000000-0000-0000-0000-000000000004"),
                    Title = "Nuovo appartamento in classe A",
                    Slug = "nuovo-appartamento-in-classe-a",
                    Location = "Cesena - Diegaro",
                    DisplayLocation = "Cesena - Diegaro",
                    Price = 310000m,
                    Rooms = 4,
                    Bathrooms = 2,
                    SurfaceSquareMeters = 110,
                    Status = "Nuova costruzione",
                    Contract = "Nuove costruzioni",
                    PropertyType = "Appartamento",
                    Zone = "Diegaro",
                    FeatureKeys = "|terrace|elevator|move-in-ready|high-energy-class|",
                    ImageUrl = "/medri-reference/assets/properties/property-08.jpg",
                    Latitude = 44.1005,
                    Longitude = 12.2140,
                    SortOrder = 4,
                    Address = "Via Diegaro 8, Cesena",
                    BedroomsLabel = "3",
                    FloorLabel = "Secondo piano con ascensore",
                    GarageLabel = "Garage acquistabile a parte",
                    OutdoorSpaceLabel = "Terrazzo abitabile",
                    EnergyClass = "A",
                    AvailabilityLabel = "Consegna prevista a rogito",
                    HeatingLabel = "Pompa di calore",
                    RequiredWorksLabel = "Nessuno",
                    ConstructionYearLabel = "2025",
                    CondoFeesLabel = "Circa EUR 120 / mese",
                    BalconyLabel = "Si",
                    CellarLabel = "No",
                    NearbyServicesLabel = "Servizi principali in auto",
                    SummaryTitle = "Nuovo appartamento in classe A",
                    SummaryParagraph1 = "Appartamento di nuova costruzione con terrazzo abitabile e prestazioni energetiche elevate.",
                    SummaryParagraph2 = "Soluzione pronta e efficiente, con prezzo iniziale piu alto rispetto alle alternative usate.",
                    ReadinessNote = "Nuovo, pronto alla consegna.",
                    CostsNote = "Costi energetici bassi e piu prevedibili.",
                    ContextNote = "Diegaro, nuova area residenziale.",
                    DecisionMarginNote = "Valido se la priorita e ridurre lavori e consumi.",
                    HumanFitNote = "Potrebbe essere adatto se cerchi casa pronta, efficiente e con terrazzo.",
                    ZoneComparisonLabel = "Diegaro, nuova area residenziale",
                    SurfaceRoomsComparisonLabel = "110 mq, 4 locali",
                    StatusWorksComparisonLabel = "Nuovo, pronto",
                    MainCompromise = "Prezzo iniziale piu alto",
                    AccessLabel = "Secondo piano con ascensore",
                    ManagementCostsLabel = "Circa EUR 120 / mese",
                    EstimatedWorksLabel = "Nessun lavoro immediato",
                    EnergyCostsLabel = "Bassi e piu prevedibili",
                    PersonalizationLabel = "Bassa, finiture gia definite",
                    TransportLabel = "Collegamento buono ma meno centrale",
                    PrivacyLabel = "Media, nuovo condominio",
                    NoiseLabel = "Bassa, area recente",
                    IdealTargetLabel = "Chi cerca casa pronta e efficiente"
                },
                new PropertyListing
                {
                    Id = Guid.Parse("20000000-0000-0000-0000-000000000005"),
                    Title = "Casa indipendente",
                    Slug = "casa-indipendente",
                    Location = "Cesena - Case Finali",
                    DisplayLocation = "Cesena - Case Finali",
                    Price = 390000m,
                    Rooms = 5,
                    Bathrooms = 2,
                    SurfaceSquareMeters = 160,
                    Status = "Vendita",
                    Contract = "Vendita",
                    PropertyType = "Casa indipendente",
                    Zone = "Case Finali",
                    FeatureKeys = "|garden|garage|parking|move-in-ready|renovation-needed|",
                    ImageUrl = "/medri-reference/assets/properties/property-06.jpg",
                    Latitude = 44.1565,
                    Longitude = 12.2275,
                    SortOrder = 5,
                    Address = "Via Case Finali 21, Cesena",
                    BedroomsLabel = "3",
                    FloorLabel = "Terra + primo",
                    GarageLabel = "Garage",
                    OutdoorSpaceLabel = "Corte privata",
                    EnergyClass = "D",
                    AvailabilityLabel = "Da concordare",
                    HeatingLabel = "Autonomo",
                    RequiredWorksLabel = "Leggeri",
                    ConstructionYearLabel = "2001",
                    CondoFeesLabel = "Nessuna",
                    BalconyLabel = "Si",
                    CellarLabel = "Si",
                    NearbyServicesLabel = "Servizi principali in auto",
                    SummaryTitle = "Casa indipendente",
                    SummaryParagraph1 = "Casa indipendente con spazi generosi e corte privata.",
                    SummaryParagraph2 = "Gli spazi interni ed esterni permettono di valutare bene gestione quotidiana, interventi leggeri e margini di personalizzazione.",
                    ReadinessNote = "Abitabile con aggiornamenti leggeri.",
                    CostsNote = "Gestione autonoma senza spese condominiali.",
                    ContextNote = "Case Finali, contesto residenziale.",
                    DecisionMarginNote = "Adatta a chi cerca indipendenza.",
                    HumanFitNote = "Potrebbe essere adatta se cerchi spazi ampi e autonomia.",
                    ZoneComparisonLabel = "Case Finali, contesto residenziale",
                    SurfaceRoomsComparisonLabel = "160 mq, 5 locali",
                    StatusWorksComparisonLabel = "Buono, lavori leggeri",
                    MainCompromise = "Budget piu alto",
                    AccessLabel = "Ingresso indipendente",
                    ManagementCostsLabel = "Nessuna spesa condominiale",
                    EstimatedWorksLabel = "Finiture da aggiornare",
                    EnergyCostsLabel = "Medio-alti",
                    PersonalizationLabel = "Alta",
                    TransportLabel = "Servizi principali in auto",
                    PrivacyLabel = "Alta",
                    NoiseLabel = "Bassa",
                    IdealTargetLabel = "Famiglia che cerca indipendenza"
                },
                new PropertyListing
                {
                    Id = Guid.Parse("20000000-0000-0000-0000-000000000006"),
                    Title = "Bilocale vicino al centro",
                    Slug = "bilocale-vicino-al-centro",
                    Location = "Cesena - Cesena Nord",
                    DisplayLocation = "Cesena - Cesena Nord",
                    Price = 620m,
                    Rooms = 2,
                    Bathrooms = 1,
                    SurfaceSquareMeters = 58,
                    Status = "Affitto",
                    Contract = "Affitto",
                    PropertyType = "Bilocale",
                    Zone = "Cesena Nord",
                    FeatureKeys = "|terrace|move-in-ready|near-services|near-public-transport|",
                    ImageUrl = "/medri-reference/assets/properties/property-02.jpg",
                    Latitude = 44.1467,
                    Longitude = 12.2467,
                    SortOrder = 6,
                    Address = "Via Stazione 4, Cesena",
                    BedroomsLabel = "1",
                    FloorLabel = "Secondo piano",
                    GarageLabel = "No",
                    OutdoorSpaceLabel = "Balcone",
                    EnergyClass = "E",
                    AvailabilityLabel = "Libero subito",
                    HeatingLabel = "Autonomo",
                    RequiredWorksLabel = "Nessuno",
                    ConstructionYearLabel = "2008",
                    CondoFeesLabel = "Circa EUR 50 / mese",
                    BalconyLabel = "Si",
                    CellarLabel = "No",
                    NearbyServicesLabel = "Stazione e servizi vicini",
                    SummaryTitle = "Bilocale vicino al centro",
                    SummaryParagraph1 = "Bilocale in affitto vicino alla stazione e al centro.",
                    SummaryParagraph2 = "La posizione consente di usare stazione, centro e servizi senza dipendere sempre dall'auto, mantenendo un canone controllato.",
                    ReadinessNote = "Pronto per ingresso.",
                    CostsNote = "Canone e spese contenute.",
                    ContextNote = "Stazione, collegamenti vicini.",
                    DecisionMarginNote = "Adatto a chi privilegia mobilita e prezzo.",
                    HumanFitNote = "Potrebbe essere adatto se cerchi un affitto compatto e servito.",
                    ZoneComparisonLabel = "Stazione, collegamenti vicini",
                    SurfaceRoomsComparisonLabel = "58 mq, 2 locali",
                    StatusWorksComparisonLabel = "Buono, pronto",
                    MainCompromise = "Spazi ridotti",
                    AccessLabel = "Secondo piano",
                    ManagementCostsLabel = "Circa EUR 50 / mese",
                    EstimatedWorksLabel = "Nessun lavoro immediato",
                    EnergyCostsLabel = "Medi",
                    PersonalizationLabel = "Media",
                    TransportLabel = "Stazione raggiungibile a piedi",
                    PrivacyLabel = "Media",
                    NoiseLabel = "Media",
                    IdealTargetLabel = "Chi cerca affitto compatto"
                },
                new PropertyListing
                {
                    Id = Guid.Parse("20000000-0000-0000-0000-000000000007"),
                    Title = "Quadrilocale ristrutturato",
                    Slug = "quadrilocale-ristrutturato",
                    Location = "Cesena - Fiorenzuola",
                    DisplayLocation = "Cesena - Fiorenzuola",
                    Price = 248000m,
                    Rooms = 4,
                    Bathrooms = 2,
                    SurfaceSquareMeters = 102,
                    Status = "Vendita",
                    Contract = "Vendita",
                    PropertyType = "Quadrilocale",
                    Zone = "Fiorenzuola",
                    FeatureKeys = "|terrace|parking|move-in-ready|near-services|near-public-transport|",
                    ImageUrl = "/medri-reference/assets/properties/property-05.jpg",
                    Latitude = 44.1334,
                    Longitude = 12.2550,
                    SortOrder = 7,
                    Address = "Via Fiorenzuola 16, Cesena",
                    BedroomsLabel = "3",
                    FloorLabel = "Primo piano",
                    GarageLabel = "Posto auto",
                    OutdoorSpaceLabel = "Terrazzo",
                    EnergyClass = "C",
                    AvailabilityLabel = "Da concordare",
                    HeatingLabel = "Autonomo",
                    RequiredWorksLabel = "Nessuno",
                    ConstructionYearLabel = "2014",
                    CondoFeesLabel = "Circa EUR 90 / mese",
                    BalconyLabel = "Si",
                    CellarLabel = "Si",
                    NearbyServicesLabel = "Scuole e negozi",
                    SummaryTitle = "Quadrilocale ristrutturato",
                    SummaryParagraph1 = "Quadrilocale ristrutturato con terrazzo e spazi equilibrati.",
                    SummaryParagraph2 = "La distribuzione interna privilegia zona giorno, camere e terrazzo, con costi condominiali da verificare prima della proposta.",
                    ReadinessNote = "Ristrutturato, pronto.",
                    CostsNote = "Gestione condominiale prevedibile.",
                    ContextNote = "Fiorenzuola, zona residenziale servita.",
                    DecisionMarginNote = "Buon equilibrio tra metratura e prezzo.",
                    HumanFitNote = "Potrebbe essere adatto se cerchi una soluzione pronta con terrazzo.",
                    ZoneComparisonLabel = "Fiorenzuola, servizi vicini",
                    SurfaceRoomsComparisonLabel = "102 mq, 4 locali",
                    StatusWorksComparisonLabel = "Ristrutturato, pronto",
                    MainCompromise = "Meno indipendenza",
                    AccessLabel = "Primo piano",
                    ManagementCostsLabel = "Circa EUR 90 / mese",
                    EstimatedWorksLabel = "Nessun lavoro immediato",
                    EnergyCostsLabel = "Medi",
                    PersonalizationLabel = "Media",
                    TransportLabel = "Collegamenti urbani vicini",
                    PrivacyLabel = "Media",
                    NoiseLabel = "Bassa",
                    IdealTargetLabel = "Chi cerca casa pronta"
                },
                new PropertyListing
                {
                    Id = Guid.Parse("20000000-0000-0000-0000-000000000008"),
                    Title = "Attico con terrazzo",
                    Slug = "attico-con-terrazzo",
                    Location = "Cesena - Centro storico",
                    DisplayLocation = "Cesena - Centro storico",
                    Price = 335000m,
                    Rooms = 3,
                    Bathrooms = 2,
                    SurfaceSquareMeters = 88,
                    Status = "Vendita",
                    Contract = "Vendita",
                    PropertyType = "Attico",
                    Zone = "Centro storico",
                    FeatureKeys = "|terrace|move-in-ready|high-energy-class|near-services|near-public-transport|",
                    ImageUrl = "/medri-reference/assets/properties/property-04.jpg",
                    Latitude = 44.1391,
                    Longitude = 12.2464,
                    SortOrder = 8,
                    Address = "Via Centro Storico 9, Cesena",
                    BedroomsLabel = "2",
                    FloorLabel = "Ultimo piano",
                    GarageLabel = "No",
                    OutdoorSpaceLabel = "Terrazzo",
                    EnergyClass = "B",
                    AvailabilityLabel = "Da concordare",
                    HeatingLabel = "Autonomo",
                    RequiredWorksLabel = "Nessuno",
                    ConstructionYearLabel = "2018",
                    CondoFeesLabel = "Circa EUR 100 / mese",
                    BalconyLabel = "Si",
                    CellarLabel = "No",
                    NearbyServicesLabel = "Centro storico",
                    SummaryTitle = "Attico con terrazzo",
                    SummaryParagraph1 = "Attico luminoso con terrazzo in posizione centrale.",
                    SummaryParagraph2 = "Il terrazzo e la posizione centrale sono i punti da pesare insieme a spese, piano e gestione degli spazi accessori.",
                    ReadinessNote = "Pronto e ben mantenuto.",
                    CostsNote = "Spese condominiali prevedibili.",
                    ContextNote = "Centro storico, servizi a piedi.",
                    DecisionMarginNote = "Valido se la priorita e la posizione centrale.",
                    HumanFitNote = "Potrebbe essere adatto se cerchi luminosita, terrazzo e centro.",
                    ZoneComparisonLabel = "Centro storico, servizi a piedi",
                    SurfaceRoomsComparisonLabel = "88 mq, 3 locali",
                    StatusWorksComparisonLabel = "Buono, pronto",
                    MainCompromise = "Assenza garage",
                    AccessLabel = "Ultimo piano",
                    ManagementCostsLabel = "Circa EUR 100 / mese",
                    EstimatedWorksLabel = "Nessun lavoro immediato",
                    EnergyCostsLabel = "Medi",
                    PersonalizationLabel = "Media",
                    TransportLabel = "Centro e servizi a piedi",
                    PrivacyLabel = "Media",
                    NoiseLabel = "Media",
                    IdealTargetLabel = "Chi cerca centro e terrazzo"
                }
            };

            foreach (var property in propertyListings)
            {
                property.PublicationStatus = "Published";
            }

            var operationalListings = new[]
            {
                new PropertyListing
                {
                    Id = Guid.Parse("20000000-0000-0000-0000-000000000101"),
                    InternalReference = "IM-342",
                    PublicationStatus = "Incomplete",
                    CompletionPercent = 78,
                    MissingItems = "Foto esterne, nota impianti",
                    AssignedAgencyUserId = chiaraAgencyUserId,
                    Title = "Trilocale ristrutturato",
                    Slug = "bozza-im-342",
                    Location = "Cesena",
                    DisplayLocation = "Cesena - Centro",
                    Price = 235000m,
                    Rooms = 3,
                    Bathrooms = 1,
                    SurfaceSquareMeters = 88,
                    Status = "Vendita",
                    Contract = "Vendita",
                    PropertyType = "Trilocale",
                    Zone = "Centro",
                    FeatureKeys = "|draft|",
                    ImageUrl = "/medri-reference/assets/properties/property-06.jpg",
                    SortOrder = 201
                },
                new PropertyListing
                {
                    Id = Guid.Parse("20000000-0000-0000-0000-000000000102"),
                    InternalReference = "IM-329",
                    PublicationStatus = "NeedsUpdate",
                    CompletionPercent = 76,
                    MissingItems = "Prezzo, disponibilita",
                    AssignedAgencyUserId = martinaAgencyUserId,
                    Title = "Appartamento con terrazzo",
                    Slug = "bozza-im-329",
                    Location = "Cesena",
                    DisplayLocation = "Cesena - Fiorenzuola",
                    Price = 260000m,
                    Rooms = 4,
                    Bathrooms = 2,
                    SurfaceSquareMeters = 105,
                    Status = "Vendita",
                    Contract = "Vendita",
                    PropertyType = "Appartamento",
                    Zone = "Fiorenzuola",
                    FeatureKeys = "|draft|",
                    ImageUrl = "/medri-reference/assets/properties/property-05.jpg",
                    SortOrder = 202
                },
                new PropertyListing
                {
                    Id = Guid.Parse("20000000-0000-0000-0000-000000000103"),
                    InternalReference = "IM-318",
                    PublicationStatus = "Ready",
                    CompletionPercent = 100,
                    MissingItems = "Nessun dato mancante",
                    AssignedAgencyUserId = lorenzoAgencyUserId,
                    Title = "Villetta a schiera",
                    Slug = "bozza-im-318",
                    Location = "Cesena",
                    DisplayLocation = "Cesena - S. Egidio",
                    Price = 285000m,
                    Rooms = 4,
                    Bathrooms = 2,
                    SurfaceSquareMeters = 120,
                    Status = "Vendita",
                    Contract = "Vendita",
                    PropertyType = "Villetta",
                    Zone = "Sant'Egidio",
                    FeatureKeys = "|draft|",
                    ImageUrl = "/medri-reference/assets/properties/property-03.jpg",
                    SortOrder = 203
                },
                new PropertyListing
                {
                    Id = Guid.Parse("20000000-0000-0000-0000-000000001042"),
                    InternalReference = "IM-1042",
                    PublicationStatus = "Incomplete",
                    CompletionPercent = 82,
                    MissingItems = "Planimetria, nota lavori",
                    AssignedAgencyUserId = chiaraAgencyUserId,
                    Title = "Villetta a schiera",
                    Slug = "admin-im-1042",
                    Location = "Cesena",
                    DisplayLocation = "Cesena - S. Egidio",
                    Price = 285000m,
                    Rooms = 4,
                    Bathrooms = 2,
                    SurfaceSquareMeters = 120,
                    Status = "Vendita",
                    Contract = "Vendita",
                    PropertyType = "Villetta",
                    ListingCategory = "Abitazione",
                    Zone = "Sant'Egidio",
                    FeatureKeys = "|draft|",
                    ImageUrl = "/medri-reference/assets/properties/property-03.jpg",
                    SortOrder = 10,
                    Address = "Via del Tiglio 18",
                    BedroomsLabel = "3 camere",
                    FloorLabel = "Terra e primo",
                    ElevatorLabel = "No",
                    GarageLabel = "Garage e posto auto",
                    OutdoorSpaceLabel = "Giardino privato",
                    EnergyClass = "D",
                    RequiredWorksLabel = "Buono, lavori leggeri",
                    CondoFeesLabel = "Nessuna",
                    SummaryParagraph1 = "Villetta a schiera in contesto residenziale tranquillo, con spazi adatti a una famiglia, ingresso indipendente e giardino privato.",
                    AccessLabel = "Ingresso indipendente e giardino privato",
                    ContextNote = "Zona residenziale tranquilla",
                    MainCompromise = "Lavori leggeri da pianificare",
                    NearbyServicesLabel = "Garage e posto auto",
                    DecisionMarginNote = "",
                    CostsNote = "",
                    HumanFitNote = "Cerchi indipendenza, giardino e una casa familiare senza arrivare al costo di una villa indipendente."
                },
                new PropertyListing
                {
                    Id = Guid.Parse("20000000-0000-0000-0000-000000001039"),
                    InternalReference = "IM-1039",
                    PublicationStatus = "Incomplete",
                    CompletionPercent = 61,
                    MissingItems = "Foto zona giorno, APE, spese",
                    AssignedAgencyUserId = martinaAgencyUserId,
                    Title = "Appartamento con giardino",
                    Slug = "admin-im-1039",
                    Location = "Cesena",
                    DisplayLocation = "Cesena - Centro",
                    Price = 210000m,
                    Rooms = 3,
                    Bathrooms = 1,
                    SurfaceSquareMeters = 95,
                    Status = "Vendita",
                    Contract = "Vendita",
                    PropertyType = "Appartamento",
                    Zone = "Centro",
                    FeatureKeys = "|draft|",
                    ImageUrl = "/medri-reference/assets/properties/property-01.jpg",
                    SortOrder = 11
                },
                new PropertyListing
                {
                    Id = Guid.Parse("20000000-0000-0000-0000-000000001046"),
                    InternalReference = "IM-1046",
                    PublicationStatus = "Ready",
                    CompletionPercent = 100,
                    MissingItems = "Nessun dato mancante",
                    AssignedAgencyUserId = lorenzoAgencyUserId,
                    Title = "Nuovo appartamento in classe A",
                    Slug = "admin-im-1046",
                    Location = "Cesena",
                    DisplayLocation = "Cesena - Diegaro",
                    Price = 310000m,
                    Rooms = 4,
                    Bathrooms = 2,
                    SurfaceSquareMeters = 110,
                    Status = "Vendita",
                    Contract = "Vendita",
                    PropertyType = "Appartamento",
                    Zone = "Diegaro",
                    FeatureKeys = "|draft|",
                    ImageUrl = "/medri-reference/assets/properties/property-08.jpg",
                    SortOrder = 12
                },
                new PropertyListing
                {
                    Id = Guid.Parse("20000000-0000-0000-0000-000000000208"),
                    InternalReference = "AF-208",
                    PublicationStatus = "NeedsUpdate",
                    CompletionPercent = 74,
                    MissingItems = "Canone, disponibilita, regole",
                    AssignedAgencyUserId = marcoAgencyUserId,
                    Title = "Trilocale arredato",
                    Slug = "admin-af-208",
                    Location = "Cesena",
                    DisplayLocation = "Cesena - Oltresavio",
                    Price = 750m,
                    Rooms = 3,
                    Bathrooms = 1,
                    SurfaceSquareMeters = 75,
                    Status = "Affitto",
                    Contract = "Affitto",
                    PropertyType = "Trilocale",
                    Zone = "Oltresavio",
                    FeatureKeys = "|draft|",
                    ImageUrl = "/medri-reference/assets/properties/property-07.jpg",
                    SortOrder = 13
                },
                new PropertyListing
                {
                    Id = Guid.Parse("20000000-0000-0000-0000-000000001050"),
                    InternalReference = "IM-1050",
                    PublicationStatus = "Ready",
                    CompletionPercent = 100,
                    MissingItems = "Nessun dato mancante",
                    AssignedAgencyUserId = chiaraAgencyUserId,
                    Title = "Bilocale in centro",
                    Slug = "admin-im-1050",
                    Location = "Cesena",
                    DisplayLocation = "Cesena - Centro",
                    Price = 165000m,
                    Rooms = 2,
                    Bathrooms = 1,
                    SurfaceSquareMeters = 62,
                    Status = "Vendita",
                    Contract = "Vendita",
                    PropertyType = "Bilocale",
                    Zone = "Centro",
                    FeatureKeys = "|draft|",
                    ImageUrl = "/medri-reference/assets/properties/property-02.jpg",
                    SortOrder = 30
                },
                new PropertyListing
                {
                    Id = Guid.Parse("20000000-0000-0000-0000-000000001051"),
                    InternalReference = "IM-1051",
                    PublicationStatus = "Ready",
                    CompletionPercent = 100,
                    MissingItems = "Nessun dato mancante",
                    AssignedAgencyUserId = martinaAgencyUserId,
                    Title = "Casa indipendente",
                    Slug = "admin-im-1051",
                    Location = "Cesena",
                    DisplayLocation = "Cesena - Case Finali",
                    Price = 390000m,
                    Rooms = 5,
                    Bathrooms = 2,
                    SurfaceSquareMeters = 160,
                    Status = "Vendita",
                    Contract = "Vendita",
                    PropertyType = "Casa indipendente",
                    Zone = "Case Finali",
                    FeatureKeys = "|draft|",
                    ImageUrl = "/medri-reference/assets/properties/property-06.jpg",
                    SortOrder = 31
                },
                new PropertyListing
                {
                    Id = Guid.Parse("20000000-0000-0000-0000-000000001052"),
                    InternalReference = "IM-1052",
                    PublicationStatus = "Ready",
                    CompletionPercent = 100,
                    MissingItems = "Nessun dato mancante",
                    AssignedAgencyUserId = lorenzoAgencyUserId,
                    Title = "Loft zona stazione",
                    Slug = "admin-im-1052",
                    Location = "Cesena",
                    DisplayLocation = "Cesena - Stazione",
                    Price = 198000m,
                    Rooms = 2,
                    Bathrooms = 1,
                    SurfaceSquareMeters = 70,
                    Status = "Vendita",
                    Contract = "Vendita",
                    PropertyType = "Loft",
                    Zone = "Stazione",
                    FeatureKeys = "|draft|",
                    ImageUrl = "/medri-reference/assets/properties/property-04.jpg",
                    SortOrder = 32
                },
                new PropertyListing
                {
                    Id = Guid.Parse("20000000-0000-0000-0000-000000001053"),
                    InternalReference = "IM-1053",
                    PublicationStatus = "Ready",
                    CompletionPercent = 100,
                    MissingItems = "Nessun dato mancante",
                    AssignedAgencyUserId = marcoAgencyUserId,
                    Title = "Porzione con corte",
                    Slug = "admin-im-1053",
                    Location = "Cesena",
                    DisplayLocation = "Cesena - San Mauro",
                    Price = 248000m,
                    Rooms = 4,
                    Bathrooms = 2,
                    SurfaceSquareMeters = 116,
                    Status = "Vendita",
                    Contract = "Vendita",
                    PropertyType = "Porzione",
                    Zone = "San Mauro",
                    FeatureKeys = "|draft|",
                    ImageUrl = "/medri-reference/assets/properties/property-05.jpg",
                    SortOrder = 33
                },
                new PropertyListing
                {
                    Id = Guid.Parse("20000000-0000-0000-0000-000000001054"),
                    InternalReference = "AF-209",
                    PublicationStatus = "Ready",
                    CompletionPercent = 100,
                    MissingItems = "Nessun dato mancante",
                    AssignedAgencyUserId = martinaAgencyUserId,
                    Title = "Monolocale arredato",
                    Slug = "admin-af-209",
                    Location = "Cesena",
                    DisplayLocation = "Cesena - Centro",
                    Price = 580m,
                    Rooms = 1,
                    Bathrooms = 1,
                    SurfaceSquareMeters = 42,
                    Status = "Affitto",
                    Contract = "Affitto",
                    PropertyType = "Monolocale",
                    Zone = "Centro",
                    FeatureKeys = "|draft|",
                    ImageUrl = "/medri-reference/assets/properties/property-07.jpg",
                    SortOrder = 34
                },
                new PropertyListing
                {
                    Id = Guid.Parse("20000000-0000-0000-0000-000000001055"),
                    InternalReference = "IM-1055",
                    PublicationStatus = "NeedsUpdate",
                    CompletionPercent = 72,
                    MissingItems = "Prezzo, foto bagno",
                    AssignedAgencyUserId = chiaraAgencyUserId,
                    Title = "Quadrilocale luminoso",
                    Slug = "admin-im-1055",
                    Location = "Cesena",
                    DisplayLocation = "Cesena - Ponte Abbadesse",
                    Price = 255000m,
                    Rooms = 4,
                    Bathrooms = 2,
                    SurfaceSquareMeters = 108,
                    Status = "Vendita",
                    Contract = "Vendita",
                    PropertyType = "Quadrilocale",
                    Zone = "Ponte Abbadesse",
                    FeatureKeys = "|draft|",
                    ImageUrl = "/medri-reference/assets/properties/property-02.jpg",
                    SortOrder = 40
                },
                new PropertyListing
                {
                    Id = Guid.Parse("20000000-0000-0000-0000-000000001056"),
                    InternalReference = "AF-210",
                    PublicationStatus = "NeedsUpdate",
                    CompletionPercent = 68,
                    MissingItems = "Disponibilita, regole animali",
                    AssignedAgencyUserId = marcoAgencyUserId,
                    Title = "Bilocale arredato",
                    Slug = "admin-af-210",
                    Location = "Cesena",
                    DisplayLocation = "Cesena - Fiorita",
                    Price = 690m,
                    Rooms = 2,
                    Bathrooms = 1,
                    SurfaceSquareMeters = 58,
                    Status = "Affitto",
                    Contract = "Affitto",
                    PropertyType = "Bilocale",
                    Zone = "Fiorita",
                    FeatureKeys = "|draft|",
                    ImageUrl = "/medri-reference/assets/properties/property-01.jpg",
                    SortOrder = 41
                }
            };

            var generatedListingBlueprints = new[]
            {
                new
                {
                    SaleTitle = "Trilocale con balcone",
                    RentTitle = "Bilocale arredato in centro",
                    DisplayLocation = "Cesena - Centro",
                    Zone = "Centro",
                    PropertyType = "Trilocale",
                    FeatureKeys = "|terrace|move-in-ready|near-services|near-public-transport|",
                    ImageUrl = "/medri-reference/assets/properties/property-01.jpg",
                    Latitude = 44.1396d,
                    Longitude = 12.2431d,
                    Address = "Via Mura Barriera Ponente 12, Cesena",
                    Rooms = 3,
                    Bathrooms = 1,
                    Surface = 86,
                    EnergyClass = "C"
                },
                new
                {
                    SaleTitle = "Appartamento con terrazzo",
                    RentTitle = "Trilocale arredato Oltresavio",
                    DisplayLocation = "Cesena - Oltresavio",
                    Zone = "Oltresavio",
                    PropertyType = "Appartamento",
                    FeatureKeys = "|terrace|parking|move-in-ready|near-services|",
                    ImageUrl = "/medri-reference/assets/properties/property-07.jpg",
                    Latitude = 44.1285d,
                    Longitude = 12.2370d,
                    Address = "Via Savio 18, Cesena",
                    Rooms = 3,
                    Bathrooms = 1,
                    Surface = 78,
                    EnergyClass = "E"
                },
                new
                {
                    SaleTitle = "Villetta con giardino",
                    RentTitle = "Porzione con corte privata",
                    DisplayLocation = "Cesena - S. Egidio",
                    Zone = "Sant'Egidio",
                    PropertyType = "Villetta",
                    FeatureKeys = "|garden|garage|parking|renovation-needed|near-services|",
                    ImageUrl = "/medri-reference/assets/properties/property-03.jpg",
                    Latitude = 44.1530d,
                    Longitude = 12.2860d,
                    Address = "Via S. Egidio 42, Cesena",
                    Rooms = 4,
                    Bathrooms = 2,
                    Surface = 118,
                    EnergyClass = "D"
                },
                new
                {
                    SaleTitle = "Quadrilocale in classe A",
                    RentTitle = "Appartamento recente arredato",
                    DisplayLocation = "Cesena - Diegaro",
                    Zone = "Diegaro",
                    PropertyType = "Appartamento",
                    FeatureKeys = "|terrace|elevator|move-in-ready|high-energy-class|",
                    ImageUrl = "/medri-reference/assets/properties/property-08.jpg",
                    Latitude = 44.1005d,
                    Longitude = 12.2140d,
                    Address = "Via Diegaro 8, Cesena",
                    Rooms = 4,
                    Bathrooms = 2,
                    Surface = 106,
                    EnergyClass = "A"
                },
                new
                {
                    SaleTitle = "Casa indipendente con corte",
                    RentTitle = "Casa arredata con giardino",
                    DisplayLocation = "Cesena - Case Finali",
                    Zone = "Case Finali",
                    PropertyType = "Casa indipendente",
                    FeatureKeys = "|garden|garage|parking|renovation-needed|",
                    ImageUrl = "/medri-reference/assets/properties/property-06.jpg",
                    Latitude = 44.1565d,
                    Longitude = 12.2275d,
                    Address = "Via Case Finali 21, Cesena",
                    Rooms = 5,
                    Bathrooms = 2,
                    Surface = 158,
                    EnergyClass = "D"
                },
                new
                {
                    SaleTitle = "Bilocale zona stazione",
                    RentTitle = "Bilocale pronto zona stazione",
                    DisplayLocation = "Cesena - Stazione",
                    Zone = "Stazione",
                    PropertyType = "Bilocale",
                    FeatureKeys = "|terrace|move-in-ready|near-services|near-public-transport|",
                    ImageUrl = "/medri-reference/assets/properties/property-02.jpg",
                    Latitude = 44.1467d,
                    Longitude = 12.2467d,
                    Address = "Via Stazione 4, Cesena",
                    Rooms = 2,
                    Bathrooms = 1,
                    Surface = 64,
                    EnergyClass = "E"
                },
                new
                {
                    SaleTitle = "Porzione con terrazzo",
                    RentTitle = "Trilocale con posto auto",
                    DisplayLocation = "Cesena - Fiorenzuola",
                    Zone = "Fiorenzuola",
                    PropertyType = "Porzione",
                    FeatureKeys = "|terrace|parking|move-in-ready|near-services|",
                    ImageUrl = "/medri-reference/assets/properties/property-05.jpg",
                    Latitude = 44.1334d,
                    Longitude = 12.2550d,
                    Address = "Via Fiorenzuola 16, Cesena",
                    Rooms = 4,
                    Bathrooms = 2,
                    Surface = 104,
                    EnergyClass = "C"
                },
                new
                {
                    SaleTitle = "Appartamento ultimo piano",
                    RentTitle = "Mansarda arredata in centro",
                    DisplayLocation = "Cesena - Centro storico",
                    Zone = "Centro storico",
                    PropertyType = "Appartamento",
                    FeatureKeys = "|terrace|move-in-ready|high-energy-class|near-services|",
                    ImageUrl = "/medri-reference/assets/properties/property-04.jpg",
                    Latitude = 44.1391d,
                    Longitude = 12.2464d,
                    Address = "Via Centro Storico 9, Cesena",
                    Rooms = 3,
                    Bathrooms = 1,
                    Surface = 82,
                    EnergyClass = "B"
                }
            };

            var publishedAdminListings = Enumerable.Range(1, 42)
                .Select(index =>
                {
                    var listingBlueprint = generatedListingBlueprints[(index - 1) % generatedListingBlueprints.Length];
                    var isRent = index % 5 == 0;
                    var contract = isRent ? "Affitto" : "Vendita";
                    var title = isRent ? listingBlueprint.RentTitle : listingBlueprint.SaleTitle;
                    var price = isRent
                        ? 540m + (index % 9) * 35m
                        : 165000m + index * 4200m;

                    return new PropertyListing
                    {
                        Id = Guid.Parse($"20000000-0000-0000-0000-{300 + index:000000000000}"),
                        InternalReference = isRent ? $"AF-P{index:000}" : $"IM-P{index:000}",
                        PublicationStatus = "Published",
                        CompletionPercent = 100,
                        MissingItems = "Nessun dato mancante",
                        FeaturedSortOrder = index <= 3 ? index : (int?)null,
                        AssignedAgencyUserId = index % 4 == 0
                            ? marcoAgencyUserId
                            : index % 3 == 0
                                ? lorenzoAgencyUserId
                                : index % 2 == 0
                                    ? martinaAgencyUserId
                                    : chiaraAgencyUserId,
                        Title = title,
                        Slug = $"admin-pubblicato-{index:00}",
                        Location = listingBlueprint.DisplayLocation,
                        DisplayLocation = listingBlueprint.DisplayLocation,
                        Price = price,
                        Rooms = isRent ? Math.Max(1, listingBlueprint.Rooms - 1) : listingBlueprint.Rooms,
                        Bathrooms = listingBlueprint.Bathrooms,
                        SurfaceSquareMeters = isRent ? Math.Max(38, listingBlueprint.Surface - 18) : listingBlueprint.Surface + index % 6,
                        Status = contract,
                        Contract = contract,
                        PropertyType = listingBlueprint.PropertyType,
                        Zone = listingBlueprint.Zone,
                        FeatureKeys = listingBlueprint.FeatureKeys,
                        ImageUrl = listingBlueprint.ImageUrl,
                        Latitude = listingBlueprint.Latitude + ((index % 3) - 1) * 0.0012d,
                        Longitude = listingBlueprint.Longitude + ((index % 4) - 1.5d) * 0.0012d,
                        Address = listingBlueprint.Address,
                        BedroomsLabel = isRent
                            ? Math.Max(1, listingBlueprint.Rooms - 2).ToString()
                            : Math.Max(1, listingBlueprint.Rooms - 1).ToString(),
                        FloorLabel = index % 3 == 0 ? "Piano alto" : "Piano intermedio",
                        GarageLabel = index % 2 == 0 ? "Posto auto" : "Garage",
                        OutdoorSpaceLabel = listingBlueprint.FeatureKeys.Contains("|garden|") ? "Giardino" : "Balcone",
                        EnergyClass = listingBlueprint.EnergyClass,
                        AvailabilityLabel = isRent ? "Libero da concordare" : "Disponibile a rogito",
                        HeatingLabel = "Autonomo",
                        RequiredWorksLabel = index % 4 == 0 ? "Aggiornamenti leggeri" : "Buono",
                        SummaryTitle = title,
                        SummaryParagraph1 = string.Empty,
                        SummaryParagraph2 = string.Empty,
                        HumanFitNote = string.Empty,
                        SortOrder = 300 + index,
                        UpdatedAtUtc = seedDate.AddDays(-index)
                    };
                })
                .ToArray();

            var reservedAdminListings = Enumerable.Range(1, 5)
                .Select(index =>
                {
                    var listingBlueprint = generatedListingBlueprints[(index + 2) % generatedListingBlueprints.Length];

                    return new PropertyListing
                    {
                        Id = Guid.Parse($"20000000-0000-0000-0000-{500 + index:000000000000}"),
                        InternalReference = $"IM-R{index:000}",
                        PublicationStatus = "Reserved",
                        CompletionPercent = 90,
                        MissingItems = "Verifica finale",
                        AssignedAgencyUserId = index % 2 == 0 ? martinaAgencyUserId : chiaraAgencyUserId,
                        Title = listingBlueprint.SaleTitle,
                        Slug = $"admin-riservato-{index:00}",
                        Location = listingBlueprint.DisplayLocation,
                        DisplayLocation = listingBlueprint.DisplayLocation,
                        Price = 210000m + index * 2500m,
                        Rooms = listingBlueprint.Rooms,
                        Bathrooms = listingBlueprint.Bathrooms,
                        SurfaceSquareMeters = listingBlueprint.Surface,
                        Status = "Vendita",
                        Contract = "Vendita",
                        PropertyType = listingBlueprint.PropertyType,
                        Zone = listingBlueprint.Zone,
                        FeatureKeys = listingBlueprint.FeatureKeys,
                        ImageUrl = listingBlueprint.ImageUrl,
                        Latitude = listingBlueprint.Latitude,
                        Longitude = listingBlueprint.Longitude,
                        Address = listingBlueprint.Address,
                        EnergyClass = listingBlueprint.EnergyClass,
                        SortOrder = 500 + index
                    };
                })
                .ToArray();

            foreach (var item in operationalListings
                .Concat(reservedAdminListings)
                .Select((listing, index) => new { listing, index }))
            {
                var listingBlueprint = generatedListingBlueprints
                    .FirstOrDefault(candidate =>
                        item.listing.DisplayLocation?.IndexOf(candidate.Zone, StringComparison.OrdinalIgnoreCase) >= 0) ??
                    generatedListingBlueprints[item.index % generatedListingBlueprints.Length];

                if (item.listing.Latitude == 0d && item.listing.Longitude == 0d)
                {
                    item.listing.Latitude = listingBlueprint.Latitude + ((item.index % 3) - 1) * 0.001d;
                    item.listing.Longitude = listingBlueprint.Longitude + ((item.index % 4) - 1.5d) * 0.001d;
                }

                if (string.IsNullOrWhiteSpace(item.listing.Address))
                {
                    item.listing.Address = listingBlueprint.Address;
                }

                if (string.IsNullOrWhiteSpace(item.listing.EnergyClass))
                {
                    item.listing.EnergyClass = listingBlueprint.EnergyClass;
                }
            }

            CompleteSeedListingDetails(
                propertyListings
                    .Concat(operationalListings)
                    .Concat(publishedAdminListings)
                    .Concat(reservedAdminListings)
                    .ToArray());

            context.AgencyUsers.AddRange(
                new AgencyUser
                {
                    Id = agencyUserId,
                    DisplayName = "Segreteria Medri",
                    Email = "segreteria.medri@example.test",
                    Role = AgencyUserRoles.Advisor,
                    IsSystemSeed = true
                },
                new AgencyUser
                {
                    Id = martinaAgencyUserId,
                    DisplayName = "Martina Ricci",
                    Email = "martina.ricci@example.test",
                    Role = AgencyUserRoles.Advisor
                },
                new AgencyUser
                {
                    Id = chiaraAgencyUserId,
                    DisplayName = "Chiara Medri",
                    Email = "chiara.medri@email.it",
                    Role = AgencyUserRoles.Manager
                },
                new AgencyUser
                {
                    Id = lorenzoAgencyUserId,
                    DisplayName = "Lorenzo Bassi",
                    Email = "lorenzo.bassi@example.test",
                    Role = AgencyUserRoles.Operator
                },
                new AgencyUser
                {
                    Id = marcoAgencyUserId,
                    DisplayName = "Marco Guidi",
                    Email = "marco.guidi@example.test",
                    Role = AgencyUserRoles.Operator
                });

            context.PropertyListings.AddRange(propertyListings);
            context.PropertyListings.AddRange(operationalListings);
            context.PropertyListings.AddRange(publishedAdminListings);
            context.PropertyListings.AddRange(reservedAdminListings);

            context.PropertyMedia.AddRange(propertyListings.Select((listing, index) => new PropertyMedia
            {
                Id = Guid.Parse($"21000000-0000-0000-0000-{index + 1:000000000000}"),
                PropertyListingId = listing.Id,
                Url = listing.ImageUrl,
                AltText = listing.Title,
                SortOrder = 1
            }));

            var adminPropertyMediaTitles = new[]
            {
                "Copertina",
                "Esterno",
                "Soggiorno",
                "Cucina",
                "Giardino",
                "Camera matrimoniale",
                "Bagno",
                "Camera singola"
            };

            context.PropertyMedia.AddRange(adminPropertyMediaTitles.Select((title, index) => new PropertyMedia
            {
                Id = Guid.Parse($"21000000-0000-0000-0000-{100 + index + 1:000000000000}"),
                PropertyListingId = Guid.Parse("20000000-0000-0000-0000-000000001042"),
                Url = "/medri-reference/assets/properties/property-03.jpg",
                AltText = title,
                SortOrder = index + 1
            }));

            AddGalleryMediaForListings(
                context,
                propertyListings
                    .Concat(operationalListings)
                    .Concat(publishedAdminListings)
                    .Concat(reservedAdminListings)
                    .ToArray());

            context.FavoriteProperties.AddRange(
                new FavoriteProperty
                {
                    Id = Guid.Parse("40000000-0000-0000-0000-000000000201"),
                    UserId = clientUserId,
                    PropertyListingId = Guid.Parse("20000000-0000-0000-0000-000000000002"),
                    CreatedAtUtc = seedDate.AddDays(-3)
                },
                new FavoriteProperty
                {
                    Id = Guid.Parse("40000000-0000-0000-0000-000000000202"),
                    UserId = clientUserId,
                    PropertyListingId = Guid.Parse("20000000-0000-0000-0000-000000000007"),
                    CreatedAtUtc = seedDate.AddDays(-2)
                },
                new FavoriteProperty
                {
                    Id = Guid.Parse("40000000-0000-0000-0000-000000000203"),
                    UserId = clientUserId,
                    PropertyListingId = propertyListingId,
                    CreatedAtUtc = seedDate.AddDays(-1)
                });

            context.ClientNotificationPreferences.AddRange(
                new ClientNotificationPreference
                {
                    Id = Guid.Parse("40000000-0000-0000-0000-000000000301"),
                    UserId = clientUserId,
                    Category = ClientNotificationCategories.Requests,
                    IsActive = true,
                    IsDaily = true,
                    IsWeekly = false,
                    UpdatedAtUtc = seedDate
                },
                new ClientNotificationPreference
                {
                    Id = Guid.Parse("40000000-0000-0000-0000-000000000302"),
                    UserId = clientUserId,
                    Category = ClientNotificationCategories.Favorites,
                    IsActive = true,
                    IsDaily = true,
                    IsWeekly = false,
                    UpdatedAtUtc = seedDate
                },
                new ClientNotificationPreference
                {
                    Id = Guid.Parse("40000000-0000-0000-0000-000000000303"),
                    UserId = clientUserId,
                    Category = ClientNotificationCategories.SavedSearches,
                    IsActive = false,
                    IsDaily = true,
                    IsWeekly = false,
                    UpdatedAtUtc = seedDate
                });

            context.Leads.Add(new Lead
            {
                Id = leadId,
                FullName = "Lead seed tecnico",
                Email = "lead.seed@example.test",
                Phone = "+390000000000",
                SourceChannel = "Seed",
                WorkflowStatus = "Qualified",
                CreatedAtUtc = seedDate
            });

            context.Leads.AddRange(
                new Lead
                {
                    Id = clientBuyLeadId,
                    ClientUserId = clientUserId,
                    PublicReference = "RQ-2047",
                    FullName = "Elena Gori",
                    Email = "elena.gori@email.it",
                    Phone = "347 9988776",
                    SourceChannel = "Lead convertito",
                    RequestType = "Buy",
                    WorkflowStatus = "Qualified",
                    AssignedAgencyUserId = martinaAgencyUserId,
                    Notes = "3 immobili proposti",
                    CreatedAtUtc = seedDate.AddDays(-2)
                },
                new Lead
                {
                    Id = clientValuationLeadId,
                    ClientUserId = clientUserId,
                    PublicReference = "RQ-2034",
                    FullName = "Elena Gori",
                    Email = "elena.gori@email.it",
                    Phone = "333 2034000",
                    SourceChannel = "Public lead intake",
                    RequestType = "Valuation",
                    WorkflowStatus = "Qualified",
                    Notes = "Da ricontattare",
                    CreatedAtUtc = seedDate.AddDays(-8)
                },
                new Lead
                {
                    Id = paoloLeadId,
                    InternalReference = "LD-1184",
                    WorkflowStatus = "New",
                    QualificationPercent = 45,
                    NextAction = "Richiamare per zona, tempi e motivo vendita",
                    FullName = "Paolo Rinaldi",
                    Email = "paolo.r@email.it",
                    Phone = "333 1234567",
                    SourceChannel = "Telefono",
                    RequestType = "Valuation",
                    Notes = "Ha chiamato per capire se sia il momento giusto per vendere un immobile familiare. Sembra prudente, vuole prima parlare con un referente esperto e non desidera una stima automatica.",
                    CreatedAtUtc = seedDate.AddDays(-1)
                },
                new Lead
                {
                    Id = niccoloLeadId,
                    InternalReference = "LD-1176",
                    WorkflowStatus = "InContact",
                    QualificationPercent = 64,
                    NextAction = "Chiedere budget massimo e garanzie",
                    AssignedAgencyUserId = lorenzoAgencyUserId,
                    FullName = "Niccolo Fabbri",
                    Email = "niccolo.f@email.it",
                    SourceChannel = "Email",
                    RequestType = "Rent",
                    CreatedAtUtc = seedDate.AddDays(-2)
                },
                new Lead
                {
                    Id = annaLeadId,
                    InternalReference = "LD-1169",
                    WorkflowStatus = "New",
                    QualificationPercent = 38,
                    NextAction = "Completare recapito e indirizzo immobile",
                    FullName = "Anna Conti",
                    Email = "anna.conti@example.test",
                    SourceChannel = "Ufficio",
                    RequestType = "Valuation",
                    Notes = "Valutazione casa familiare",
                    CreatedAtUtc = seedDate.AddDays(-3)
                },
                new Lead
                {
                    Id = marcoLeadId,
                    WorkflowStatus = "Qualified",
                    FullName = "Marco Guidi",
                    Email = "marco.guidi@example.test",
                    Phone = "333 2028000",
                    SourceChannel = "Agency",
                    RequestType = "Sell",
                    AssignedAgencyUserId = chiaraAgencyUserId,
                    Notes = "Vuole vendere appartamento",
                    CreatedAtUtc = seedDate.AddDays(-4),
                    UpdatedAtUtc = seedDate.AddDays(-4)
                });

            Lead CreateAdminLead(
                int idSuffix,
                string reference,
                string workflowStatus,
                string fullName,
                string email,
                string phone,
                string sourceChannel,
                string requestType,
                int qualificationPercent,
                string nextAction,
                Guid? assignedAgencyUserId,
                int hoursOffset)
            {
                return new Lead
                {
                    Id = Guid.Parse($"30000000-0000-0000-0000-{idSuffix:000000000000}"),
                    InternalReference = reference,
                    WorkflowStatus = workflowStatus,
                    QualificationPercent = qualificationPercent,
                    NextAction = nextAction,
                    AssignedAgencyUserId = assignedAgencyUserId,
                    FullName = fullName,
                    Email = email,
                    Phone = phone,
                    SourceChannel = sourceChannel,
                    RequestType = requestType,
                    CreatedAtUtc = seedDate.AddHours(hoursOffset),
                    UpdatedAtUtc = seedDate.AddHours(hoursOffset)
                };
            }

            context.Leads.AddRange(
                CreateAdminLead(205, "LD-1181", "InContact", "Elena Gori", null, "347 9988776", "WhatsApp", "Buy", 72, "Inviare recap con 3 immobili compatibili", martinaAgencyUserId, -96),
                CreateAdminLead(206, "LD-1168", "New", "Sara Monti", "sara.monti@example.test", "333 1168000", "Telefono", "Buy", 42, "Capire budget sostenibile e zona prioritaria", null, -240),
                CreateAdminLead(207, "LD-1167", "New", "Giulio Neri", "giulio.neri@example.test", "333 1167000", "Email", "Rent", 36, "Verificare garanzie e tempi di ingresso", null, -264),
                CreateAdminLead(208, "LD-1166", "New", "Marta Serra", "marta.serra@example.test", "333 1166000", "WhatsApp", "Sell", 51, "Raccogliere indirizzo e stato immobile", chiaraAgencyUserId, -288),
                CreateAdminLead(209, "LD-1165", "New", "Luca Bellini", "luca.bellini@example.test", "333 1165000", "Ufficio", "Valuation", 40, "Fissare primo confronto telefonico", null, -312),
                CreateAdminLead(210, "LD-1164", "New", "Francesca Ricci", "francesca.ricci@example.test", "333 1164000", "Telefono", "Buy", 48, "Completare esigenze familiari", martinaAgencyUserId, -336),
                CreateAdminLead(211, "LD-1163", "New", "Roberto Fini", "roberto.fini@example.test", "333 1163000", "Email", "RentOut", 44, "Chiarire disponibilita e contratto desiderato", chiaraAgencyUserId, -360),
                CreateAdminLead(212, "LD-1162", "InContact", "Irene Vitali", "irene.vitali@example.test", "333 1162000", "WhatsApp", "Buy", 68, "Inviare immobili zona Centro", martinaAgencyUserId, -384),
                CreateAdminLead(213, "LD-1161", "InContact", "Carlo Benini", "carlo.benini@example.test", "333 1161000", "Telefono", "Sell", 59, "Concordare sopralluogo", chiaraAgencyUserId, -408),
                CreateAdminLead(214, "LD-1160", "InContact", "Laura Guidi", "laura.guidi@example.test", "333 1160000", "Email", "Rent", 63, "Richiedere documentazione garanzie", lorenzoAgencyUserId, -432),
                CreateAdminLead(215, "LD-1159", "InContact", "Davide Farina", "davide.farina@example.test", "333 1159000", "Ufficio", "Valuation", 70, "Preparare riepilogo valutazione", chiaraAgencyUserId, -456),
                CreateAdminLead(216, "LD-1158", "InContact", "Silvia Moretti", "silvia.moretti@example.test", "333 1158000", "WhatsApp", "Buy", 61, "Aggiornare preferenze terrazzo", martinaAgencyUserId, -480),
                CreateAdminLead(217, "LD-1157", "InContact", "Enrico Berti", "enrico.berti@example.test", "333 1157000", "Telefono", "RentOut", 58, "Definire canone atteso", lorenzoAgencyUserId, -504),
                CreateAdminLead(218, "LD-1156", "InContact", "Alessia Fontana", "alessia.fontana@example.test", "333 1156000", "Email", "Buy", 66, "Mandare recap compatibilita", martinaAgencyUserId, -528),
                CreateAdminLead(219, "LD-1155", "InContact", "Giorgio Lombardi", "giorgio.lombardi@example.test", "333 1155000", "Telefono", "Sell", 55, "Confermare documenti catastali", chiaraAgencyUserId, -552),
                CreateAdminLead(226, "LD-1148", "InContact", "Valeria Mancini", "valeria.mancini@example.test", "333 1148000", "WhatsApp", "Valuation", 57, "Richiedere indirizzo preciso per prima valutazione", chiaraAgencyUserId, -564),
                CreateAdminLead(220, "LD-1154", "Archived", "Paola Santi", "paola.santi@example.test", "333 1154000", "Email", "Buy", 22, "Contatto non piu interessato", null, -576),
                CreateAdminLead(221, "LD-1153", "Archived", "Andrea Russo", "andrea.russo@example.test", "333 1153000", "Telefono", "Rent", 18, "Budget non compatibile", null, -600),
                CreateAdminLead(222, "LD-1152", "Archived", "Monica De Angelis", "monica.deangelis@example.test", "333 1152000", "WhatsApp", "Valuation", 25, "Valutazione rinviata", null, -624),
                CreateAdminLead(223, "LD-1151", "Archived", "Stefano Riva", "stefano.riva@example.test", "333 1151000", "Ufficio", "Sell", 20, "Ha scelto altra agenzia", null, -648),
                CreateAdminLead(224, "LD-1150", "Archived", "Claudia Gatti", "claudia.gatti@example.test", "333 1150000", "Email", "Buy", 15, "Richiesta non coltivabile", null, -672),
                CreateAdminLead(225, "LD-1149", "Archived", "Filippo Marchetti", "filippo.marchetti@example.test", "333 1149000", "Telefono", "RentOut", 19, "Immobile non disponibile", null, -696));

            Lead CreateRequestLead(
                int idSuffix,
                string fullName,
                string email,
                string phone,
                string requestType,
                string notes,
                Guid? assignedAgencyUserId,
                int daysOffset)
            {
                return new Lead
                {
                    Id = Guid.Parse($"30000000-0000-0000-0000-{idSuffix:000000000000}"),
                    WorkflowStatus = "Qualified",
                    AssignedAgencyUserId = assignedAgencyUserId,
                    FullName = fullName,
                    Email = email,
                    Phone = phone,
                    SourceChannel = "Agency",
                    RequestType = requestType,
                    Notes = notes,
                    CreatedAtUtc = seedDate.AddDays(daysOffset),
                    UpdatedAtUtc = seedDate.AddDays(daysOffset)
                };
            }

            context.Leads.AddRange(
                CreateRequestLead(301, "Sara Monti", "sara.monti@example.test", "333 2027000", "Buy", "Richiesta da primo colloquio", null, -5),
                CreateRequestLead(302, "Giulio Neri", "giulio.neri@example.test", "333 2026000", "Rent", "Richiesta affitto da qualificare", null, -6),
                CreateRequestLead(303, "Marta Serra", "marta.serra@example.test", "333 2025000", "Sell", "Profilo pronto per matching proprietario", martinaAgencyUserId, -7),
                CreateRequestLead(304, "Luca Bellini", "luca.bellini@example.test", "333 2024000", "Valuation", "Richiesta valutazione assegnata", chiaraAgencyUserId, -8),
                CreateRequestLead(305, "Francesca Ricci", "francesca.ricci@example.test", "333 2023000", "Buy", "Profilo acquisto assegnato", lorenzoAgencyUserId, -9),
                CreateRequestLead(306, "Roberto Fini", "roberto.fini@example.test", "333 2022000", "RentOut", "Ricerca in matching da affinare", null, -10),
                CreateRequestLead(307, "Irene Vitali", "irene.vitali@example.test", "333 2021000", "Buy", "Richiesta in matching", null, -11),
                CreateRequestLead(308, "Carlo Benini", "carlo.benini@example.test", "333 2020000", "Sell", "Profilo vendita in matching", null, -12),
                CreateRequestLead(309, "Laura Guidi", "laura.guidi@example.test", "333 2019000", "Rent", "Aggiornare preferenze canone", null, -13),
                CreateRequestLead(310, "Davide Farina", "davide.farina@example.test", "333 2018000", "Valuation", "Aggiornare obiettivo valutazione", null, -14));

            context.SearchProfiles.AddRange(
                new SearchProfile
                {
                    Id = Guid.Parse("31000000-0000-0000-0000-000000000001"),
                    LeadId = leadId,
                    CriteriaSummary = "Profilo seed tecnico",
                    ContactEmail = "lead.seed@example.test",
                    SourceQueryString = "",
                    CreatedAtUtc = seedDate,
                    UpdatedAtUtc = seedDate
                },
                new SearchProfile
                {
                    Id = Guid.Parse("31000000-0000-0000-0000-000000000101"),
                    LeadId = clientBuyLeadId,
                    PublicReference = "RQ-2047",
                    Status = "InMatching",
                    CriteriaSummary = "Cesena Centro|max EUR 260.000|2 camere",
                    ContactEmail = "elena.gori@email.it",
                    SourceQueryString = "da lead LD-1181",
                    CreatedAtUtc = seedDate.AddDays(-2),
                    UpdatedAtUtc = seedDate.Date.AddHours(11).AddMinutes(20)
                },
                new SearchProfile
                {
                    Id = Guid.Parse("31000000-0000-0000-0000-000000000104"),
                    LeadId = niccoloLeadId,
                    PublicReference = "RQ-2039",
                    Status = "New",
                    CriteriaSummary = "Bilocale|vicino campus|max EUR 750",
                    ContactEmail = "niccolo.f@email.it",
                    SourceQueryString = "Affitto studenti - email",
                    CreatedAtUtc = seedDate.AddDays(-2),
                    UpdatedAtUtc = seedDate.Date.AddDays(-1).AddHours(16).AddMinutes(40)
                },
                new SearchProfile
                {
                    Id = Guid.Parse("31000000-0000-0000-0000-000000000102"),
                    LeadId = annaLeadId,
                    PublicReference = "RQ-2034",
                    Status = "New",
                    CriteriaSummary = "Sopralluogo|Cesena - Diegaro|entro mese",
                    ContactEmail = "anna.conti@example.test",
                    SourceQueryString = "Valutazione casa familiare",
                    CreatedAtUtc = seedDate.AddDays(-3),
                    UpdatedAtUtc = seedDate.Date.AddHours(9).AddMinutes(10)
                },
                new SearchProfile
                {
                    Id = Guid.Parse("31000000-0000-0000-0000-000000000103"),
                    LeadId = marcoLeadId,
                    PublicReference = "RQ-2028",
                    Status = "Updating",
                    CriteriaSummary = "Documenti|foto mancanti|prezzo da validare",
                    ContactEmail = "marco.guidi@example.test",
                    SourceQueryString = "",
                    CreatedAtUtc = seedDate.AddDays(-4),
                    UpdatedAtUtc = new DateTime(2026, 4, 26, 12, 15, 0, DateTimeKind.Utc)
                },
                new SearchProfile
                {
                    Id = Guid.Parse("31000000-0000-0000-0000-000000000201"),
                    LeadId = Guid.Parse("30000000-0000-0000-0000-000000000301"),
                    PublicReference = "RQ-2027",
                    Status = "New",
                    CriteriaSummary = "Centro storico|budget da definire|prima visita",
                    ContactEmail = "sara.monti@example.test",
                    SourceQueryString = "Richiesta da primo colloquio",
                    CreatedAtUtc = seedDate.AddDays(-5),
                    UpdatedAtUtc = seedDate.AddDays(-5)
                },
                new SearchProfile
                {
                    Id = Guid.Parse("31000000-0000-0000-0000-000000000202"),
                    LeadId = Guid.Parse("30000000-0000-0000-0000-000000000302"),
                    PublicReference = "RQ-2026",
                    Status = "New",
                    CriteriaSummary = "Affitto|garanzie da verificare|ingresso rapido",
                    ContactEmail = "giulio.neri@example.test",
                    SourceQueryString = "Richiesta affitto da qualificare",
                    CreatedAtUtc = seedDate.AddDays(-6),
                    UpdatedAtUtc = seedDate.AddDays(-6)
                },
                new SearchProfile
                {
                    Id = Guid.Parse("31000000-0000-0000-0000-000000000203"),
                    LeadId = Guid.Parse("30000000-0000-0000-0000-000000000303"),
                    PublicReference = "RQ-2025",
                    Status = "InMatching",
                    CriteriaSummary = "Trilocale|zona Stadio|box auto",
                    ContactEmail = "marta.serra@example.test",
                    SourceQueryString = "Profilo pronto per matching proprietario",
                    CreatedAtUtc = seedDate.AddDays(-7),
                    UpdatedAtUtc = seedDate.AddDays(-7)
                },
                new SearchProfile
                {
                    Id = Guid.Parse("31000000-0000-0000-0000-000000000204"),
                    LeadId = Guid.Parse("30000000-0000-0000-0000-000000000304"),
                    PublicReference = "RQ-2024",
                    Status = "InMatching",
                    CriteriaSummary = "Sopralluogo|zona Borello|documenti pronti",
                    ContactEmail = "luca.bellini@example.test",
                    SourceQueryString = "Richiesta valutazione assegnata",
                    CreatedAtUtc = seedDate.AddDays(-8),
                    UpdatedAtUtc = seedDate.AddDays(-8)
                },
                new SearchProfile
                {
                    Id = Guid.Parse("31000000-0000-0000-0000-000000000205"),
                    LeadId = Guid.Parse("30000000-0000-0000-0000-000000000305"),
                    PublicReference = "RQ-2023",
                    Status = "InMatching",
                    CriteriaSummary = "Villetta|giardino|mutuo avviato",
                    ContactEmail = "francesca.ricci@example.test",
                    SourceQueryString = "Profilo acquisto assegnato",
                    CreatedAtUtc = seedDate.AddDays(-9),
                    UpdatedAtUtc = seedDate.AddDays(-9)
                },
                new SearchProfile
                {
                    Id = Guid.Parse("31000000-0000-0000-0000-000000000206"),
                    LeadId = Guid.Parse("30000000-0000-0000-0000-000000000306"),
                    PublicReference = "RQ-2022",
                    Status = "InMatching",
                    CriteriaSummary = "Affitto gestito|contratto concordato|arredo parziale",
                    ContactEmail = "roberto.fini@example.test",
                    SourceQueryString = "Ricerca in matching da affinare",
                    CreatedAtUtc = seedDate.AddDays(-10),
                    UpdatedAtUtc = seedDate.AddDays(-10)
                },
                new SearchProfile
                {
                    Id = Guid.Parse("31000000-0000-0000-0000-000000000207"),
                    LeadId = Guid.Parse("30000000-0000-0000-0000-000000000307"),
                    PublicReference = "RQ-2021",
                    Status = "InMatching",
                    CriteriaSummary = "Casa indipendente|zona Ponte Abbadesse|garage",
                    ContactEmail = "irene.vitali@example.test",
                    SourceQueryString = "Richiesta in matching",
                    CreatedAtUtc = seedDate.AddDays(-11),
                    UpdatedAtUtc = seedDate.AddDays(-11)
                },
                new SearchProfile
                {
                    Id = Guid.Parse("31000000-0000-0000-0000-000000000208"),
                    LeadId = Guid.Parse("30000000-0000-0000-0000-000000000308"),
                    PublicReference = "RQ-2020",
                    Status = "InMatching",
                    CriteriaSummary = "Vendita appartamento|foto da produrre|prezzo coerente",
                    ContactEmail = "carlo.benini@example.test",
                    SourceQueryString = "Profilo vendita in matching",
                    CreatedAtUtc = seedDate.AddDays(-12),
                    UpdatedAtUtc = seedDate.AddDays(-12)
                },
                new SearchProfile
                {
                    Id = Guid.Parse("31000000-0000-0000-0000-000000000209"),
                    LeadId = Guid.Parse("30000000-0000-0000-0000-000000000309"),
                    PublicReference = "RQ-2019",
                    Status = "Updating",
                    CriteriaSummary = "Bilocale|canone aggiornato|garanzie",
                    ContactEmail = "laura.guidi@example.test",
                    SourceQueryString = "Aggiornare preferenze canone",
                    CreatedAtUtc = seedDate.AddDays(-13),
                    UpdatedAtUtc = seedDate.AddDays(-13)
                },
                new SearchProfile
                {
                    Id = Guid.Parse("31000000-0000-0000-0000-000000000210"),
                    LeadId = Guid.Parse("30000000-0000-0000-0000-000000000310"),
                    PublicReference = "RQ-2018",
                    Status = "Updating",
                    CriteriaSummary = "Valutazione|obiettivo cambiato|richiamare",
                    ContactEmail = "davide.farina@example.test",
                    SourceQueryString = "Aggiornare obiettivo valutazione",
                    CreatedAtUtc = seedDate.AddDays(-14),
                    UpdatedAtUtc = seedDate.AddDays(-14)
                });

            context.LeadPreferences.Add(new LeadPreference
            {
                Id = Guid.Parse("32000000-0000-0000-0000-000000000001"),
                LeadId = leadId,
                MinimumPrice = 150000m,
                MaximumPrice = 300000m,
                DesiredLocation = "Area seed",
                MinimumRooms = 3
            });

            context.LeadPreferences.AddRange(
                new LeadPreference
                {
                    Id = Guid.Parse("32000000-0000-0000-0000-000000000101"),
                    LeadId = clientBuyLeadId,
                    MaximumPrice = 260000m,
                    DesiredLocation = "Cesena Centro, San Mauro",
                    AcceptableLocations = "Oltresavio, S. Egidio, Diegaro",
                    MinimumRooms = 2,
                    SustainableBudgetLabel = "max EUR 260.000",
                    ExpectedPriceOrMainQuestion = "EUR 230.000 - 245.000",
                    Timing = "Alta",
                    DesiredMoveIn = "Entro 3 mesi",
                    FinancingStatus = "Pre-delibera in corso",
                    PropertyToSellStatus = "No",
                    SearchStage = "Cerca una casa definitiva a Cesena, possibilmente comoda al centro ma non necessariamente in pieno centro. Budget chiaro, ricerca matura, preferisce evitare ristrutturazioni pesanti.",
                    HouseholdDescription = "Vuole ricevere poche proposte ma motivate. Ha scartato immobili luminosi ma senza ascensore. Sensibile a spese condominiali e stato impianti.",
                    PreferencesAndCompromises = "Terrazzo o piccolo giardino|Garage o posto auto|Pronta da abitare|Classe energetica buona",
                    PropertyCondition = "Metratura flessibile|Piccoli lavori accettati|Quartieri limitrofi|Bagno singolo se casa valida",
                    ValuationGoal = "Ricerca abitazione principale"
                },
                new LeadPreference
                {
                    Id = Guid.Parse("32000000-0000-0000-0000-000000000102"),
                    LeadId = clientValuationLeadId,
                    DesiredLocation = "Diegaro",
                    PropertyType = "Casa familiare",
                    ValuationGoal = "Prima valutazione"
                },
                new LeadPreference
                {
                    Id = Guid.Parse("32000000-0000-0000-0000-000000000201"),
                    LeadId = paoloLeadId,
                    Timing = "Da capire",
                    ValuationGoal = "Vuole capire se vendere entro l'anno",
                    PreferencesAndCompromises = "Zona precisa, metratura indicativa, stato dell'immobile, tempi desiderati, eventuali vincoli familiari."
                },
                new LeadPreference
                {
                    Id = Guid.Parse("32000000-0000-0000-0000-000000000202"),
                    LeadId = niccoloLeadId,
                    PropertyType = "Bilocale",
                    DesiredLocation = "vicino campus",
                    SustainableBudgetLabel = "max EUR 750",
                    Timing = "Media"
                },
                new LeadPreference
                {
                    Id = Guid.Parse("32000000-0000-0000-0000-000000000203"),
                    LeadId = annaLeadId,
                    PropertyType = "Sopralluogo",
                    DesiredLocation = "Cesena - Diegaro",
                    ValuationGoal = "entro mese",
                    Timing = "Alta"
                },
                new LeadPreference
                {
                    Id = Guid.Parse("32000000-0000-0000-0000-000000000204"),
                    LeadId = marcoLeadId,
                    PropertyToSellStatus = "Documenti",
                    PreferencesAndCompromises = "foto mancanti",
                    ExpectedPriceOrMainQuestion = "prezzo da validare",
                    Timing = "Media"
                });

            context.Interactions.Add(new Interaction
            {
                Id = Guid.Parse("33000000-0000-0000-0000-000000000001"),
                LeadId = leadId,
                Channel = "Seed",
                Notes = "Interazione seed tecnico",
                OccurredAtUtc = seedDate
            });

            context.Interactions.AddRange(
                new Interaction
                {
                    Id = Guid.Parse("33000000-0000-0000-0000-000000000101"),
                    LeadId = clientBuyLeadId,
                    Channel = "Preferenze aggiornate",
                    Notes = "Aggiunto vincolo ascensore per piani alti e maggiore attenzione a spese condominiali.",
                    OccurredAtUtc = seedDate.Date.AddHours(11).AddMinutes(20)
                },
                new Interaction
                {
                    Id = Guid.Parse("33000000-0000-0000-0000-000000000102"),
                    LeadId = clientBuyLeadId,
                    Channel = "Richiesta convertita da lead",
                    Notes = "Martina ha strutturato budget, zone valutabili e vincoli minimi dalla conversazione WhatsApp.",
                    OccurredAtUtc = seedDate.Date.AddDays(-1).AddHours(17).AddMinutes(5)
                },
                new Interaction
                {
                    Id = Guid.Parse("33000000-0000-0000-0000-000000000103"),
                    LeadId = clientBuyLeadId,
                    Channel = "Primo contatto",
                    Notes = "Cliente interessata a ricevere proposte selezionate, non una lista generica di annunci.",
                    OccurredAtUtc = new DateTime(2026, 4, 26, 12, 0, 0, DateTimeKind.Utc)
                },
                new Interaction
                {
                    Id = Guid.Parse("33000000-0000-0000-0000-000000000201"),
                    LeadId = paoloLeadId,
                    Channel = "Telefonata registrata",
                    Notes = "Michela ha annotato il primo contatto e i dati minimi disponibili.",
                    OccurredAtUtc = seedDate.AddDays(-1).Date.AddHours(10).AddMinutes(42)
                },
                new Interaction
                {
                    Id = Guid.Parse("33000000-0000-0000-0000-000000000202"),
                    LeadId = paoloLeadId,
                    Channel = "Prossima azione proposta",
                    Notes = "Richiamare per completare zona, tempi e caratteristiche dell'immobile.",
                    OccurredAtUtc = seedDate.AddDays(-1).Date.AddHours(10).AddMinutes(45)
                });

            context.Appointments.Add(new Appointment
            {
                Id = Guid.Parse("35000000-0000-0000-0000-000000000001"),
                LeadId = leadId,
                PropertyListingId = propertyListingId,
                AgencyUserId = agencyUserId,
                ScheduledAtUtc = seedDate.AddDays(1),
                Status = "Scheduled",
                RequestType = "Seed",
                CreatedAtUtc = seedDate
            });

            context.Appointments.AddRange(
                new Appointment
                {
                    Id = Guid.Parse("35000000-0000-0000-0000-000000000101"),
                    LeadId = clientBuyLeadId,
                    AgencyUserId = martinaAgencyUserId,
                    Status = "InMatching",
                    RequestType = "GeneralContact",
                    CreatedAtUtc = seedDate.AddDays(-2)
                },
                new Appointment
                {
                    Id = Guid.Parse("35000000-0000-0000-0000-000000000102"),
                    LeadId = clientValuationLeadId,
                    AgencyUserId = chiaraAgencyUserId,
                    Status = "Received",
                    RequestType = "GeneralContact",
                    CreatedAtUtc = seedDate.AddDays(-8)
                });

            context.SaveChanges();
        }

        private static void CompleteSeedListingDetails(PropertyListing[] listings)
        {
            foreach (var item in listings.Select((listing, index) => new { listing, index }))
            {
                var listing = item.listing;
                var ordinal = item.index + 1;

                if (listing.Latitude == 0d && listing.Longitude == 0d)
                {
                    listing.Latitude = 44.1396d + ((ordinal % 7) - 3) * 0.0011d;
                    listing.Longitude = 12.2431d + ((ordinal % 5) - 2) * 0.0011d;
                }

                listing.ListingCategory = Fill(listing.ListingCategory, "Abitazione");
                listing.BedroomsLabel = Fill(
                    listing.BedroomsLabel,
                    Math.Max(1, listing.Rooms - 1).ToString(System.Globalization.CultureInfo.InvariantCulture));
                listing.FloorLabel = Fill(listing.FloorLabel, ordinal % 4 == 0 ? "Piano terra" : ordinal % 3 == 0 ? "Piano alto" : "Piano intermedio");
                listing.ElevatorLabel = Fill(listing.ElevatorLabel, ordinal % 3 == 0 ? "Ascensore presente" : "Ascensore da verificare");
                listing.GarageLabel = Fill(listing.GarageLabel, listing.FeatureKeys.Contains("|garage|") ? "Garage" : "Posto auto");
                listing.OutdoorSpaceLabel = Fill(listing.OutdoorSpaceLabel, listing.FeatureKeys.Contains("|garden|") ? "Giardino" : "Balcone");
                listing.EnergyClass = Fill(listing.EnergyClass, ordinal % 4 == 0 ? "D" : "C");
                listing.AvailabilityLabel = Fill(listing.AvailabilityLabel, string.Equals(listing.Contract, "Affitto", StringComparison.OrdinalIgnoreCase) ? "Ingresso da concordare" : "Disponibile a rogito");
                listing.HeatingLabel = Fill(listing.HeatingLabel, "Autonomo");
                listing.RequiredWorksLabel = Fill(listing.RequiredWorksLabel, listing.FeatureKeys.Contains("|renovation-needed|") ? "Aggiornamenti da prevedere" : "Buono");
                listing.ConstructionYearLabel = Fill(listing.ConstructionYearLabel, (1996 + (ordinal % 24)).ToString(System.Globalization.CultureInfo.InvariantCulture));
                listing.CondoFeesLabel = Fill(listing.CondoFeesLabel, listing.PropertyType.Contains("Casa", StringComparison.OrdinalIgnoreCase) ? "Nessuna spesa condominiale" : "Spese condominiali da verificare");
                listing.BalconyLabel = Fill(listing.BalconyLabel, listing.FeatureKeys.Contains("|terrace|") ? "Terrazzo o balcone" : "Balcone da verificare");
                listing.CellarLabel = Fill(listing.CellarLabel, ordinal % 2 == 0 ? "Cantina" : "Cantina da verificare");
                listing.NearbyServicesLabel = Fill(listing.NearbyServicesLabel, "Servizi di quartiere e collegamenti vicini");
                listing.SummaryTitle = Fill(listing.SummaryTitle, listing.Title);

                if (ShouldReplaceSeedText(listing.SummaryParagraph1))
                {
                    listing.SummaryParagraph1 = BuildPrimaryDescription(listing);
                }

                if (ShouldReplaceSeedText(listing.SummaryParagraph2))
                {
                    listing.SummaryParagraph2 = BuildSecondaryDescription(listing);
                }

                listing.ReadinessNote = Fill(listing.ReadinessNote, $"{listing.AvailabilityLabel}; stato immobile: {listing.RequiredWorksLabel}.");
                listing.CostsNote = Fill(listing.CostsNote, $"{listing.CondoFeesLabel}; eventuali interventi da quantificare dopo il sopralluogo.");
                listing.ContextNote = Fill(listing.ContextNote, $"{listing.DisplayLocation}, zona {listing.Zone}, con servizi e collegamenti nella quotidianita.");
                listing.DecisionMarginNote = Fill(listing.DecisionMarginNote, $"Da valutare rispetto a budget, tempi di ingresso e priorita su {listing.OutdoorSpaceLabel.ToLowerInvariant()}.");

                if (ShouldReplaceSeedText(listing.HumanFitNote))
                {
                    listing.HumanFitNote = BuildHumanFitNote(listing);
                }

                listing.ZoneComparisonLabel = Fill(listing.ZoneComparisonLabel, $"{listing.Zone}: {listing.NearbyServicesLabel}");
                listing.SurfaceRoomsComparisonLabel = Fill(listing.SurfaceRoomsComparisonLabel, $"{listing.SurfaceSquareMeters.ToString(System.Globalization.CultureInfo.InvariantCulture)} mq - {listing.Rooms.ToString(System.Globalization.CultureInfo.InvariantCulture)} locali");
                listing.StatusWorksComparisonLabel = Fill(listing.StatusWorksComparisonLabel, $"{listing.RequiredWorksLabel} - classe {listing.EnergyClass}");
                listing.MainCompromise = Fill(listing.MainCompromise, listing.FeatureKeys.Contains("|renovation-needed|") ? "Prezzo e spazio compensano lavori da programmare" : "Equilibrio tra posizione, spazi e costi");
                listing.AccessLabel = Fill(listing.AccessLabel, listing.FloorLabel);
                listing.ManagementCostsLabel = Fill(listing.ManagementCostsLabel, listing.CondoFeesLabel);
                listing.EstimatedWorksLabel = Fill(listing.EstimatedWorksLabel, listing.RequiredWorksLabel);
                listing.EnergyCostsLabel = Fill(listing.EnergyCostsLabel, $"Classe {listing.EnergyClass}");
                listing.PersonalizationLabel = Fill(listing.PersonalizationLabel, listing.FeatureKeys.Contains("|renovation-needed|") ? "Alta" : "Media");
                listing.TransportLabel = Fill(listing.TransportLabel, listing.FeatureKeys.Contains("|near-public-transport|") ? "Collegamenti vicini" : "Servizi raggiungibili in auto");
                listing.PrivacyLabel = Fill(listing.PrivacyLabel, listing.FeatureKeys.Contains("|garden|") ? "Buona" : "Media");
                listing.NoiseLabel = Fill(listing.NoiseLabel, listing.Zone.Contains("Centro", StringComparison.OrdinalIgnoreCase) ? "Media, contesto centrale" : "Bassa o media");
                listing.IdealTargetLabel = Fill(listing.IdealTargetLabel, IdealTarget(listing));
            }
        }

        private static string Fill(string value, string fallback)
        {
            return string.IsNullOrWhiteSpace(value) ? fallback : value;
        }

        private static bool ShouldReplaceSeedText(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return true;
            }

            var normalized = value.ToLowerInvariant();
            return normalized.Contains("soluzione dimostrativa") ||
                normalized.Contains("dati essenziali") ||
                normalized.Contains("coerente con i risultati") ||
                normalized.Contains("soluzione utile per valutare");
        }

        private static string BuildPrimaryDescription(PropertyListing listing)
        {
            var contractText = string.Equals(listing.Contract, "Affitto", StringComparison.OrdinalIgnoreCase)
                ? "in locazione"
                : "in vendita";
            return $"{listing.Title} {contractText} a {listing.DisplayLocation}: {listing.SurfaceSquareMeters.ToString(System.Globalization.CultureInfo.InvariantCulture)} mq distribuiti in {listing.Rooms.ToString(System.Globalization.CultureInfo.InvariantCulture)} locali, con {listing.OutdoorSpaceLabel.ToLowerInvariant()} e servizi raggiungibili nella quotidianita.";
        }

        private static string BuildSecondaryDescription(PropertyListing listing)
        {
            var condition = string.Equals(listing.RequiredWorksLabel, "Buono", StringComparison.OrdinalIgnoreCase)
                ? "si presenta in buono stato"
                : "richiede una valutazione puntuale degli interventi";
            return $"L'immobile {condition}; la scheda mette in evidenza disponibilita, costi ricorrenti, contesto e margini decisionali prima della visita.";
        }

        private static string BuildHumanFitNote(PropertyListing listing)
        {
            return $"Potrebbe essere adatto se cerchi {listing.PropertyType.ToLowerInvariant()} in zona {listing.Zone}, vuoi capire subito spazi, costi e tempi, e preferisci approfondire solo annunci con informazioni complete.";
        }

        private static string IdealTarget(PropertyListing listing)
        {
            if (string.Equals(listing.Contract, "Affitto", StringComparison.OrdinalIgnoreCase))
            {
                return listing.Rooms <= 2 ? "Single o coppia" : "Coppia o piccolo nucleo";
            }

            return listing.Rooms >= 4 ? "Famiglia" : "Coppia o primo acquisto";
        }

        private static void AddGalleryMediaForListings(MedriDbContext context, PropertyListing[] listings)
        {
            foreach (var listing in listings.Where(listing => !string.IsNullOrWhiteSpace(listing.ImageUrl)))
            {
                var existingSortOrders = context.PropertyMedia.Local
                    .Where(media => media.PropertyListingId == listing.Id)
                    .Select(media => media.SortOrder)
                    .ToHashSet();

                if (existingSortOrders.Count >= 4)
                {
                    continue;
                }

                for (var sortOrder = 1; sortOrder <= 4; sortOrder++)
                {
                    if (existingSortOrders.Contains(sortOrder))
                    {
                        continue;
                    }

                    context.PropertyMedia.Add(new PropertyMedia
                    {
                        Id = CreateGalleryMediaId(listing.Id, sortOrder),
                        PropertyListingId = listing.Id,
                        Url = GalleryImageUrl(listing, sortOrder),
                        AltText = GalleryAltText(listing, sortOrder),
                        SortOrder = sortOrder
                    });
                }
            }
        }

        private static string GalleryImageUrl(PropertyListing listing, int sortOrder)
        {
            if (sortOrder == 1)
            {
                return listing.ImageUrl;
            }

            var pool = new[]
            {
                "/medri-reference/assets/properties/property-01.jpg",
                "/medri-reference/assets/properties/property-02.jpg",
                "/medri-reference/assets/properties/property-03.jpg",
                "/medri-reference/assets/properties/property-04.jpg",
                "/medri-reference/assets/properties/property-05.jpg",
                "/medri-reference/assets/properties/property-06.jpg",
                "/medri-reference/assets/properties/property-07.jpg",
                "/medri-reference/assets/properties/property-08.jpg"
            }
            .Where(url => !string.Equals(url, listing.ImageUrl, StringComparison.OrdinalIgnoreCase))
            .ToArray();

            return pool[(sortOrder - 2) % pool.Length];
        }

        private static string GalleryAltText(PropertyListing listing, int sortOrder)
        {
            return sortOrder switch
            {
                1 => listing.Title,
                2 => "Zona giorno",
                3 => listing.OutdoorSpaceLabel,
                4 => "Camera e servizi",
                _ => "Foto immobile"
            };
        }

        private static Guid CreateGalleryMediaId(Guid listingId, int sortOrder)
        {
            var bytes = listingId.ToByteArray();
            bytes[3] = 0x22;
            bytes[2] = (byte)sortOrder;
            return new Guid(bytes);
        }
    }
}
