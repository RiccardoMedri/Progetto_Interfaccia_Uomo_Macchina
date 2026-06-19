using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Medri.Services;
using Medri.Services.Medri;
using Medri.Services.Medri.Application;
using Medri.Services.Medri.Identity;
using Microsoft.EntityFrameworkCore;

namespace Medri.Infrastructure
{
    public class DataGenerator
    {
        private const int SeedListingCount = 50;

        public static void InitializeUsers(MedriDbContext context)
        {
            if (context.Users.Any())
            {
                return;
            }

            var refs = new SeedReferences();
            var demoPasswordHash = AccountPasswordHasher.Hash("password");

            context.Users.AddRange(
                new User
                {
                    Id = refs.ClientUserId,
                    Email = "elena.gori@email.it",
                    Password = demoPasswordHash,
                    FirstName = "Elena",
                    LastName = "Gori",
                    NickName = "Elena",
                    DisplayName = "Elena Gori",
                    Role = UserRoles.Client
                },
                new User
                {
                    Id = refs.Martina,
                    Email = "martina.ricci@example.test",
                    Password = demoPasswordHash,
                    FirstName = "Martina",
                    LastName = "Ricci",
                    NickName = "Martina",
                    DisplayName = "Martina Ricci",
                    Role = UserRoles.Operator,
                    AgencyRole = AgencyStaffRoles.Advisor
                },
                new User
                {
                    Id = refs.Chiara,
                    Email = "chiara.medri@email.it",
                    Password = demoPasswordHash,
                    FirstName = "Chiara",
                    LastName = "Medri",
                    NickName = "Chiara",
                    DisplayName = "Chiara Medri",
                    Role = UserRoles.Admin,
                    AgencyRole = AgencyStaffRoles.Manager
                },
                new User
                {
                    Id = refs.Lorenzo,
                    Email = "lorenzo.bassi@example.test",
                    Password = demoPasswordHash,
                    FirstName = "Lorenzo",
                    LastName = "Bassi",
                    NickName = "Lorenzo",
                    DisplayName = "Lorenzo Bassi",
                    Role = UserRoles.Operator,
                    AgencyRole = AgencyStaffRoles.Operator
                },
                new User
                {
                    Id = refs.Marco,
                    Email = "marco.guidi@example.test",
                    Password = demoPasswordHash,
                    FirstName = "Marco",
                    LastName = "Guidi",
                    NickName = "Marco",
                    DisplayName = "Marco Guidi",
                    Role = UserRoles.Operator,
                    AgencyRole = AgencyStaffRoles.Operator
                });

            context.SaveChanges();
        }

        public static void InitializeMedriDemoData(MedriDbContext context)
        {
            if (context.PropertyListings.IgnoreQueryFilters().Any())
            {
                return;
            }

            var refs = new SeedReferences();

            SeedListings(context, refs);
            SeedFavorites(context, refs);
            SeedNotificationPreferences(context, refs);
            SeedStandaloneLeads(context, refs);
            SeedRequests(context, refs);
            SeedAppointments(context, refs);

            context.SaveChanges();
        }


        private static void SeedListings(MedriDbContext context, SeedReferences refs)
        {
            var listings = BuildSeedListings(refs);

            CompleteSeedListingDetails(listings);

            context.PropertyListings.AddRange(listings);
            AddGalleryMediaForListings(context, listings);
            ApplySeedCompletion(listings, context.PropertyMedia.Local);
        }

        private static PropertyListing[] BuildSeedListings(SeedReferences refs)
        {
            var specs = SeedListingSpecs();
            if (specs.Length != SeedListingCount)
            {
                throw new InvalidOperationException(
                    $"Expected {SeedListingCount.ToString(CultureInfo.InvariantCulture)} seed listings, found {specs.Length.ToString(CultureInfo.InvariantCulture)}.");
            }

            return specs
                .Select((spec, index) => CreateSeedListing(spec, refs, index))
                .ToArray();
        }

        private static PropertyListing CreateSeedListing(
            SeedListingSpec spec,
            SeedReferences refs,
            int index)
        {
            return new PropertyListing
            {
                Id = Guid.Parse($"20000000-0000-0000-0000-{spec.IdSuffix}"),
                InternalReference = spec.Reference,
                PublicationStatus = spec.PublicationStatus,
                AssignedAgencyUserId = ResolveStaffUserId(spec.AssignedUser, refs),
                FeaturedSortOrder = spec.FeaturedSortOrder,
                Title = spec.Title,
                Slug = spec.Slug,
                Location = "Cesena",
                DisplayLocation = spec.DisplayLocation,
                Price = spec.Price,
                Rooms = spec.Rooms,
                Bathrooms = spec.Bathrooms,
                SurfaceSquareMeters = spec.SurfaceSquareMeters,
                Status = spec.Contract,
                Contract = spec.Contract,
                PropertyType = spec.PropertyType,
                Zone = spec.Zone,
                FeatureKeys = spec.FeatureKeys,
                ImageUrl = spec.ImageUrl,
                Latitude = spec.Latitude,
                Longitude = spec.Longitude,
                Address = spec.Address,
                EnergyClass = spec.EnergyClass,
                SortOrder = spec.SortOrder,
                UpdatedAtUtc = refs.SeedDate.AddDays(-(index + 1))
            };
        }

        private static Guid ResolveStaffUserId(string assignedUser, SeedReferences refs)
        {
            return assignedUser switch
            {
                "Martina" => refs.Martina,
                "Lorenzo" => refs.Lorenzo,
                "Marco" => refs.Marco,
                _ => refs.Chiara
            };
        }

        private static SeedListingSpec[] SeedListingSpecs()
        {
            return new[]
            {
                Spec("000000000101", "IM-342", PropertyPublicationStatuses.Incomplete, "Chiara", "Trilocale ristrutturato", "bozza-im-342", "Cesena - Centro", 235000m, 3, 1, 88, "Vendita", "Trilocale", "Centro", "|terrace|move-in-ready|near-services|near-public-transport|", "/medri-reference/assets/properties/property-06.jpg", 44.1390d, 12.2430d, "Via Mura Barriera Ponente 12, Cesena", "C", 201),
                Spec("000000000102", "IM-329", PropertyPublicationStatuses.NeedsUpdate, "Martina", "Appartamento con terrazzo", "bozza-im-329", "Cesena - Fiorenzuola", 260000m, 4, 2, 105, "Vendita", "Appartamento", "Fiorenzuola", "|terrace|parking|move-in-ready|near-services|", "/medri-reference/assets/properties/property-05.jpg", 44.1334d, 12.2550d, "Via Fiorenzuola 16, Cesena", "C", 202),
                Spec("000000000103", "IM-318", PropertyPublicationStatuses.Ready, "Lorenzo", "Villetta a schiera", "bozza-im-318", "Cesena - S. Egidio", 285000m, 4, 2, 120, "Vendita", "Villetta", "Sant'Egidio", "|garden|garage|parking|renovation-needed|near-services|", "/medri-reference/assets/properties/property-03.jpg", 44.1530d, 12.2860d, "Via S. Egidio 42, Cesena", "D", 203),
                Spec("000000001039", "IM-1039", PropertyPublicationStatuses.Incomplete, "Martina", "Appartamento con giardino", "admin-im-1039", "Cesena - Centro", 210000m, 3, 1, 95, "Vendita", "Appartamento", "Centro", "|garden|move-in-ready|near-services|", "/medri-reference/assets/properties/property-01.jpg", 44.1396d, 12.2431d, "Via Cesare Battisti 6, Cesena", "C", 11),
                Spec("000000001046", "IM-1046", PropertyPublicationStatuses.Ready, "Lorenzo", "Nuovo appartamento in classe A", "admin-im-1046", "Cesena - Diegaro", 310000m, 4, 2, 110, "Vendita", "Appartamento", "Diegaro", "|terrace|elevator|move-in-ready|high-energy-class|", "/medri-reference/assets/properties/property-08.jpg", 44.1005d, 12.2140d, "Via Diegaro 8, Cesena", "A", 12),
                Spec("000000000208", "AF-208", PropertyPublicationStatuses.NeedsUpdate, "Marco", "Trilocale arredato", "admin-af-208", "Cesena - Oltresavio", 750m, 3, 1, 75, "Affitto", "Trilocale", "Oltresavio", "|terrace|parking|move-in-ready|near-services|", "/medri-reference/assets/properties/property-07.jpg", 44.1285d, 12.2370d, "Via Savio 18, Cesena", "E", 13),
                Spec("000000001050", "IM-1050", PropertyPublicationStatuses.Ready, "Chiara", "Bilocale in centro", "admin-im-1050", "Cesena - Centro", 165000m, 2, 1, 62, "Vendita", "Bilocale", "Centro", "|terrace|move-in-ready|near-services|near-public-transport|", "/medri-reference/assets/properties/property-02.jpg", 44.1403d, 12.2462d, "Corso Sozzi 14, Cesena", "E", 30),
                Spec("000000001051", "IM-1051", PropertyPublicationStatuses.Ready, "Martina", "Casa indipendente", "admin-im-1051", "Cesena - Case Finali", 390000m, 5, 2, 160, "Vendita", "Casa indipendente", "Case Finali", "|garden|garage|parking|renovation-needed|", "/medri-reference/assets/properties/property-06.jpg", 44.1565d, 12.2275d, "Via Case Finali 21, Cesena", "D", 31),
                Spec("000000001052", "IM-1052", PropertyPublicationStatuses.Ready, "Lorenzo", "Loft zona stazione", "admin-im-1052", "Cesena - Stazione", 198000m, 2, 1, 70, "Vendita", "Loft", "Stazione", "|terrace|move-in-ready|near-services|near-public-transport|", "/medri-reference/assets/properties/property-04.jpg", 44.1467d, 12.2467d, "Via Stazione 4, Cesena", "E", 32),
                Spec("000000001053", "IM-1053", PropertyPublicationStatuses.Ready, "Marco", "Porzione con corte", "admin-im-1053", "Cesena - San Mauro", 248000m, 4, 2, 116, "Vendita", "Porzione", "San Mauro", "|garden|parking|move-in-ready|near-services|", "/medri-reference/assets/properties/property-05.jpg", 44.1431d, 12.2245d, "Via San Mauro 17, Cesena", "C", 33),
                Spec("000000001054", "AF-209", PropertyPublicationStatuses.Ready, "Martina", "Monolocale arredato", "admin-af-209", "Cesena - Centro", 580m, 1, 1, 42, "Affitto", "Monolocale", "Centro", "|furnished|move-in-ready|near-services|near-public-transport|", "/medri-reference/assets/properties/property-07.jpg", 44.1391d, 12.2464d, "Via Zeffirino Re 9, Cesena", "E", 34),
                Spec("000000001055", "IM-1055", PropertyPublicationStatuses.NeedsUpdate, "Chiara", "Quadrilocale luminoso", "admin-im-1055", "Cesena - Ponte Abbadesse", 255000m, 4, 2, 108, "Vendita", "Quadrilocale", "Ponte Abbadesse", "|terrace|parking|move-in-ready|near-services|", "/medri-reference/assets/properties/property-02.jpg", 44.1294d, 12.2528d, "Via Ponte Abbadesse 11, Cesena", "D", 40),
                Spec("000000001056", "AF-210", PropertyPublicationStatuses.NeedsUpdate, "Marco", "Bilocale arredato", "admin-af-210", "Cesena - Fiorita", 690m, 2, 1, 58, "Affitto", "Bilocale", "Fiorita", "|furnished|parking|move-in-ready|near-services|", "/medri-reference/assets/properties/property-01.jpg", 44.1364d, 12.2642d, "Via Fiorita 5, Cesena", "E", 41),

                Spec("000000000301", "IM-P001", PropertyPublicationStatuses.Published, "Chiara", "Trilocale con balcone", "admin-pubblicato-01", "Cesena - Centro", 169200m, 3, 1, 86, "Vendita", "Trilocale", "Centro", "|terrace|move-in-ready|near-services|near-public-transport|", "/medri-reference/assets/properties/property-01.jpg", 44.1396d, 12.2431d, "Via Mura Barriera Ponente 12, Cesena", "C", 301, 1),
                Spec("000000000302", "IM-P002", PropertyPublicationStatuses.Published, "Martina", "Appartamento con terrazzo", "admin-pubblicato-02", "Cesena - Oltresavio", 173400m, 3, 1, 78, "Vendita", "Appartamento", "Oltresavio", "|terrace|parking|move-in-ready|near-services|", "/medri-reference/assets/properties/property-07.jpg", 44.1285d, 12.2370d, "Via Savio 18, Cesena", "E", 302, 2),
                Spec("000000000303", "IM-P003", PropertyPublicationStatuses.Published, "Lorenzo", "Villetta con giardino", "admin-pubblicato-03", "Cesena - S. Egidio", 177600m, 4, 2, 118, "Vendita", "Villetta", "Sant'Egidio", "|garden|garage|parking|renovation-needed|near-services|", "/medri-reference/assets/properties/property-03.jpg", 44.1530d, 12.2860d, "Via S. Egidio 42, Cesena", "D", 303, 3),
                Spec("000000000304", "IM-P004", PropertyPublicationStatuses.Published, "Marco", "Quadrilocale in classe A", "admin-pubblicato-04", "Cesena - Diegaro", 181800m, 4, 2, 106, "Vendita", "Appartamento", "Diegaro", "|terrace|elevator|move-in-ready|high-energy-class|", "/medri-reference/assets/properties/property-08.jpg", 44.1005d, 12.2140d, "Via Diegaro 8, Cesena", "A", 304),
                Spec("000000000305", "AF-P005", PropertyPublicationStatuses.Published, "Chiara", "Bilocale arredato in centro", "admin-pubblicato-05", "Cesena - Centro", 715m, 2, 1, 58, "Affitto", "Bilocale", "Centro", "|furnished|terrace|move-in-ready|near-services|near-public-transport|", "/medri-reference/assets/properties/property-02.jpg", 44.1406d, 12.2440d, "Via Chiaramonti 3, Cesena", "E", 305),
                Spec("000000000306", "IM-P006", PropertyPublicationStatuses.Published, "Lorenzo", "Bilocale zona stazione", "admin-pubblicato-06", "Cesena - Stazione", 190200m, 2, 1, 64, "Vendita", "Bilocale", "Stazione", "|terrace|move-in-ready|near-services|near-public-transport|", "/medri-reference/assets/properties/property-02.jpg", 44.1467d, 12.2467d, "Via Stazione 4, Cesena", "E", 306),
                Spec("000000000307", "IM-P007", PropertyPublicationStatuses.Published, "Chiara", "Porzione con terrazzo", "admin-pubblicato-07", "Cesena - Fiorenzuola", 194400m, 4, 2, 104, "Vendita", "Porzione", "Fiorenzuola", "|terrace|parking|move-in-ready|near-services|", "/medri-reference/assets/properties/property-05.jpg", 44.1334d, 12.2550d, "Via Fiorenzuola 16, Cesena", "C", 307),
                Spec("000000000308", "IM-P008", PropertyPublicationStatuses.Published, "Marco", "Appartamento ultimo piano", "admin-pubblicato-08", "Cesena - Centro storico", 198600m, 3, 1, 82, "Vendita", "Appartamento", "Centro storico", "|terrace|move-in-ready|high-energy-class|near-services|", "/medri-reference/assets/properties/property-04.jpg", 44.1391d, 12.2464d, "Via Centro Storico 9, Cesena", "B", 308),
                Spec("000000000309", "IM-P009", PropertyPublicationStatuses.Published, "Lorenzo", "Casa indipendente con corte", "admin-pubblicato-09", "Cesena - Case Finali", 202800m, 5, 2, 158, "Vendita", "Casa indipendente", "Case Finali", "|garden|garage|parking|renovation-needed|", "/medri-reference/assets/properties/property-06.jpg", 44.1565d, 12.2275d, "Via Case Finali 21, Cesena", "D", 309),
                Spec("000000000310", "AF-P010", PropertyPublicationStatuses.Published, "Martina", "Trilocale arredato Oltresavio", "admin-pubblicato-10", "Cesena - Oltresavio", 575m, 3, 1, 70, "Affitto", "Trilocale", "Oltresavio", "|furnished|terrace|parking|move-in-ready|near-services|", "/medri-reference/assets/properties/property-07.jpg", 44.1279d, 12.2381d, "Via Savio 22, Cesena", "E", 310),
                Spec("000000000311", "IM-P011", PropertyPublicationStatuses.Published, "Chiara", "Appartamento con terrazzo", "admin-pubblicato-11", "Cesena - Oltresavio", 211200m, 3, 1, 79, "Vendita", "Appartamento", "Oltresavio", "|terrace|parking|move-in-ready|near-services|", "/medri-reference/assets/properties/property-07.jpg", 44.1290d, 12.2362d, "Via Savio 30, Cesena", "E", 311),
                Spec("000000000312", "IM-P012", PropertyPublicationStatuses.Published, "Marco", "Villetta con giardino", "admin-pubblicato-12", "Cesena - S. Egidio", 215400m, 4, 2, 120, "Vendita", "Villetta", "Sant'Egidio", "|garden|garage|parking|renovation-needed|near-services|", "/medri-reference/assets/properties/property-03.jpg", 44.1521d, 12.2848d, "Via S. Egidio 58, Cesena", "D", 312),
                Spec("000000000313", "IM-P013", PropertyPublicationStatuses.Published, "Chiara", "Quadrilocale in classe A", "admin-pubblicato-13", "Cesena - Diegaro", 219600m, 4, 2, 108, "Vendita", "Appartamento", "Diegaro", "|terrace|elevator|move-in-ready|high-energy-class|", "/medri-reference/assets/properties/property-08.jpg", 44.1014d, 12.2151d, "Via Diegaro 12, Cesena", "A", 313),
                Spec("000000000314", "IM-P014", PropertyPublicationStatuses.Published, "Martina", "Casa indipendente con corte", "admin-pubblicato-14", "Cesena - Case Finali", 223800m, 5, 2, 160, "Vendita", "Casa indipendente", "Case Finali", "|garden|garage|parking|renovation-needed|", "/medri-reference/assets/properties/property-06.jpg", 44.1572d, 12.2284d, "Via Case Finali 32, Cesena", "D", 314),
                Spec("000000000315", "AF-P015", PropertyPublicationStatuses.Published, "Lorenzo", "Bilocale pronto zona stazione", "admin-pubblicato-15", "Cesena - Stazione", 750m, 2, 1, 55, "Affitto", "Bilocale", "Stazione", "|furnished|terrace|move-in-ready|near-services|near-public-transport|", "/medri-reference/assets/properties/property-02.jpg", 44.1460d, 12.2475d, "Via Stazione 12, Cesena", "E", 315),
                Spec("000000000316", "IM-P016", PropertyPublicationStatuses.Published, "Marco", "Porzione con terrazzo", "admin-pubblicato-16", "Cesena - Fiorenzuola", 232200m, 4, 2, 106, "Vendita", "Porzione", "Fiorenzuola", "|terrace|parking|move-in-ready|near-services|", "/medri-reference/assets/properties/property-05.jpg", 44.1343d, 12.2561d, "Via Fiorenzuola 24, Cesena", "C", 316),
                Spec("000000000317", "IM-P017", PropertyPublicationStatuses.Published, "Chiara", "Appartamento ultimo piano", "admin-pubblicato-17", "Cesena - Centro storico", 236400m, 3, 1, 84, "Vendita", "Appartamento", "Centro storico", "|terrace|move-in-ready|high-energy-class|near-services|", "/medri-reference/assets/properties/property-04.jpg", 44.1386d, 12.2472d, "Via Centro Storico 15, Cesena", "B", 317),
                Spec("000000000318", "IM-P018", PropertyPublicationStatuses.Published, "Lorenzo", "Trilocale con balcone", "admin-pubblicato-18", "Cesena - Centro", 240600m, 3, 1, 89, "Vendita", "Trilocale", "Centro", "|terrace|move-in-ready|near-services|near-public-transport|", "/medri-reference/assets/properties/property-01.jpg", 44.1401d, 12.2424d, "Via Mura Barriera Ponente 20, Cesena", "C", 318),
                Spec("000000000319", "IM-P019", PropertyPublicationStatuses.Published, "Chiara", "Appartamento con terrazzo", "admin-pubblicato-19", "Cesena - Oltresavio", 244800m, 3, 1, 81, "Vendita", "Appartamento", "Oltresavio", "|terrace|parking|move-in-ready|near-services|", "/medri-reference/assets/properties/property-07.jpg", 44.1282d, 12.2358d, "Via Savio 36, Cesena", "E", 319),
                Spec("000000000320", "AF-P020", PropertyPublicationStatuses.Published, "Marco", "Casa arredata con giardino", "admin-pubblicato-20", "Cesena - Case Finali", 610m, 4, 2, 112, "Affitto", "Casa indipendente", "Case Finali", "|furnished|garden|garage|parking|move-in-ready|", "/medri-reference/assets/properties/property-06.jpg", 44.1558d, 12.2268d, "Via Case Finali 45, Cesena", "D", 320),
                Spec("000000000321", "IM-P021", PropertyPublicationStatuses.Published, "Martina", "Bilocale zona stazione", "admin-pubblicato-21", "Cesena - Stazione", 253200m, 2, 1, 66, "Vendita", "Bilocale", "Stazione", "|terrace|move-in-ready|near-services|near-public-transport|", "/medri-reference/assets/properties/property-02.jpg", 44.1472d, 12.2460d, "Via Stazione 18, Cesena", "E", 321),
                Spec("000000000322", "IM-P022", PropertyPublicationStatuses.Published, "Chiara", "Porzione con terrazzo", "admin-pubblicato-22", "Cesena - Fiorenzuola", 257400m, 4, 2, 108, "Vendita", "Porzione", "Fiorenzuola", "|terrace|parking|move-in-ready|near-services|", "/medri-reference/assets/properties/property-05.jpg", 44.1326d, 12.2544d, "Via Fiorenzuola 30, Cesena", "C", 322),
                Spec("000000000323", "IM-P023", PropertyPublicationStatuses.Published, "Lorenzo", "Appartamento ultimo piano", "admin-pubblicato-23", "Cesena - Centro storico", 261600m, 3, 1, 86, "Vendita", "Appartamento", "Centro storico", "|terrace|move-in-ready|high-energy-class|near-services|", "/medri-reference/assets/properties/property-04.jpg", 44.1398d, 12.2458d, "Via Centro Storico 22, Cesena", "B", 323),
                Spec("000000000324", "IM-P024", PropertyPublicationStatuses.Published, "Marco", "Villetta con giardino", "admin-pubblicato-24", "Cesena - S. Egidio", 265800m, 4, 2, 122, "Vendita", "Villetta", "Sant'Egidio", "|garden|garage|parking|renovation-needed|near-services|", "/medri-reference/assets/properties/property-03.jpg", 44.1540d, 12.2852d, "Via S. Egidio 66, Cesena", "D", 324),
                Spec("000000000325", "AF-P025", PropertyPublicationStatuses.Published, "Chiara", "Appartamento recente arredato", "admin-pubblicato-25", "Cesena - Diegaro", 785m, 3, 1, 74, "Affitto", "Appartamento", "Diegaro", "|furnished|terrace|elevator|move-in-ready|high-energy-class|", "/medri-reference/assets/properties/property-08.jpg", 44.1000d, 12.2133d, "Via Diegaro 19, Cesena", "A", 325),
                Spec("000000000326", "IM-P026", PropertyPublicationStatuses.Published, "Martina", "Casa indipendente con corte", "admin-pubblicato-26", "Cesena - Case Finali", 274200m, 5, 2, 162, "Vendita", "Casa indipendente", "Case Finali", "|garden|garage|parking|renovation-needed|", "/medri-reference/assets/properties/property-06.jpg", 44.1569d, 12.2291d, "Via Case Finali 54, Cesena", "D", 326),
                Spec("000000000327", "IM-P027", PropertyPublicationStatuses.Published, "Lorenzo", "Bilocale zona stazione", "admin-pubblicato-27", "Cesena - Stazione", 278400m, 2, 1, 68, "Vendita", "Bilocale", "Stazione", "|terrace|move-in-ready|near-services|near-public-transport|", "/medri-reference/assets/properties/property-02.jpg", 44.1464d, 12.2483d, "Via Stazione 25, Cesena", "E", 327),
                Spec("000000000328", "IM-P028", PropertyPublicationStatuses.Published, "Marco", "Porzione con terrazzo", "admin-pubblicato-28", "Cesena - Fiorenzuola", 282600m, 4, 2, 110, "Vendita", "Porzione", "Fiorenzuola", "|terrace|parking|move-in-ready|near-services|", "/medri-reference/assets/properties/property-05.jpg", 44.1339d, 12.2558d, "Via Fiorenzuola 38, Cesena", "C", 328),
                Spec("000000000329", "IM-P029", PropertyPublicationStatuses.Published, "Chiara", "Appartamento ultimo piano", "admin-pubblicato-29", "Cesena - Centro storico", 286800m, 3, 1, 88, "Vendita", "Appartamento", "Centro storico", "|terrace|move-in-ready|high-energy-class|near-services|", "/medri-reference/assets/properties/property-04.jpg", 44.1389d, 12.2469d, "Via Centro Storico 31, Cesena", "B", 329),
                Spec("000000000330", "AF-P030", PropertyPublicationStatuses.Published, "Martina", "Mansarda arredata in centro", "admin-pubblicato-30", "Cesena - Centro storico", 645m, 2, 1, 58, "Affitto", "Mansarda", "Centro storico", "|furnished|terrace|move-in-ready|near-services|near-public-transport|", "/medri-reference/assets/properties/property-04.jpg", 44.1394d, 12.2475d, "Via Centro Storico 40, Cesena", "B", 330),
                Spec("000000000331", "IM-P031", PropertyPublicationStatuses.Published, "Lorenzo", "Trilocale con balcone", "admin-pubblicato-31", "Cesena - Centro", 295200m, 3, 1, 91, "Vendita", "Trilocale", "Centro", "|terrace|move-in-ready|near-services|near-public-transport|", "/medri-reference/assets/properties/property-01.jpg", 44.1407d, 12.2420d, "Via Mura Barriera Ponente 28, Cesena", "C", 331),
                Spec("000000000332", "IM-P032", PropertyPublicationStatuses.Published, "Marco", "Quadrilocale in classe A", "admin-pubblicato-32", "Cesena - Diegaro", 299400m, 4, 2, 112, "Vendita", "Appartamento", "Diegaro", "|terrace|elevator|move-in-ready|high-energy-class|", "/medri-reference/assets/properties/property-08.jpg", 44.1011d, 12.2146d, "Via Diegaro 27, Cesena", "A", 332),

                Spec("000000000501", "IM-R001", PropertyPublicationStatuses.Reserved, "Chiara", "Quadrilocale in classe A", "admin-riservato-01", "Cesena - Diegaro", 212500m, 4, 2, 106, "Vendita", "Appartamento", "Diegaro", "|terrace|elevator|move-in-ready|high-energy-class|", "/medri-reference/assets/properties/property-08.jpg", 44.1008d, 12.2144d, "Via Diegaro 31, Cesena", "A", 501),
                Spec("000000000502", "IM-R002", PropertyPublicationStatuses.Reserved, "Martina", "Casa indipendente con corte", "admin-riservato-02", "Cesena - Case Finali", 215000m, 5, 2, 158, "Vendita", "Casa indipendente", "Case Finali", "|garden|garage|parking|renovation-needed|", "/medri-reference/assets/properties/property-06.jpg", 44.1560d, 12.2280d, "Via Case Finali 61, Cesena", "D", 502),
                Spec("000000000503", "IM-R003", PropertyPublicationStatuses.Reserved, "Chiara", "Bilocale zona stazione", "admin-riservato-03", "Cesena - Stazione", 217500m, 2, 1, 64, "Vendita", "Bilocale", "Stazione", "|terrace|move-in-ready|near-services|near-public-transport|", "/medri-reference/assets/properties/property-02.jpg", 44.1469d, 12.2471d, "Via Stazione 31, Cesena", "E", 503),
                Spec("000000000504", "IM-R004", PropertyPublicationStatuses.Reserved, "Martina", "Porzione con terrazzo", "admin-riservato-04", "Cesena - Fiorenzuola", 220000m, 4, 2, 104, "Vendita", "Porzione", "Fiorenzuola", "|terrace|parking|move-in-ready|near-services|", "/medri-reference/assets/properties/property-05.jpg", 44.1331d, 12.2553d, "Via Fiorenzuola 44, Cesena", "C", 504),
                Spec("000000000505", "IM-R005", PropertyPublicationStatuses.Reserved, "Chiara", "Appartamento ultimo piano", "admin-riservato-05", "Cesena - Centro storico", 222500m, 3, 1, 82, "Vendita", "Appartamento", "Centro storico", "|terrace|move-in-ready|high-energy-class|near-services|", "/medri-reference/assets/properties/property-04.jpg", 44.1390d, 12.2466d, "Via Centro Storico 48, Cesena", "B", 505)
            };
        }

        private static SeedListingSpec Spec(
            string idSuffix,
            string reference,
            string publicationStatus,
            string assignedUser,
            string title,
            string slug,
            string displayLocation,
            decimal price,
            int rooms,
            int bathrooms,
            int surfaceSquareMeters,
            string contract,
            string propertyType,
            string zone,
            string featureKeys,
            string imageUrl,
            double latitude,
            double longitude,
            string address,
            string energyClass,
            int sortOrder,
            int? featuredSortOrder = null)
        {
            return new SeedListingSpec(
                idSuffix,
                reference,
                publicationStatus,
                assignedUser,
                title,
                slug,
                displayLocation,
                price,
                rooms,
                bathrooms,
                surfaceSquareMeters,
                contract,
                propertyType,
                zone,
                featureKeys,
                imageUrl,
                latitude,
                longitude,
                address,
                energyClass,
                sortOrder,
                featuredSortOrder);
        }


        private static void SeedFavorites(MedriDbContext context, SeedReferences refs)
        {
            context.FavoriteProperties.AddRange(
                new FavoriteProperty
                {
                    Id = Guid.Parse("40000000-0000-0000-0000-000000000201"),
                    UserId = refs.ClientUserId,
                    PropertyListingId = Guid.Parse("20000000-0000-0000-0000-000000000302"),
                    CreatedAtUtc = refs.SeedDate.AddDays(-3)
                },
                new FavoriteProperty
                {
                    Id = Guid.Parse("40000000-0000-0000-0000-000000000202"),
                    UserId = refs.ClientUserId,
                    PropertyListingId = Guid.Parse("20000000-0000-0000-0000-000000000303"),
                    CreatedAtUtc = refs.SeedDate.AddDays(-2)
                },
                new FavoriteProperty
                {
                    Id = Guid.Parse("40000000-0000-0000-0000-000000000203"),
                    UserId = refs.ClientUserId,
                    PropertyListingId = refs.PropertyListingId,
                    CreatedAtUtc = refs.SeedDate.AddDays(-1)
                });
        }


        private static void SeedNotificationPreferences(MedriDbContext context, SeedReferences refs)
        {
            context.ClientNotificationPreferences.AddRange(
                new ClientNotificationPreference
                {
                    Id = Guid.Parse("40000000-0000-0000-0000-000000000301"),
                    UserId = refs.ClientUserId,
                    Category = ClientNotificationCategories.Requests,
                    IsActive = true,
                    IsDaily = true,
                    IsWeekly = false,
                    UpdatedAtUtc = refs.SeedDate
                },
                new ClientNotificationPreference
                {
                    Id = Guid.Parse("40000000-0000-0000-0000-000000000302"),
                    UserId = refs.ClientUserId,
                    Category = ClientNotificationCategories.Favorites,
                    IsActive = true,
                    IsDaily = true,
                    IsWeekly = false,
                    UpdatedAtUtc = refs.SeedDate
                },
                new ClientNotificationPreference
                {
                    Id = Guid.Parse("40000000-0000-0000-0000-000000000303"),
                    UserId = refs.ClientUserId,
                    Category = ClientNotificationCategories.SavedSearches,
                    IsActive = false,
                    IsDaily = true,
                    IsWeekly = false,
                    UpdatedAtUtc = refs.SeedDate
                });
        }


        private static void SeedStandaloneLeads(MedriDbContext context, SeedReferences refs)
        {
            var seedDate = refs.SeedDate;

            context.Leads.AddRange(
                new Lead
                {
                    Id = refs.LeadId,
                    WorkflowStatus = "Qualified",
                    QualificationPercent = 100,
                    FullName = "Lead seed tecnico",
                    Email = "lead.seed@example.test",
                    Phone = "+390000000000",
                    SourceChannel = "Seed",
                    RequestType = RequestTypes.Seed,
                    CreatedAtUtc = seedDate,
                    UpdatedAtUtc = seedDate
                },
                new Lead
                {
                    Id = refs.PaoloLeadId,
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
                    CreatedAtUtc = seedDate.AddDays(-1),
                    UpdatedAtUtc = seedDate.AddDays(-1)
                },
                new Lead
                {
                    Id = Guid.Parse("30000000-0000-0000-0000-000000000205"),
                    InternalReference = "LD-1181",
                    WorkflowStatus = "InContact",
                    QualificationPercent = 72,
                    NextAction = "Inviare recap con 3 immobili compatibili",
                    AssignedAgencyUserId = refs.Martina,
                    FullName = "Elena Gori",
                    Phone = "347 9988776",
                    SourceChannel = "WhatsApp",
                    RequestType = "Buy",
                    CreatedAtUtc = seedDate.AddHours(-96),
                    UpdatedAtUtc = seedDate.AddHours(-96)
                },
                new Lead
                {
                    Id = Guid.Parse("30000000-0000-0000-0000-000000000206"),
                    InternalReference = "LD-1168",
                    WorkflowStatus = "New",
                    QualificationPercent = 42,
                    NextAction = "Capire budget sostenibile e zona prioritaria",
                    FullName = "Sara Monti",
                    Email = "sara.monti@example.test",
                    Phone = "333 1168000",
                    SourceChannel = "Telefono",
                    RequestType = "Buy",
                    CreatedAtUtc = seedDate.AddHours(-240),
                    UpdatedAtUtc = seedDate.AddHours(-240)
                },
                new Lead
                {
                    Id = Guid.Parse("30000000-0000-0000-0000-000000000207"),
                    InternalReference = "LD-1167",
                    WorkflowStatus = "New",
                    QualificationPercent = 36,
                    NextAction = "Verificare garanzie e tempi di ingresso",
                    FullName = "Giulio Neri",
                    Email = "giulio.neri@example.test",
                    Phone = "333 1167000",
                    SourceChannel = "Email",
                    RequestType = "Rent",
                    CreatedAtUtc = seedDate.AddHours(-264),
                    UpdatedAtUtc = seedDate.AddHours(-264)
                },
                new Lead
                {
                    Id = Guid.Parse("30000000-0000-0000-0000-000000000208"),
                    InternalReference = "LD-1166",
                    WorkflowStatus = "New",
                    QualificationPercent = 51,
                    NextAction = "Raccogliere indirizzo e stato immobile",
                    AssignedAgencyUserId = refs.Chiara,
                    FullName = "Marta Serra",
                    Email = "marta.serra@example.test",
                    Phone = "333 1166000",
                    SourceChannel = "WhatsApp",
                    RequestType = "Sell",
                    CreatedAtUtc = seedDate.AddHours(-288),
                    UpdatedAtUtc = seedDate.AddHours(-288)
                },
                new Lead
                {
                    Id = Guid.Parse("30000000-0000-0000-0000-000000000209"),
                    InternalReference = "LD-1165",
                    WorkflowStatus = "New",
                    QualificationPercent = 40,
                    NextAction = "Fissare primo confronto telefonico",
                    FullName = "Luca Bellini",
                    Email = "luca.bellini@example.test",
                    Phone = "333 1165000",
                    SourceChannel = "Ufficio",
                    RequestType = "Valuation",
                    CreatedAtUtc = seedDate.AddHours(-312),
                    UpdatedAtUtc = seedDate.AddHours(-312)
                },
                new Lead
                {
                    Id = Guid.Parse("30000000-0000-0000-0000-000000000210"),
                    InternalReference = "LD-1164",
                    WorkflowStatus = "New",
                    QualificationPercent = 48,
                    NextAction = "Completare esigenze familiari",
                    AssignedAgencyUserId = refs.Martina,
                    FullName = "Francesca Ricci",
                    Email = "francesca.ricci@example.test",
                    Phone = "333 1164000",
                    SourceChannel = "Telefono",
                    RequestType = "Buy",
                    CreatedAtUtc = seedDate.AddHours(-336),
                    UpdatedAtUtc = seedDate.AddHours(-336)
                },
                new Lead
                {
                    Id = Guid.Parse("30000000-0000-0000-0000-000000000211"),
                    InternalReference = "LD-1163",
                    WorkflowStatus = "New",
                    QualificationPercent = 44,
                    NextAction = "Chiarire disponibilita e contratto desiderato",
                    AssignedAgencyUserId = refs.Chiara,
                    FullName = "Roberto Fini",
                    Email = "roberto.fini@example.test",
                    Phone = "333 1163000",
                    SourceChannel = "Email",
                    RequestType = "RentOut",
                    CreatedAtUtc = seedDate.AddHours(-360),
                    UpdatedAtUtc = seedDate.AddHours(-360)
                },
                new Lead
                {
                    Id = Guid.Parse("30000000-0000-0000-0000-000000000212"),
                    InternalReference = "LD-1162",
                    WorkflowStatus = "InContact",
                    QualificationPercent = 68,
                    NextAction = "Inviare immobili zona Centro",
                    AssignedAgencyUserId = refs.Martina,
                    FullName = "Irene Vitali",
                    Email = "irene.vitali@example.test",
                    Phone = "333 1162000",
                    SourceChannel = "WhatsApp",
                    RequestType = "Buy",
                    CreatedAtUtc = seedDate.AddHours(-384),
                    UpdatedAtUtc = seedDate.AddHours(-384)
                },
                new Lead
                {
                    Id = Guid.Parse("30000000-0000-0000-0000-000000000213"),
                    InternalReference = "LD-1161",
                    WorkflowStatus = "InContact",
                    QualificationPercent = 59,
                    NextAction = "Concordare sopralluogo",
                    AssignedAgencyUserId = refs.Chiara,
                    FullName = "Carlo Benini",
                    Email = "carlo.benini@example.test",
                    Phone = "333 1161000",
                    SourceChannel = "Telefono",
                    RequestType = "Sell",
                    CreatedAtUtc = seedDate.AddHours(-408),
                    UpdatedAtUtc = seedDate.AddHours(-408)
                },
                new Lead
                {
                    Id = Guid.Parse("30000000-0000-0000-0000-000000000214"),
                    InternalReference = "LD-1160",
                    WorkflowStatus = "InContact",
                    QualificationPercent = 63,
                    NextAction = "Richiedere documentazione garanzie",
                    AssignedAgencyUserId = refs.Lorenzo,
                    FullName = "Laura Guidi",
                    Email = "laura.guidi@example.test",
                    Phone = "333 1160000",
                    SourceChannel = "Email",
                    RequestType = "Rent",
                    CreatedAtUtc = seedDate.AddHours(-432),
                    UpdatedAtUtc = seedDate.AddHours(-432)
                },
                new Lead
                {
                    Id = Guid.Parse("30000000-0000-0000-0000-000000000215"),
                    InternalReference = "LD-1159",
                    WorkflowStatus = "InContact",
                    QualificationPercent = 70,
                    NextAction = "Preparare riepilogo valutazione",
                    AssignedAgencyUserId = refs.Chiara,
                    FullName = "Davide Farina",
                    Email = "davide.farina@example.test",
                    Phone = "333 1159000",
                    SourceChannel = "Ufficio",
                    RequestType = "Valuation",
                    CreatedAtUtc = seedDate.AddHours(-456),
                    UpdatedAtUtc = seedDate.AddHours(-456)
                },
                new Lead
                {
                    Id = Guid.Parse("30000000-0000-0000-0000-000000000216"),
                    InternalReference = "LD-1158",
                    WorkflowStatus = "InContact",
                    QualificationPercent = 61,
                    NextAction = "Aggiornare preferenze terrazzo",
                    AssignedAgencyUserId = refs.Martina,
                    FullName = "Silvia Moretti",
                    Email = "silvia.moretti@example.test",
                    Phone = "333 1158000",
                    SourceChannel = "WhatsApp",
                    RequestType = "Buy",
                    CreatedAtUtc = seedDate.AddHours(-480),
                    UpdatedAtUtc = seedDate.AddHours(-480)
                },
                new Lead
                {
                    Id = Guid.Parse("30000000-0000-0000-0000-000000000217"),
                    InternalReference = "LD-1157",
                    WorkflowStatus = "InContact",
                    QualificationPercent = 58,
                    NextAction = "Definire canone atteso",
                    AssignedAgencyUserId = refs.Lorenzo,
                    FullName = "Enrico Berti",
                    Email = "enrico.berti@example.test",
                    Phone = "333 1157000",
                    SourceChannel = "Telefono",
                    RequestType = "RentOut",
                    CreatedAtUtc = seedDate.AddHours(-504),
                    UpdatedAtUtc = seedDate.AddHours(-504)
                },
                new Lead
                {
                    Id = Guid.Parse("30000000-0000-0000-0000-000000000218"),
                    InternalReference = "LD-1156",
                    WorkflowStatus = "InContact",
                    QualificationPercent = 66,
                    NextAction = "Mandare recap compatibilita",
                    AssignedAgencyUserId = refs.Martina,
                    FullName = "Alessia Fontana",
                    Email = "alessia.fontana@example.test",
                    Phone = "333 1156000",
                    SourceChannel = "Email",
                    RequestType = "Buy",
                    CreatedAtUtc = seedDate.AddHours(-528),
                    UpdatedAtUtc = seedDate.AddHours(-528)
                },
                new Lead
                {
                    Id = Guid.Parse("30000000-0000-0000-0000-000000000219"),
                    InternalReference = "LD-1155",
                    WorkflowStatus = "InContact",
                    QualificationPercent = 55,
                    NextAction = "Confermare documenti catastali",
                    AssignedAgencyUserId = refs.Chiara,
                    FullName = "Giorgio Lombardi",
                    Email = "giorgio.lombardi@example.test",
                    Phone = "333 1155000",
                    SourceChannel = "Telefono",
                    RequestType = "Sell",
                    CreatedAtUtc = seedDate.AddHours(-552),
                    UpdatedAtUtc = seedDate.AddHours(-552)
                },
                new Lead
                {
                    Id = Guid.Parse("30000000-0000-0000-0000-000000000226"),
                    InternalReference = "LD-1148",
                    WorkflowStatus = "InContact",
                    QualificationPercent = 57,
                    NextAction = "Richiedere indirizzo preciso per prima valutazione",
                    AssignedAgencyUserId = refs.Chiara,
                    FullName = "Valeria Mancini",
                    Email = "valeria.mancini@example.test",
                    Phone = "333 1148000",
                    SourceChannel = "WhatsApp",
                    RequestType = "Valuation",
                    CreatedAtUtc = seedDate.AddHours(-564),
                    UpdatedAtUtc = seedDate.AddHours(-564)
                },
                new Lead
                {
                    Id = Guid.Parse("30000000-0000-0000-0000-000000000220"),
                    InternalReference = "LD-1154",
                    WorkflowStatus = "Archived",
                    QualificationPercent = 22,
                    NextAction = "Contatto non piu interessato",
                    FullName = "Paola Santi",
                    Email = "paola.santi@example.test",
                    Phone = "333 1154000",
                    SourceChannel = "Email",
                    RequestType = "Buy",
                    CreatedAtUtc = seedDate.AddHours(-576),
                    UpdatedAtUtc = seedDate.AddHours(-576)
                },
                new Lead
                {
                    Id = Guid.Parse("30000000-0000-0000-0000-000000000221"),
                    InternalReference = "LD-1153",
                    WorkflowStatus = "Archived",
                    QualificationPercent = 18,
                    NextAction = "Budget non compatibile",
                    FullName = "Andrea Russo",
                    Email = "andrea.russo@example.test",
                    Phone = "333 1153000",
                    SourceChannel = "Telefono",
                    RequestType = "Rent",
                    CreatedAtUtc = seedDate.AddHours(-600),
                    UpdatedAtUtc = seedDate.AddHours(-600)
                },
                new Lead
                {
                    Id = Guid.Parse("30000000-0000-0000-0000-000000000222"),
                    InternalReference = "LD-1152",
                    WorkflowStatus = "Archived",
                    QualificationPercent = 25,
                    NextAction = "Valutazione rinviata",
                    FullName = "Monica De Angelis",
                    Email = "monica.deangelis@example.test",
                    Phone = "333 1152000",
                    SourceChannel = "WhatsApp",
                    RequestType = "Valuation",
                    CreatedAtUtc = seedDate.AddHours(-624),
                    UpdatedAtUtc = seedDate.AddHours(-624)
                },
                new Lead
                {
                    Id = Guid.Parse("30000000-0000-0000-0000-000000000223"),
                    InternalReference = "LD-1151",
                    WorkflowStatus = "Archived",
                    QualificationPercent = 20,
                    NextAction = "Ha scelto altra agenzia",
                    FullName = "Stefano Riva",
                    Email = "stefano.riva@example.test",
                    Phone = "333 1151000",
                    SourceChannel = "Ufficio",
                    RequestType = "Sell",
                    CreatedAtUtc = seedDate.AddHours(-648),
                    UpdatedAtUtc = seedDate.AddHours(-648)
                },
                new Lead
                {
                    Id = Guid.Parse("30000000-0000-0000-0000-000000000224"),
                    InternalReference = "LD-1150",
                    WorkflowStatus = "Archived",
                    QualificationPercent = 15,
                    NextAction = "Richiesta non coltivabile",
                    FullName = "Claudia Gatti",
                    Email = "claudia.gatti@example.test",
                    Phone = "333 1150000",
                    SourceChannel = "Email",
                    RequestType = "Buy",
                    CreatedAtUtc = seedDate.AddHours(-672),
                    UpdatedAtUtc = seedDate.AddHours(-672)
                },
                new Lead
                {
                    Id = Guid.Parse("30000000-0000-0000-0000-000000000225"),
                    InternalReference = "LD-1149",
                    WorkflowStatus = "Archived",
                    QualificationPercent = 19,
                    NextAction = "Immobile non disponibile",
                    FullName = "Filippo Marchetti",
                    Email = "filippo.marchetti@example.test",
                    Phone = "333 1149000",
                    SourceChannel = "Telefono",
                    RequestType = "RentOut",
                    CreatedAtUtc = seedDate.AddHours(-696),
                    UpdatedAtUtc = seedDate.AddHours(-696)
                });

            context.LeadPreferences.AddRange(
                new LeadPreference
                {
                    Id = Guid.Parse("32000000-0000-0000-0000-000000000001"),
                    LeadId = refs.LeadId,
                    MinimumPrice = 150000m,
                    MaximumPrice = 300000m,
                    DesiredLocation = "Area seed",
                    MinimumRooms = 3
                },
                new LeadPreference
                {
                    Id = Guid.Parse("32000000-0000-0000-0000-000000000201"),
                    LeadId = refs.PaoloLeadId,
                    Timing = "Da capire",
                    ValuationGoal = "Vuole capire se vendere entro l'anno",
                    PreferencesAndCompromises = "Zona precisa, metratura indicativa, stato dell'immobile, tempi desiderati, eventuali vincoli familiari."
                });

            context.Interactions.AddRange(
                new Interaction
                {
                    Id = Guid.Parse("33000000-0000-0000-0000-000000000201"),
                    LeadId = refs.PaoloLeadId,
                    Channel = "Telefonata registrata",
                    Notes = "Michela ha annotato il primo contatto e i dati minimi disponibili.",
                    OccurredAtUtc = seedDate.AddDays(-1).Date.AddHours(10).AddMinutes(42)
                },
                new Interaction
                {
                    Id = Guid.Parse("33000000-0000-0000-0000-000000000202"),
                    LeadId = refs.PaoloLeadId,
                    Channel = "Prossima azione proposta",
                    Notes = "Richiamare per completare zona, tempi e caratteristiche dell'immobile.",
                    OccurredAtUtc = seedDate.AddDays(-1).Date.AddHours(10).AddMinutes(45)
                });
        }


        private static void SeedRequests(MedriDbContext context, SeedReferences refs)
        {
            var seedDate = refs.SeedDate;

            var requests = new[]
            {
                (
                    Lead: new Lead
                    {
                        Id = refs.ClientBuyLeadId,
                        ClientUserId = refs.ClientUserId,
                        PublicReference = "RQ-2047",
                        WorkflowStatus = LeadWorkflowStatuses.Qualified,
                        AssignedAgencyUserId = refs.Martina,
                        FullName = "Elena Gori",
                        Email = "elena.gori@email.it",
                        Phone = "347 9988776",
                        SourceChannel = "Lead convertito",
                        RequestType = RequestTypes.Buy,
                        Notes = "3 immobili proposti",
                        CreatedAtUtc = seedDate.AddDays(-2),
                        UpdatedAtUtc = seedDate.AddDays(-2)
                    },
                    Preference: new LeadPreference
                    {
                        Id = Guid.Parse("32000000-0000-0000-0000-000000000101"),
                        LeadId = refs.ClientBuyLeadId,
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
                    Profile: new SearchProfile
                    {
                        Id = Guid.Parse("31000000-0000-0000-0000-000000000101"),
                        LeadId = refs.ClientBuyLeadId,
                        PublicReference = "RQ-2047",
                        Status = RequestStatuses.InMatching,
                        CriteriaSummary = "Cesena Centro|max EUR 260.000|2 camere",
                        ContactEmail = "elena.gori@email.it",
                        SourceQueryString = "da lead LD-1181",
                        CreatedAtUtc = seedDate.AddDays(-2),
                        UpdatedAtUtc = seedDate.Date.AddHours(11).AddMinutes(20)
                    }
                ),
                (
                    Lead: new Lead
                    {
                        Id = refs.ClientValuationLeadId,
                        ClientUserId = refs.ClientUserId,
                        PublicReference = "RQ-2040",
                        WorkflowStatus = LeadWorkflowStatuses.Qualified,
                        AssignedAgencyUserId = refs.Chiara,
                        FullName = "Elena Gori",
                        Email = "elena.gori@email.it",
                        Phone = "333 2034000",
                        SourceChannel = "Public lead intake",
                        RequestType = RequestTypes.Valuation,
                        Notes = "Da ricontattare",
                        CreatedAtUtc = seedDate.AddDays(-8),
                        UpdatedAtUtc = seedDate.AddDays(-8)
                    },
                    Preference: new LeadPreference
                    {
                        Id = Guid.Parse("32000000-0000-0000-0000-000000000102"),
                        LeadId = refs.ClientValuationLeadId,
                        DesiredLocation = "Diegaro",
                        PropertyType = "Casa familiare",
                        ValuationGoal = "Prima valutazione",
                        Timing = "Media"
                    },
                    Profile: new SearchProfile
                    {
                        Id = Guid.Parse("31000000-0000-0000-0000-000000000105"),
                        LeadId = refs.ClientValuationLeadId,
                        PublicReference = "RQ-2040",
                        Status = RequestStatuses.New,
                        CriteriaSummary = "Valutazione|Diegaro|prima valutazione",
                        ContactEmail = "elena.gori@email.it",
                        SourceQueryString = "Public lead intake",
                        CreatedAtUtc = seedDate.AddDays(-8),
                        UpdatedAtUtc = seedDate.AddDays(-8)
                    }
                ),
                (
                    Lead: new Lead
                    {
                        Id = refs.NiccoloLeadId,
                        InternalReference = "LD-1176",
                        PublicReference = "RQ-2039",
                        WorkflowStatus = LeadWorkflowStatuses.Qualified,
                        NextAction = "Chiedere budget massimo e garanzie",
                        AssignedAgencyUserId = refs.Lorenzo,
                        FullName = "Niccolo Fabbri",
                        Email = "niccolo.f@email.it",
                        SourceChannel = "Email",
                        RequestType = RequestTypes.Rent,
                        CreatedAtUtc = seedDate.AddDays(-2),
                        UpdatedAtUtc = seedDate.AddDays(-2)
                    },
                    Preference: new LeadPreference
                    {
                        Id = Guid.Parse("32000000-0000-0000-0000-000000000202"),
                        LeadId = refs.NiccoloLeadId,
                        PropertyType = "Bilocale",
                        DesiredLocation = "vicino campus",
                        SustainableBudgetLabel = "max EUR 750",
                        Timing = "Media"
                    },
                    Profile: new SearchProfile
                    {
                        Id = Guid.Parse("31000000-0000-0000-0000-000000000104"),
                        LeadId = refs.NiccoloLeadId,
                        PublicReference = "RQ-2039",
                        Status = RequestStatuses.New,
                        CriteriaSummary = "Bilocale|vicino campus|max EUR 750",
                        ContactEmail = "niccolo.f@email.it",
                        SourceQueryString = "Affitto studenti - email",
                        CreatedAtUtc = seedDate.AddDays(-2),
                        UpdatedAtUtc = seedDate.Date.AddDays(-1).AddHours(16).AddMinutes(40)
                    }
                ),
                (
                    Lead: new Lead
                    {
                        Id = refs.AnnaLeadId,
                        InternalReference = "LD-1169",
                        PublicReference = "RQ-2034",
                        WorkflowStatus = LeadWorkflowStatuses.Qualified,
                        NextAction = "Completare recapito e indirizzo immobile",
                        FullName = "Anna Conti",
                        Email = "anna.conti@example.test",
                        SourceChannel = "Ufficio",
                        RequestType = RequestTypes.Valuation,
                        Notes = "Valutazione casa familiare",
                        CreatedAtUtc = seedDate.AddDays(-3),
                        UpdatedAtUtc = seedDate.AddDays(-3)
                    },
                    Preference: new LeadPreference
                    {
                        Id = Guid.Parse("32000000-0000-0000-0000-000000000203"),
                        LeadId = refs.AnnaLeadId,
                        PropertyType = "Sopralluogo",
                        DesiredLocation = "Cesena - Diegaro",
                        ValuationGoal = "entro mese",
                        Timing = "Alta"
                    },
                    Profile: new SearchProfile
                    {
                        Id = Guid.Parse("31000000-0000-0000-0000-000000000102"),
                        LeadId = refs.AnnaLeadId,
                        PublicReference = "RQ-2034",
                        Status = RequestStatuses.New,
                        CriteriaSummary = "Sopralluogo|Cesena - Diegaro|entro mese",
                        ContactEmail = "anna.conti@example.test",
                        SourceQueryString = "Valutazione casa familiare",
                        CreatedAtUtc = seedDate.AddDays(-3),
                        UpdatedAtUtc = seedDate.Date.AddHours(9).AddMinutes(10)
                    }
                ),
                (
                    Lead: new Lead
                    {
                        Id = refs.MarcoLeadId,
                        PublicReference = "RQ-2028",
                        WorkflowStatus = LeadWorkflowStatuses.Qualified,
                        FullName = "Marco Guidi",
                        Email = "marco.guidi@example.test",
                        Phone = "333 2028000",
                        SourceChannel = "Agency",
                        RequestType = RequestTypes.Sell,
                        AssignedAgencyUserId = refs.Chiara,
                        Notes = "Vuole vendere appartamento",
                        CreatedAtUtc = seedDate.AddDays(-4),
                        UpdatedAtUtc = seedDate.AddDays(-4)
                    },
                    Preference: new LeadPreference
                    {
                        Id = Guid.Parse("32000000-0000-0000-0000-000000000204"),
                        LeadId = refs.MarcoLeadId,
                        PropertyToSellStatus = "Documenti",
                        PreferencesAndCompromises = "foto mancanti",
                        ExpectedPriceOrMainQuestion = "prezzo da validare",
                        Timing = "Media"
                    },
                    Profile: new SearchProfile
                    {
                        Id = Guid.Parse("31000000-0000-0000-0000-000000000103"),
                        LeadId = refs.MarcoLeadId,
                        PublicReference = "RQ-2028",
                        Status = RequestStatuses.Updating,
                        CriteriaSummary = "Documenti|foto mancanti|prezzo da validare",
                        ContactEmail = "marco.guidi@example.test",
                        SourceQueryString = "",
                        CreatedAtUtc = seedDate.AddDays(-4),
                        UpdatedAtUtc = new DateTime(2026, 4, 26, 12, 15, 0, DateTimeKind.Utc)
                    }
                ),
                (
                    Lead: new Lead
                    {
                        Id = Guid.Parse("30000000-0000-0000-0000-000000000301"),
                        PublicReference = "RQ-2027",
                        WorkflowStatus = LeadWorkflowStatuses.Qualified,
                        FullName = "Sara Monti",
                        Email = "sara.monti@example.test",
                        Phone = "333 2027000",
                        SourceChannel = "Agency",
                        RequestType = RequestTypes.Buy,
                        Notes = "Richiesta da primo colloquio",
                        CreatedAtUtc = seedDate.AddDays(-5),
                        UpdatedAtUtc = seedDate.AddDays(-5)
                    },
                    Preference: new LeadPreference
                    {
                        Id = Guid.Parse("32000000-0000-0000-0000-000000000301"),
                        LeadId = Guid.Parse("30000000-0000-0000-0000-000000000301"),
                        DesiredLocation = "Centro storico",
                        SustainableBudgetLabel = "budget da definire",
                        ExpectedPriceOrMainQuestion = "prima visita",
                        Timing = "Alta"
                    },
                    Profile: new SearchProfile
                    {
                        Id = Guid.Parse("31000000-0000-0000-0000-000000000201"),
                        LeadId = Guid.Parse("30000000-0000-0000-0000-000000000301"),
                        PublicReference = "RQ-2027",
                        Status = RequestStatuses.New,
                        CriteriaSummary = "Centro storico|budget da definire|prima visita",
                        ContactEmail = "sara.monti@example.test",
                        SourceQueryString = "Richiesta da primo colloquio",
                        CreatedAtUtc = seedDate.AddDays(-5),
                        UpdatedAtUtc = seedDate.AddDays(-5)
                    }
                ),
                (
                    Lead: new Lead
                    {
                        Id = Guid.Parse("30000000-0000-0000-0000-000000000302"),
                        PublicReference = "RQ-2026",
                        WorkflowStatus = LeadWorkflowStatuses.Qualified,
                        FullName = "Giulio Neri",
                        Email = "giulio.neri@example.test",
                        Phone = "333 2026000",
                        SourceChannel = "Agency",
                        RequestType = RequestTypes.Rent,
                        Notes = "Richiesta affitto da qualificare",
                        CreatedAtUtc = seedDate.AddDays(-6),
                        UpdatedAtUtc = seedDate.AddDays(-6)
                    },
                    Preference: new LeadPreference
                    {
                        Id = Guid.Parse("32000000-0000-0000-0000-000000000302"),
                        LeadId = Guid.Parse("30000000-0000-0000-0000-000000000302"),
                        PropertyType = "Bilocale",
                        DesiredLocation = "zona universitaria",
                        SustainableBudgetLabel = "max EUR 750",
                        AvailableGuarantees = "garanzie da verificare",
                        DesiredMoveIn = "ingresso rapido"
                    },
                    Profile: new SearchProfile
                    {
                        Id = Guid.Parse("31000000-0000-0000-0000-000000000202"),
                        LeadId = Guid.Parse("30000000-0000-0000-0000-000000000302"),
                        PublicReference = "RQ-2026",
                        Status = RequestStatuses.New,
                        CriteriaSummary = "Affitto|garanzie da verificare|ingresso rapido",
                        ContactEmail = "giulio.neri@example.test",
                        SourceQueryString = "Richiesta affitto da qualificare",
                        CreatedAtUtc = seedDate.AddDays(-6),
                        UpdatedAtUtc = seedDate.AddDays(-6)
                    }
                ),
                (
                    Lead: new Lead
                    {
                        Id = Guid.Parse("30000000-0000-0000-0000-000000000303"),
                        PublicReference = "RQ-2025",
                        WorkflowStatus = LeadWorkflowStatuses.Qualified,
                        AssignedAgencyUserId = refs.Martina,
                        FullName = "Marta Serra",
                        Email = "marta.serra@example.test",
                        Phone = "333 2025000",
                        SourceChannel = "Agency",
                        RequestType = RequestTypes.Sell,
                        Notes = "Profilo pronto per matching proprietario",
                        CreatedAtUtc = seedDate.AddDays(-7),
                        UpdatedAtUtc = seedDate.AddDays(-7)
                    },
                    Preference: new LeadPreference
                    {
                        Id = Guid.Parse("32000000-0000-0000-0000-000000000303"),
                        LeadId = Guid.Parse("30000000-0000-0000-0000-000000000303"),
                        PropertyType = "Trilocale",
                        DesiredLocation = "zona Stadio",
                        Appurtenances = "box auto",
                        PropertyToSellStatus = "Profilo pronto per matching proprietario"
                    },
                    Profile: new SearchProfile
                    {
                        Id = Guid.Parse("31000000-0000-0000-0000-000000000203"),
                        LeadId = Guid.Parse("30000000-0000-0000-0000-000000000303"),
                        PublicReference = "RQ-2025",
                        Status = RequestStatuses.InMatching,
                        CriteriaSummary = "Trilocale|zona Stadio|box auto",
                        ContactEmail = "marta.serra@example.test",
                        SourceQueryString = "Profilo pronto per matching proprietario",
                        CreatedAtUtc = seedDate.AddDays(-7),
                        UpdatedAtUtc = seedDate.AddDays(-7)
                    }
                ),
                (
                    Lead: new Lead
                    {
                        Id = Guid.Parse("30000000-0000-0000-0000-000000000304"),
                        PublicReference = "RQ-2024",
                        WorkflowStatus = LeadWorkflowStatuses.Qualified,
                        AssignedAgencyUserId = refs.Chiara,
                        FullName = "Luca Bellini",
                        Email = "luca.bellini@example.test",
                        Phone = "333 2024000",
                        SourceChannel = "Agency",
                        RequestType = RequestTypes.Valuation,
                        Notes = "Richiesta valutazione assegnata",
                        CreatedAtUtc = seedDate.AddDays(-8),
                        UpdatedAtUtc = seedDate.AddDays(-8)
                    },
                    Preference: new LeadPreference
                    {
                        Id = Guid.Parse("32000000-0000-0000-0000-000000000304"),
                        LeadId = Guid.Parse("30000000-0000-0000-0000-000000000304"),
                        PropertyType = "Sopralluogo",
                        DesiredLocation = "zona Borello",
                        ValuationGoal = "documenti pronti",
                        Timing = "Alta"
                    },
                    Profile: new SearchProfile
                    {
                        Id = Guid.Parse("31000000-0000-0000-0000-000000000204"),
                        LeadId = Guid.Parse("30000000-0000-0000-0000-000000000304"),
                        PublicReference = "RQ-2024",
                        Status = RequestStatuses.InMatching,
                        CriteriaSummary = "Sopralluogo|zona Borello|documenti pronti",
                        ContactEmail = "luca.bellini@example.test",
                        SourceQueryString = "Richiesta valutazione assegnata",
                        CreatedAtUtc = seedDate.AddDays(-8),
                        UpdatedAtUtc = seedDate.AddDays(-8)
                    }
                ),
                (
                    Lead: new Lead
                    {
                        Id = Guid.Parse("30000000-0000-0000-0000-000000000305"),
                        PublicReference = "RQ-2023",
                        WorkflowStatus = LeadWorkflowStatuses.Qualified,
                        AssignedAgencyUserId = refs.Lorenzo,
                        FullName = "Francesca Ricci",
                        Email = "francesca.ricci@example.test",
                        Phone = "333 2023000",
                        SourceChannel = "Agency",
                        RequestType = RequestTypes.Buy,
                        Notes = "Profilo acquisto assegnato",
                        CreatedAtUtc = seedDate.AddDays(-9),
                        UpdatedAtUtc = seedDate.AddDays(-9)
                    },
                    Preference: new LeadPreference
                    {
                        Id = Guid.Parse("32000000-0000-0000-0000-000000000305"),
                        LeadId = Guid.Parse("30000000-0000-0000-0000-000000000305"),
                        PropertyType = "Villetta",
                        DesiredLocation = "Cesena",
                        SustainableBudgetLabel = "max EUR 320.000",
                        PreferencesAndCompromises = "giardino",
                        FinancingStatus = "mutuo avviato"
                    },
                    Profile: new SearchProfile
                    {
                        Id = Guid.Parse("31000000-0000-0000-0000-000000000205"),
                        LeadId = Guid.Parse("30000000-0000-0000-0000-000000000305"),
                        PublicReference = "RQ-2023",
                        Status = RequestStatuses.InMatching,
                        CriteriaSummary = "Villetta|giardino|mutuo avviato",
                        ContactEmail = "francesca.ricci@example.test",
                        SourceQueryString = "Profilo acquisto assegnato",
                        CreatedAtUtc = seedDate.AddDays(-9),
                        UpdatedAtUtc = seedDate.AddDays(-9)
                    }
                ),
                (
                    Lead: new Lead
                    {
                        Id = Guid.Parse("30000000-0000-0000-0000-000000000306"),
                        PublicReference = "RQ-2022",
                        WorkflowStatus = LeadWorkflowStatuses.Qualified,
                        FullName = "Roberto Fini",
                        Email = "roberto.fini@example.test",
                        Phone = "333 2022000",
                        SourceChannel = "Agency",
                        RequestType = RequestTypes.RentOut,
                        Notes = "Ricerca in matching da affinare",
                        CreatedAtUtc = seedDate.AddDays(-10),
                        UpdatedAtUtc = seedDate.AddDays(-10)
                    },
                    Preference: new LeadPreference
                    {
                        Id = Guid.Parse("32000000-0000-0000-0000-000000000306"),
                        LeadId = Guid.Parse("30000000-0000-0000-0000-000000000306"),
                        PropertyType = "Affitto gestito",
                        DesiredContractType = "contratto concordato",
                        PropertyCondition = "arredo parziale"
                    },
                    Profile: new SearchProfile
                    {
                        Id = Guid.Parse("31000000-0000-0000-0000-000000000206"),
                        LeadId = Guid.Parse("30000000-0000-0000-0000-000000000306"),
                        PublicReference = "RQ-2022",
                        Status = RequestStatuses.InMatching,
                        CriteriaSummary = "Affitto gestito|contratto concordato|arredo parziale",
                        ContactEmail = "roberto.fini@example.test",
                        SourceQueryString = "Ricerca in matching da affinare",
                        CreatedAtUtc = seedDate.AddDays(-10),
                        UpdatedAtUtc = seedDate.AddDays(-10)
                    }
                ),
                (
                    Lead: new Lead
                    {
                        Id = Guid.Parse("30000000-0000-0000-0000-000000000307"),
                        PublicReference = "RQ-2021",
                        WorkflowStatus = LeadWorkflowStatuses.Qualified,
                        FullName = "Irene Vitali",
                        Email = "irene.vitali@example.test",
                        Phone = "333 2021000",
                        SourceChannel = "Agency",
                        RequestType = RequestTypes.Buy,
                        Notes = "Richiesta in matching",
                        CreatedAtUtc = seedDate.AddDays(-11),
                        UpdatedAtUtc = seedDate.AddDays(-11)
                    },
                    Preference: new LeadPreference
                    {
                        Id = Guid.Parse("32000000-0000-0000-0000-000000000307"),
                        LeadId = Guid.Parse("30000000-0000-0000-0000-000000000307"),
                        PropertyType = "Casa indipendente",
                        DesiredLocation = "Ponte Abbadesse",
                        Appurtenances = "garage"
                    },
                    Profile: new SearchProfile
                    {
                        Id = Guid.Parse("31000000-0000-0000-0000-000000000207"),
                        LeadId = Guid.Parse("30000000-0000-0000-0000-000000000307"),
                        PublicReference = "RQ-2021",
                        Status = RequestStatuses.InMatching,
                        CriteriaSummary = "Casa indipendente|zona Ponte Abbadesse|garage",
                        ContactEmail = "irene.vitali@example.test",
                        SourceQueryString = "Richiesta in matching",
                        CreatedAtUtc = seedDate.AddDays(-11),
                        UpdatedAtUtc = seedDate.AddDays(-11)
                    }
                ),
                (
                    Lead: new Lead
                    {
                        Id = Guid.Parse("30000000-0000-0000-0000-000000000308"),
                        PublicReference = "RQ-2020",
                        WorkflowStatus = LeadWorkflowStatuses.Qualified,
                        FullName = "Carlo Benini",
                        Email = "carlo.benini@example.test",
                        Phone = "333 2020000",
                        SourceChannel = "Agency",
                        RequestType = RequestTypes.Sell,
                        Notes = "Profilo vendita in matching",
                        CreatedAtUtc = seedDate.AddDays(-12),
                        UpdatedAtUtc = seedDate.AddDays(-12)
                    },
                    Preference: new LeadPreference
                    {
                        Id = Guid.Parse("32000000-0000-0000-0000-000000000308"),
                        LeadId = Guid.Parse("30000000-0000-0000-0000-000000000308"),
                        PropertyType = "Appartamento",
                        PropertyCondition = "foto da produrre",
                        ExpectedPriceOrMainQuestion = "prezzo coerente"
                    },
                    Profile: new SearchProfile
                    {
                        Id = Guid.Parse("31000000-0000-0000-0000-000000000208"),
                        LeadId = Guid.Parse("30000000-0000-0000-0000-000000000308"),
                        PublicReference = "RQ-2020",
                        Status = RequestStatuses.InMatching,
                        CriteriaSummary = "Vendita appartamento|foto da produrre|prezzo coerente",
                        ContactEmail = "carlo.benini@example.test",
                        SourceQueryString = "Profilo vendita in matching",
                        CreatedAtUtc = seedDate.AddDays(-12),
                        UpdatedAtUtc = seedDate.AddDays(-12)
                    }
                ),
                (
                    Lead: new Lead
                    {
                        Id = Guid.Parse("30000000-0000-0000-0000-000000000309"),
                        PublicReference = "RQ-2019",
                        WorkflowStatus = LeadWorkflowStatuses.Qualified,
                        FullName = "Laura Guidi",
                        Email = "laura.guidi@example.test",
                        Phone = "333 2019000",
                        SourceChannel = "Agency",
                        RequestType = RequestTypes.Rent,
                        Notes = "Aggiornare preferenze canone",
                        CreatedAtUtc = seedDate.AddDays(-13),
                        UpdatedAtUtc = seedDate.AddDays(-13)
                    },
                    Preference: new LeadPreference
                    {
                        Id = Guid.Parse("32000000-0000-0000-0000-000000000309"),
                        LeadId = Guid.Parse("30000000-0000-0000-0000-000000000309"),
                        PropertyType = "Bilocale",
                        SustainableBudgetLabel = "canone aggiornato",
                        AvailableGuarantees = "garanzie"
                    },
                    Profile: new SearchProfile
                    {
                        Id = Guid.Parse("31000000-0000-0000-0000-000000000209"),
                        LeadId = Guid.Parse("30000000-0000-0000-0000-000000000309"),
                        PublicReference = "RQ-2019",
                        Status = RequestStatuses.Updating,
                        CriteriaSummary = "Bilocale|canone aggiornato|garanzie",
                        ContactEmail = "laura.guidi@example.test",
                        SourceQueryString = "Aggiornare preferenze canone",
                        CreatedAtUtc = seedDate.AddDays(-13),
                        UpdatedAtUtc = seedDate.AddDays(-13)
                    }
                ),
                (
                    Lead: new Lead
                    {
                        Id = Guid.Parse("30000000-0000-0000-0000-000000000310"),
                        PublicReference = "RQ-2018",
                        WorkflowStatus = LeadWorkflowStatuses.Qualified,
                        FullName = "Davide Farina",
                        Email = "davide.farina@example.test",
                        Phone = "333 2018000",
                        SourceChannel = "Agency",
                        RequestType = RequestTypes.Valuation,
                        Notes = "Aggiornare obiettivo valutazione",
                        CreatedAtUtc = seedDate.AddDays(-14),
                        UpdatedAtUtc = seedDate.AddDays(-14)
                    },
                    Preference: new LeadPreference
                    {
                        Id = Guid.Parse("32000000-0000-0000-0000-000000000310"),
                        LeadId = Guid.Parse("30000000-0000-0000-0000-000000000310"),
                        ValuationGoal = "obiettivo cambiato",
                        ExpectedPriceOrMainQuestion = "richiamare"
                    },
                    Profile: new SearchProfile
                    {
                        Id = Guid.Parse("31000000-0000-0000-0000-000000000210"),
                        LeadId = Guid.Parse("30000000-0000-0000-0000-000000000310"),
                        PublicReference = "RQ-2018",
                        Status = RequestStatuses.Updating,
                        CriteriaSummary = "Valutazione|obiettivo cambiato|richiamare",
                        ContactEmail = "davide.farina@example.test",
                        SourceQueryString = "Aggiornare obiettivo valutazione",
                        CreatedAtUtc = seedDate.AddDays(-14),
                        UpdatedAtUtc = seedDate.AddDays(-14)
                    }
                )
            };

            foreach (var request in requests)
            {
                request.Lead.QualificationPercent = LeadQualificationCalculator.Calculate(
                    request.Lead,
                    request.Preference);

                context.Leads.Add(request.Lead);
                context.LeadPreferences.Add(request.Preference);
                context.SearchProfiles.Add(request.Profile);
            }

            context.Interactions.AddRange(
                new Interaction
                {
                    Id = Guid.Parse("33000000-0000-0000-0000-000000000101"),
                    LeadId = refs.ClientBuyLeadId,
                    Channel = "Preferenze aggiornate",
                    Notes = "Aggiunto vincolo ascensore per piani alti e maggiore attenzione a spese condominiali.",
                    OccurredAtUtc = seedDate.Date.AddHours(11).AddMinutes(20)
                },
                new Interaction
                {
                    Id = Guid.Parse("33000000-0000-0000-0000-000000000102"),
                    LeadId = refs.ClientBuyLeadId,
                    Channel = "Richiesta convertita da lead",
                    Notes = "Martina ha strutturato budget, zone valutabili e vincoli minimi dalla conversazione WhatsApp.",
                    OccurredAtUtc = seedDate.Date.AddDays(-1).AddHours(17).AddMinutes(5)
                },
                new Interaction
                {
                    Id = Guid.Parse("33000000-0000-0000-0000-000000000103"),
                    LeadId = refs.ClientBuyLeadId,
                    Channel = "Primo contatto",
                    Notes = "Cliente interessata a ricevere proposte selezionate, non una lista generica di annunci.",
                    OccurredAtUtc = new DateTime(2026, 4, 26, 12, 0, 0, DateTimeKind.Utc)
                });
        }


        private static void SeedAppointments(MedriDbContext context, SeedReferences refs)
        {
            var seedDate = refs.SeedDate;

            context.Appointments.Add(new Appointment
            {
                Id = Guid.Parse("35000000-0000-0000-0000-000000000001"),
                LeadId = refs.LeadId,
                PropertyListingId = refs.PropertyListingId,
                AgencyUserId = refs.Chiara,
                ScheduledAtUtc = seedDate.AddDays(1),
                Status = "Scheduled",
                RequestType = "Seed",
                CreatedAtUtc = seedDate
            });

            context.Appointments.AddRange(
                new Appointment
                {
                    Id = Guid.Parse("35000000-0000-0000-0000-000000000101"),
                    LeadId = refs.ClientBuyLeadId,
                    AgencyUserId = refs.Martina,
                    Status = "InMatching",
                    RequestType = "GeneralContact",
                    CreatedAtUtc = seedDate.AddDays(-2)
                },
                new Appointment
                {
                    Id = Guid.Parse("35000000-0000-0000-0000-000000000102"),
                    LeadId = refs.ClientValuationLeadId,
                    AgencyUserId = refs.Chiara,
                    Status = "Received",
                    RequestType = "GeneralContact",
                    CreatedAtUtc = seedDate.AddDays(-8)
                });
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
                    Math.Max(1, listing.Rooms - 1).ToString(CultureInfo.InvariantCulture));
                listing.FloorLabel = Fill(listing.FloorLabel, ordinal % 4 == 0 ? "Piano terra" : ordinal % 3 == 0 ? "Piano alto" : "Piano intermedio");
                listing.ElevatorLabel = Fill(listing.ElevatorLabel, ordinal % 3 == 0 ? "Ascensore presente" : "Ascensore da verificare");
                listing.GarageLabel = Fill(listing.GarageLabel, listing.FeatureKeys.Contains("|garage|") ? "Garage" : "Posto auto");
                listing.OutdoorSpaceLabel = Fill(listing.OutdoorSpaceLabel, listing.FeatureKeys.Contains("|garden|") ? "Giardino" : "Balcone");
                listing.EnergyClass = Fill(listing.EnergyClass, ordinal % 4 == 0 ? "D" : "C");
                listing.AvailabilityLabel = Fill(listing.AvailabilityLabel, string.Equals(listing.Contract, "Affitto", StringComparison.OrdinalIgnoreCase) ? "Ingresso da concordare" : "Disponibile a rogito");
                listing.HeatingLabel = Fill(listing.HeatingLabel, "Autonomo");
                listing.RequiredWorksLabel = Fill(listing.RequiredWorksLabel, listing.FeatureKeys.Contains("|renovation-needed|") ? "Aggiornamenti da prevedere" : "Buono");
                listing.ConstructionYearLabel = Fill(listing.ConstructionYearLabel, (1996 + (ordinal % 24)).ToString(CultureInfo.InvariantCulture));
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
                listing.SurfaceRoomsComparisonLabel = Fill(listing.SurfaceRoomsComparisonLabel, $"{listing.SurfaceSquareMeters.ToString(CultureInfo.InvariantCulture)} mq - {listing.Rooms.ToString(CultureInfo.InvariantCulture)} locali");
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
            return $"{listing.Title} {contractText} a {listing.DisplayLocation}: {listing.SurfaceSquareMeters.ToString(CultureInfo.InvariantCulture)} mq distribuiti in {listing.Rooms.ToString(CultureInfo.InvariantCulture)} locali, con {listing.OutdoorSpaceLabel.ToLowerInvariant()} e servizi raggiungibili nella quotidianita.";
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
                var targetCount = SeedGalleryTargetCount(listing);

                if (existingSortOrders.Count >= targetCount)
                {
                    continue;
                }

                for (var sortOrder = 1; sortOrder <= targetCount; sortOrder++)
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

        private static void ApplySeedCompletion(
            PropertyListing[] listings,
            IEnumerable<PropertyMedia> media)
        {
            var mediaCounts = media
                .GroupBy(item => item.PropertyListingId)
                .ToDictionary(group => group.Key, group => group.Count());

            foreach (var listing in listings)
            {
                mediaCounts.TryGetValue(listing.Id, out var mediaCount);
                AdminPropertyCompletionCalculator.ApplyToListing(
                    listing,
                    AdminPropertyCompletionCalculator.Calculate(listing, mediaCount));
            }
        }

        private static int SeedGalleryTargetCount(PropertyListing listing)
        {
            return listing.PublicationStatus == PropertyPublicationStatuses.Incomplete ||
                listing.PublicationStatus == PropertyPublicationStatuses.NeedsUpdate
                    ? 4
                    : 6;
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
                5 => "Dettagli e finiture",
                6 => "Contesto esterno",
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


        private sealed class SeedReferences
        {
            public DateTime SeedDate { get; } = new DateTime(2026, 5, 26, 12, 0, 0, DateTimeKind.Utc);

            public Guid Martina { get; } = Guid.Parse("10000000-0000-0000-0000-000000000002");
            public Guid Chiara { get; } = Guid.Parse("10000000-0000-0000-0000-000000000003");
            public Guid Lorenzo { get; } = Guid.Parse("10000000-0000-0000-0000-000000000004");
            public Guid Marco { get; } = Guid.Parse("10000000-0000-0000-0000-000000000005");

            public Guid PropertyListingId { get; } = Guid.Parse("20000000-0000-0000-0000-000000000301");

            public Guid LeadId { get; } = Guid.Parse("30000000-0000-0000-0000-000000000001");
            public Guid ClientBuyLeadId { get; } = Guid.Parse("30000000-0000-0000-0000-000000000101");
            public Guid ClientValuationLeadId { get; } = Guid.Parse("30000000-0000-0000-0000-000000000102");
            public Guid PaoloLeadId { get; } = Guid.Parse("30000000-0000-0000-0000-000000000201");
            public Guid NiccoloLeadId { get; } = Guid.Parse("30000000-0000-0000-0000-000000000202");
            public Guid AnnaLeadId { get; } = Guid.Parse("30000000-0000-0000-0000-000000000203");
            public Guid MarcoLeadId { get; } = Guid.Parse("30000000-0000-0000-0000-000000000204");

            public Guid ClientUserId { get; } = Guid.Parse("40000000-0000-0000-0000-000000000001");
        }

        private sealed class SeedListingSpec
        {
            public SeedListingSpec(
                string idSuffix,
                string reference,
                string publicationStatus,
                string assignedUser,
                string title,
                string slug,
                string displayLocation,
                decimal price,
                int rooms,
                int bathrooms,
                int surfaceSquareMeters,
                string contract,
                string propertyType,
                string zone,
                string featureKeys,
                string imageUrl,
                double latitude,
                double longitude,
                string address,
                string energyClass,
                int sortOrder,
                int? featuredSortOrder)
            {
                IdSuffix = idSuffix;
                Reference = reference;
                PublicationStatus = publicationStatus;
                AssignedUser = assignedUser;
                Title = title;
                Slug = slug;
                DisplayLocation = displayLocation;
                Price = price;
                Rooms = rooms;
                Bathrooms = bathrooms;
                SurfaceSquareMeters = surfaceSquareMeters;
                Contract = contract;
                PropertyType = propertyType;
                Zone = zone;
                FeatureKeys = featureKeys;
                ImageUrl = imageUrl;
                Latitude = latitude;
                Longitude = longitude;
                Address = address;
                EnergyClass = energyClass;
                SortOrder = sortOrder;
                FeaturedSortOrder = featuredSortOrder;
            }

            public string IdSuffix { get; }
            public string Reference { get; }
            public string PublicationStatus { get; }
            public string AssignedUser { get; }
            public string Title { get; }
            public string Slug { get; }
            public string DisplayLocation { get; }
            public decimal Price { get; }
            public int Rooms { get; }
            public int Bathrooms { get; }
            public int SurfaceSquareMeters { get; }
            public string Contract { get; }
            public string PropertyType { get; }
            public string Zone { get; }
            public string FeatureKeys { get; }
            public string ImageUrl { get; }
            public double Latitude { get; }
            public double Longitude { get; }
            public string Address { get; }
            public string EnergyClass { get; }
            public int SortOrder { get; }
            public int? FeaturedSortOrder { get; }
        }
    }
}
