using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Medri.Services;
using Medri.Services.Medri;
using Medri.Services.Medri.Application;
using Medri.Services.Medri.Identity;

namespace Medri.Infrastructure
{
    // Deterministic in-memory demo seed. The public entry points are InitializeUsers and
    // InitializeMedriDemoData; the latter delegates to one Seed* method per domain area so the
    // overall shape is readable at a glance. All cross-entity ids live in SeedReferences.
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

            var refs = new SeedReferences();

            SeedAgencyUsers(context, refs);
            SeedListings(context, refs);
            SeedFavorites(context, refs);
            SeedNotificationPreferences(context, refs);
            SeedLeads(context, refs);
            SeedSearchProfiles(context, refs);
            SeedLeadPreferences(context, refs);
            SeedInteractions(context, refs);
            SeedAppointments(context, refs);

            context.SaveChanges();
        }

        // --- Agency users -------------------------------------------------------------------

        private static void SeedAgencyUsers(MedriDbContext context, SeedReferences refs)
        {
            context.AgencyUsers.AddRange(
                new AgencyUser
                {
                    Id = refs.AgencyUserId,
                    DisplayName = "Segreteria Medri",
                    Email = "segreteria.medri@example.test",
                    Role = AgencyUserRoles.Advisor,
                    IsSystemSeed = true
                },
                new AgencyUser
                {
                    Id = refs.Martina,
                    DisplayName = "Martina Ricci",
                    Email = "martina.ricci@example.test",
                    Role = AgencyUserRoles.Advisor
                },
                new AgencyUser
                {
                    Id = refs.Chiara,
                    DisplayName = "Chiara Medri",
                    Email = "chiara.medri@email.it",
                    Role = AgencyUserRoles.Manager
                },
                new AgencyUser
                {
                    Id = refs.Lorenzo,
                    DisplayName = "Lorenzo Bassi",
                    Email = "lorenzo.bassi@example.test",
                    Role = AgencyUserRoles.Operator
                },
                new AgencyUser
                {
                    Id = refs.Marco,
                    DisplayName = "Marco Guidi",
                    Email = "marco.guidi@example.test",
                    Role = AgencyUserRoles.Operator
                });
        }

        // --- Listings (operational drafts + generated published/reserved) + media ------------

        private static void SeedListings(MedriDbContext context, SeedReferences refs)
        {
            var operationalListings = BuildOperationalListings(refs);
            var publishedAdminListings = BuildPublishedAdminListings(refs);
            var reservedAdminListings = BuildReservedAdminListings(refs);

            BackfillGeneratedListingDetails(
                operationalListings.Concat(reservedAdminListings).ToArray());

            CompleteSeedListingDetails(
                operationalListings
                    .Concat(publishedAdminListings)
                    .Concat(reservedAdminListings)
                    .ToArray());

            context.PropertyListings.AddRange(operationalListings);
            context.PropertyListings.AddRange(publishedAdminListings);
            context.PropertyListings.AddRange(reservedAdminListings);

            // Rich admin example (IM-1042) gets a full named gallery.
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
                operationalListings
                    .Concat(publishedAdminListings)
                    .Concat(reservedAdminListings)
                    .ToArray());
        }

        // Common defaults for the hand-curated operational drafts; only the distinctive fields are passed.
        private static PropertyListing OperationalListing(
            string idSuffix,
            string reference,
            string publicationStatus,
            int completionPercent,
            string missingItems,
            Guid assignedAgencyUserId,
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
            string imageUrl,
            int sortOrder)
        {
            return new PropertyListing
            {
                Id = Guid.Parse($"20000000-0000-0000-0000-{idSuffix}"),
                InternalReference = reference,
                PublicationStatus = publicationStatus,
                CompletionPercent = completionPercent,
                MissingItems = missingItems,
                AssignedAgencyUserId = assignedAgencyUserId,
                Title = title,
                Slug = slug,
                Location = "Cesena",
                DisplayLocation = displayLocation,
                Price = price,
                Rooms = rooms,
                Bathrooms = bathrooms,
                SurfaceSquareMeters = surfaceSquareMeters,
                Status = contract,
                Contract = contract,
                PropertyType = propertyType,
                Zone = zone,
                FeatureKeys = "|draft|",
                ImageUrl = imageUrl,
                SortOrder = sortOrder
            };
        }

        private static PropertyListing[] BuildOperationalListings(SeedReferences refs)
        {
            return new[]
            {
                OperationalListing("000000000101", "IM-342", "Incomplete", 78, "Foto esterne, nota impianti", refs.Chiara, "Trilocale ristrutturato", "bozza-im-342", "Cesena - Centro", 235000m, 3, 1, 88, "Vendita", "Trilocale", "Centro", "/medri-reference/assets/properties/property-06.jpg", 201),
                OperationalListing("000000000102", "IM-329", "NeedsUpdate", 76, "Prezzo, disponibilita", refs.Martina, "Appartamento con terrazzo", "bozza-im-329", "Cesena - Fiorenzuola", 260000m, 4, 2, 105, "Vendita", "Appartamento", "Fiorenzuola", "/medri-reference/assets/properties/property-05.jpg", 202),
                OperationalListing("000000000103", "IM-318", "Ready", 100, "Nessun dato mancante", refs.Lorenzo, "Villetta a schiera", "bozza-im-318", "Cesena - S. Egidio", 285000m, 4, 2, 120, "Vendita", "Villetta", "Sant'Egidio", "/medri-reference/assets/properties/property-03.jpg", 203),

                // Rich admin example kept explicit (drives the detail/preview demo).
                new PropertyListing
                {
                    Id = Guid.Parse("20000000-0000-0000-0000-000000001042"),
                    InternalReference = "IM-1042",
                    PublicationStatus = "Incomplete",
                    CompletionPercent = 82,
                    MissingItems = "Planimetria, nota lavori",
                    AssignedAgencyUserId = refs.Chiara,
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

                OperationalListing("000000001039", "IM-1039", "Incomplete", 61, "Foto zona giorno, APE, spese", refs.Martina, "Appartamento con giardino", "admin-im-1039", "Cesena - Centro", 210000m, 3, 1, 95, "Vendita", "Appartamento", "Centro", "/medri-reference/assets/properties/property-01.jpg", 11),
                OperationalListing("000000001046", "IM-1046", "Ready", 100, "Nessun dato mancante", refs.Lorenzo, "Nuovo appartamento in classe A", "admin-im-1046", "Cesena - Diegaro", 310000m, 4, 2, 110, "Vendita", "Appartamento", "Diegaro", "/medri-reference/assets/properties/property-08.jpg", 12),
                OperationalListing("000000000208", "AF-208", "NeedsUpdate", 74, "Canone, disponibilita, regole", refs.Marco, "Trilocale arredato", "admin-af-208", "Cesena - Oltresavio", 750m, 3, 1, 75, "Affitto", "Trilocale", "Oltresavio", "/medri-reference/assets/properties/property-07.jpg", 13),
                OperationalListing("000000001050", "IM-1050", "Ready", 100, "Nessun dato mancante", refs.Chiara, "Bilocale in centro", "admin-im-1050", "Cesena - Centro", 165000m, 2, 1, 62, "Vendita", "Bilocale", "Centro", "/medri-reference/assets/properties/property-02.jpg", 30),
                OperationalListing("000000001051", "IM-1051", "Ready", 100, "Nessun dato mancante", refs.Martina, "Casa indipendente", "admin-im-1051", "Cesena - Case Finali", 390000m, 5, 2, 160, "Vendita", "Casa indipendente", "Case Finali", "/medri-reference/assets/properties/property-06.jpg", 31),
                OperationalListing("000000001052", "IM-1052", "Ready", 100, "Nessun dato mancante", refs.Lorenzo, "Loft zona stazione", "admin-im-1052", "Cesena - Stazione", 198000m, 2, 1, 70, "Vendita", "Loft", "Stazione", "/medri-reference/assets/properties/property-04.jpg", 32),
                OperationalListing("000000001053", "IM-1053", "Ready", 100, "Nessun dato mancante", refs.Marco, "Porzione con corte", "admin-im-1053", "Cesena - San Mauro", 248000m, 4, 2, 116, "Vendita", "Porzione", "San Mauro", "/medri-reference/assets/properties/property-05.jpg", 33),
                OperationalListing("000000001054", "AF-209", "Ready", 100, "Nessun dato mancante", refs.Martina, "Monolocale arredato", "admin-af-209", "Cesena - Centro", 580m, 1, 1, 42, "Affitto", "Monolocale", "Centro", "/medri-reference/assets/properties/property-07.jpg", 34),
                OperationalListing("000000001055", "IM-1055", "NeedsUpdate", 72, "Prezzo, foto bagno", refs.Chiara, "Quadrilocale luminoso", "admin-im-1055", "Cesena - Ponte Abbadesse", 255000m, 4, 2, 108, "Vendita", "Quadrilocale", "Ponte Abbadesse", "/medri-reference/assets/properties/property-02.jpg", 40),
                OperationalListing("000000001056", "AF-210", "NeedsUpdate", 68, "Disponibilita, regole animali", refs.Marco, "Bilocale arredato", "admin-af-210", "Cesena - Fiorita", 690m, 2, 1, 58, "Affitto", "Bilocale", "Fiorita", "/medri-reference/assets/properties/property-01.jpg", 41)
            };
        }

        // Blueprints reused by the generated published/reserved listings.
        private static GeneratedListingBlueprint[] GeneratedListingBlueprints()
        {
            return new[]
            {
                new GeneratedListingBlueprint("Trilocale con balcone", "Bilocale arredato in centro", "Cesena - Centro", "Centro", "Trilocale", "|terrace|move-in-ready|near-services|near-public-transport|", "/medri-reference/assets/properties/property-01.jpg", 44.1396d, 12.2431d, "Via Mura Barriera Ponente 12, Cesena", 3, 1, 86, "C"),
                new GeneratedListingBlueprint("Appartamento con terrazzo", "Trilocale arredato Oltresavio", "Cesena - Oltresavio", "Oltresavio", "Appartamento", "|terrace|parking|move-in-ready|near-services|", "/medri-reference/assets/properties/property-07.jpg", 44.1285d, 12.2370d, "Via Savio 18, Cesena", 3, 1, 78, "E"),
                new GeneratedListingBlueprint("Villetta con giardino", "Porzione con corte privata", "Cesena - S. Egidio", "Sant'Egidio", "Villetta", "|garden|garage|parking|renovation-needed|near-services|", "/medri-reference/assets/properties/property-03.jpg", 44.1530d, 12.2860d, "Via S. Egidio 42, Cesena", 4, 2, 118, "D"),
                new GeneratedListingBlueprint("Quadrilocale in classe A", "Appartamento recente arredato", "Cesena - Diegaro", "Diegaro", "Appartamento", "|terrace|elevator|move-in-ready|high-energy-class|", "/medri-reference/assets/properties/property-08.jpg", 44.1005d, 12.2140d, "Via Diegaro 8, Cesena", 4, 2, 106, "A"),
                new GeneratedListingBlueprint("Casa indipendente con corte", "Casa arredata con giardino", "Cesena - Case Finali", "Case Finali", "Casa indipendente", "|garden|garage|parking|renovation-needed|", "/medri-reference/assets/properties/property-06.jpg", 44.1565d, 12.2275d, "Via Case Finali 21, Cesena", 5, 2, 158, "D"),
                new GeneratedListingBlueprint("Bilocale zona stazione", "Bilocale pronto zona stazione", "Cesena - Stazione", "Stazione", "Bilocale", "|terrace|move-in-ready|near-services|near-public-transport|", "/medri-reference/assets/properties/property-02.jpg", 44.1467d, 12.2467d, "Via Stazione 4, Cesena", 2, 1, 64, "E"),
                new GeneratedListingBlueprint("Porzione con terrazzo", "Trilocale con posto auto", "Cesena - Fiorenzuola", "Fiorenzuola", "Porzione", "|terrace|parking|move-in-ready|near-services|", "/medri-reference/assets/properties/property-05.jpg", 44.1334d, 12.2550d, "Via Fiorenzuola 16, Cesena", 4, 2, 104, "C"),
                new GeneratedListingBlueprint("Appartamento ultimo piano", "Mansarda arredata in centro", "Cesena - Centro storico", "Centro storico", "Appartamento", "|terrace|move-in-ready|high-energy-class|near-services|", "/medri-reference/assets/properties/property-04.jpg", 44.1391d, 12.2464d, "Via Centro Storico 9, Cesena", 3, 1, 82, "B")
            };
        }

        private static PropertyListing[] BuildPublishedAdminListings(SeedReferences refs)
        {
            var blueprints = GeneratedListingBlueprints();

            return Enumerable.Range(1, 42)
                .Select(index =>
                {
                    var blueprint = blueprints[(index - 1) % blueprints.Length];
                    var isRent = index % 5 == 0;
                    var contract = isRent ? "Affitto" : "Vendita";
                    var title = isRent ? blueprint.RentTitle : blueprint.SaleTitle;
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
                            ? refs.Marco
                            : index % 3 == 0
                                ? refs.Lorenzo
                                : index % 2 == 0
                                    ? refs.Martina
                                    : refs.Chiara,
                        Title = title,
                        Slug = $"admin-pubblicato-{index:00}",
                        Location = blueprint.DisplayLocation,
                        DisplayLocation = blueprint.DisplayLocation,
                        Price = price,
                        Rooms = isRent ? Math.Max(1, blueprint.Rooms - 1) : blueprint.Rooms,
                        Bathrooms = blueprint.Bathrooms,
                        SurfaceSquareMeters = isRent ? Math.Max(38, blueprint.Surface - 18) : blueprint.Surface + index % 6,
                        Status = contract,
                        Contract = contract,
                        PropertyType = blueprint.PropertyType,
                        Zone = blueprint.Zone,
                        FeatureKeys = blueprint.FeatureKeys,
                        ImageUrl = blueprint.ImageUrl,
                        Latitude = blueprint.Latitude + ((index % 3) - 1) * 0.0012d,
                        Longitude = blueprint.Longitude + ((index % 4) - 1.5d) * 0.0012d,
                        Address = blueprint.Address,
                        BedroomsLabel = isRent
                            ? Math.Max(1, blueprint.Rooms - 2).ToString()
                            : Math.Max(1, blueprint.Rooms - 1).ToString(),
                        FloorLabel = index % 3 == 0 ? "Piano alto" : "Piano intermedio",
                        GarageLabel = index % 2 == 0 ? "Posto auto" : "Garage",
                        OutdoorSpaceLabel = blueprint.FeatureKeys.Contains("|garden|") ? "Giardino" : "Balcone",
                        EnergyClass = blueprint.EnergyClass,
                        AvailabilityLabel = isRent ? "Libero da concordare" : "Disponibile a rogito",
                        HeatingLabel = "Autonomo",
                        RequiredWorksLabel = index % 4 == 0 ? "Aggiornamenti leggeri" : "Buono",
                        SummaryTitle = title,
                        SummaryParagraph1 = string.Empty,
                        SummaryParagraph2 = string.Empty,
                        HumanFitNote = string.Empty,
                        SortOrder = 300 + index,
                        UpdatedAtUtc = refs.SeedDate.AddDays(-index)
                    };
                })
                .ToArray();
        }

        private static PropertyListing[] BuildReservedAdminListings(SeedReferences refs)
        {
            var blueprints = GeneratedListingBlueprints();

            return Enumerable.Range(1, 5)
                .Select(index =>
                {
                    var blueprint = blueprints[(index + 2) % blueprints.Length];

                    return new PropertyListing
                    {
                        Id = Guid.Parse($"20000000-0000-0000-0000-{500 + index:000000000000}"),
                        InternalReference = $"IM-R{index:000}",
                        PublicationStatus = "Reserved",
                        CompletionPercent = 90,
                        MissingItems = "Verifica finale",
                        AssignedAgencyUserId = index % 2 == 0 ? refs.Martina : refs.Chiara,
                        Title = blueprint.SaleTitle,
                        Slug = $"admin-riservato-{index:00}",
                        Location = blueprint.DisplayLocation,
                        DisplayLocation = blueprint.DisplayLocation,
                        Price = 210000m + index * 2500m,
                        Rooms = blueprint.Rooms,
                        Bathrooms = blueprint.Bathrooms,
                        SurfaceSquareMeters = blueprint.Surface,
                        Status = "Vendita",
                        Contract = "Vendita",
                        PropertyType = blueprint.PropertyType,
                        Zone = blueprint.Zone,
                        FeatureKeys = blueprint.FeatureKeys,
                        ImageUrl = blueprint.ImageUrl,
                        Latitude = blueprint.Latitude,
                        Longitude = blueprint.Longitude,
                        Address = blueprint.Address,
                        EnergyClass = blueprint.EnergyClass,
                        SortOrder = 500 + index
                    };
                })
                .ToArray();
        }

        // Fills coordinates/address/energy class on the hand-curated listings from the nearest blueprint.
        private static void BackfillGeneratedListingDetails(PropertyListing[] listings)
        {
            var blueprints = GeneratedListingBlueprints();

            foreach (var item in listings.Select((listing, index) => new { listing, index }))
            {
                var blueprint = blueprints
                    .FirstOrDefault(candidate =>
                        item.listing.DisplayLocation?.IndexOf(candidate.Zone, StringComparison.OrdinalIgnoreCase) >= 0) ??
                    blueprints[item.index % blueprints.Length];

                if (item.listing.Latitude == 0d && item.listing.Longitude == 0d)
                {
                    item.listing.Latitude = blueprint.Latitude + ((item.index % 3) - 1) * 0.001d;
                    item.listing.Longitude = blueprint.Longitude + ((item.index % 4) - 1.5d) * 0.001d;
                }

                if (string.IsNullOrWhiteSpace(item.listing.Address))
                {
                    item.listing.Address = blueprint.Address;
                }

                if (string.IsNullOrWhiteSpace(item.listing.EnergyClass))
                {
                    item.listing.EnergyClass = blueprint.EnergyClass;
                }
            }
        }

        // --- Favorites ----------------------------------------------------------------------

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

        // --- Client notification preferences ------------------------------------------------

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

        // --- Leads (technical seed + client requests + admin board + request board) ---------

        private static void SeedLeads(MedriDbContext context, SeedReferences refs)
        {
            var seedDate = refs.SeedDate;

            context.Leads.Add(new Lead
            {
                Id = refs.LeadId,
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
                    Id = refs.ClientBuyLeadId,
                    ClientUserId = refs.ClientUserId,
                    PublicReference = "RQ-2047",
                    FullName = "Elena Gori",
                    Email = "elena.gori@email.it",
                    Phone = "347 9988776",
                    SourceChannel = "Lead convertito",
                    RequestType = "Buy",
                    WorkflowStatus = "Qualified",
                    AssignedAgencyUserId = refs.Martina,
                    Notes = "3 immobili proposti",
                    CreatedAtUtc = seedDate.AddDays(-2)
                },
                new Lead
                {
                    Id = refs.ClientValuationLeadId,
                    ClientUserId = refs.ClientUserId,
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
                    CreatedAtUtc = seedDate.AddDays(-1)
                },
                new Lead
                {
                    Id = refs.NiccoloLeadId,
                    InternalReference = "LD-1176",
                    WorkflowStatus = "InContact",
                    QualificationPercent = 64,
                    NextAction = "Chiedere budget massimo e garanzie",
                    AssignedAgencyUserId = refs.Lorenzo,
                    FullName = "Niccolo Fabbri",
                    Email = "niccolo.f@email.it",
                    SourceChannel = "Email",
                    RequestType = "Rent",
                    CreatedAtUtc = seedDate.AddDays(-2)
                },
                new Lead
                {
                    Id = refs.AnnaLeadId,
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
                    Id = refs.MarcoLeadId,
                    WorkflowStatus = "Qualified",
                    FullName = "Marco Guidi",
                    Email = "marco.guidi@example.test",
                    Phone = "333 2028000",
                    SourceChannel = "Agency",
                    RequestType = "Sell",
                    AssignedAgencyUserId = refs.Chiara,
                    Notes = "Vuole vendere appartamento",
                    CreatedAtUtc = seedDate.AddDays(-4),
                    UpdatedAtUtc = seedDate.AddDays(-4)
                });

            Lead AdminLead(
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
                AdminLead(205, "LD-1181", "InContact", "Elena Gori", null, "347 9988776", "WhatsApp", "Buy", 72, "Inviare recap con 3 immobili compatibili", refs.Martina, -96),
                AdminLead(206, "LD-1168", "New", "Sara Monti", "sara.monti@example.test", "333 1168000", "Telefono", "Buy", 42, "Capire budget sostenibile e zona prioritaria", null, -240),
                AdminLead(207, "LD-1167", "New", "Giulio Neri", "giulio.neri@example.test", "333 1167000", "Email", "Rent", 36, "Verificare garanzie e tempi di ingresso", null, -264),
                AdminLead(208, "LD-1166", "New", "Marta Serra", "marta.serra@example.test", "333 1166000", "WhatsApp", "Sell", 51, "Raccogliere indirizzo e stato immobile", refs.Chiara, -288),
                AdminLead(209, "LD-1165", "New", "Luca Bellini", "luca.bellini@example.test", "333 1165000", "Ufficio", "Valuation", 40, "Fissare primo confronto telefonico", null, -312),
                AdminLead(210, "LD-1164", "New", "Francesca Ricci", "francesca.ricci@example.test", "333 1164000", "Telefono", "Buy", 48, "Completare esigenze familiari", refs.Martina, -336),
                AdminLead(211, "LD-1163", "New", "Roberto Fini", "roberto.fini@example.test", "333 1163000", "Email", "RentOut", 44, "Chiarire disponibilita e contratto desiderato", refs.Chiara, -360),
                AdminLead(212, "LD-1162", "InContact", "Irene Vitali", "irene.vitali@example.test", "333 1162000", "WhatsApp", "Buy", 68, "Inviare immobili zona Centro", refs.Martina, -384),
                AdminLead(213, "LD-1161", "InContact", "Carlo Benini", "carlo.benini@example.test", "333 1161000", "Telefono", "Sell", 59, "Concordare sopralluogo", refs.Chiara, -408),
                AdminLead(214, "LD-1160", "InContact", "Laura Guidi", "laura.guidi@example.test", "333 1160000", "Email", "Rent", 63, "Richiedere documentazione garanzie", refs.Lorenzo, -432),
                AdminLead(215, "LD-1159", "InContact", "Davide Farina", "davide.farina@example.test", "333 1159000", "Ufficio", "Valuation", 70, "Preparare riepilogo valutazione", refs.Chiara, -456),
                AdminLead(216, "LD-1158", "InContact", "Silvia Moretti", "silvia.moretti@example.test", "333 1158000", "WhatsApp", "Buy", 61, "Aggiornare preferenze terrazzo", refs.Martina, -480),
                AdminLead(217, "LD-1157", "InContact", "Enrico Berti", "enrico.berti@example.test", "333 1157000", "Telefono", "RentOut", 58, "Definire canone atteso", refs.Lorenzo, -504),
                AdminLead(218, "LD-1156", "InContact", "Alessia Fontana", "alessia.fontana@example.test", "333 1156000", "Email", "Buy", 66, "Mandare recap compatibilita", refs.Martina, -528),
                AdminLead(219, "LD-1155", "InContact", "Giorgio Lombardi", "giorgio.lombardi@example.test", "333 1155000", "Telefono", "Sell", 55, "Confermare documenti catastali", refs.Chiara, -552),
                AdminLead(226, "LD-1148", "InContact", "Valeria Mancini", "valeria.mancini@example.test", "333 1148000", "WhatsApp", "Valuation", 57, "Richiedere indirizzo preciso per prima valutazione", refs.Chiara, -564),
                AdminLead(220, "LD-1154", "Archived", "Paola Santi", "paola.santi@example.test", "333 1154000", "Email", "Buy", 22, "Contatto non piu interessato", null, -576),
                AdminLead(221, "LD-1153", "Archived", "Andrea Russo", "andrea.russo@example.test", "333 1153000", "Telefono", "Rent", 18, "Budget non compatibile", null, -600),
                AdminLead(222, "LD-1152", "Archived", "Monica De Angelis", "monica.deangelis@example.test", "333 1152000", "WhatsApp", "Valuation", 25, "Valutazione rinviata", null, -624),
                AdminLead(223, "LD-1151", "Archived", "Stefano Riva", "stefano.riva@example.test", "333 1151000", "Ufficio", "Sell", 20, "Ha scelto altra agenzia", null, -648),
                AdminLead(224, "LD-1150", "Archived", "Claudia Gatti", "claudia.gatti@example.test", "333 1150000", "Email", "Buy", 15, "Richiesta non coltivabile", null, -672),
                AdminLead(225, "LD-1149", "Archived", "Filippo Marchetti", "filippo.marchetti@example.test", "333 1149000", "Telefono", "RentOut", 19, "Immobile non disponibile", null, -696));

            Lead RequestLead(
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
                RequestLead(301, "Sara Monti", "sara.monti@example.test", "333 2027000", "Buy", "Richiesta da primo colloquio", null, -5),
                RequestLead(302, "Giulio Neri", "giulio.neri@example.test", "333 2026000", "Rent", "Richiesta affitto da qualificare", null, -6),
                RequestLead(303, "Marta Serra", "marta.serra@example.test", "333 2025000", "Sell", "Profilo pronto per matching proprietario", refs.Martina, -7),
                RequestLead(304, "Luca Bellini", "luca.bellini@example.test", "333 2024000", "Valuation", "Richiesta valutazione assegnata", refs.Chiara, -8),
                RequestLead(305, "Francesca Ricci", "francesca.ricci@example.test", "333 2023000", "Buy", "Profilo acquisto assegnato", refs.Lorenzo, -9),
                RequestLead(306, "Roberto Fini", "roberto.fini@example.test", "333 2022000", "RentOut", "Ricerca in matching da affinare", null, -10),
                RequestLead(307, "Irene Vitali", "irene.vitali@example.test", "333 2021000", "Buy", "Richiesta in matching", null, -11),
                RequestLead(308, "Carlo Benini", "carlo.benini@example.test", "333 2020000", "Sell", "Profilo vendita in matching", null, -12),
                RequestLead(309, "Laura Guidi", "laura.guidi@example.test", "333 2019000", "Rent", "Aggiornare preferenze canone", null, -13),
                RequestLead(310, "Davide Farina", "davide.farina@example.test", "333 2018000", "Valuation", "Aggiornare obiettivo valutazione", null, -14));
        }

        // --- Search profiles ----------------------------------------------------------------

        private static void SeedSearchProfiles(MedriDbContext context, SeedReferences refs)
        {
            var seedDate = refs.SeedDate;

            SearchProfile RequestProfile(int idSuffix, int leadSuffix, string publicReference, string status, string criteria, string contactEmail, string source, int daysOffset)
            {
                return new SearchProfile
                {
                    Id = Guid.Parse($"31000000-0000-0000-0000-{idSuffix:000000000000}"),
                    LeadId = Guid.Parse($"30000000-0000-0000-0000-{leadSuffix:000000000000}"),
                    PublicReference = publicReference,
                    Status = status,
                    CriteriaSummary = criteria,
                    ContactEmail = contactEmail,
                    SourceQueryString = source,
                    CreatedAtUtc = seedDate.AddDays(daysOffset),
                    UpdatedAtUtc = seedDate.AddDays(daysOffset)
                };
            }

            context.SearchProfiles.AddRange(
                new SearchProfile
                {
                    Id = Guid.Parse("31000000-0000-0000-0000-000000000001"),
                    LeadId = refs.LeadId,
                    CriteriaSummary = "Profilo seed tecnico",
                    ContactEmail = "lead.seed@example.test",
                    SourceQueryString = "",
                    CreatedAtUtc = seedDate,
                    UpdatedAtUtc = seedDate
                },
                new SearchProfile
                {
                    Id = Guid.Parse("31000000-0000-0000-0000-000000000101"),
                    LeadId = refs.ClientBuyLeadId,
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
                    LeadId = refs.NiccoloLeadId,
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
                    LeadId = refs.AnnaLeadId,
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
                    LeadId = refs.MarcoLeadId,
                    PublicReference = "RQ-2028",
                    Status = "Updating",
                    CriteriaSummary = "Documenti|foto mancanti|prezzo da validare",
                    ContactEmail = "marco.guidi@example.test",
                    SourceQueryString = "",
                    CreatedAtUtc = seedDate.AddDays(-4),
                    UpdatedAtUtc = new DateTime(2026, 4, 26, 12, 15, 0, DateTimeKind.Utc)
                },
                RequestProfile(201, 301, "RQ-2027", "New", "Centro storico|budget da definire|prima visita", "sara.monti@example.test", "Richiesta da primo colloquio", -5),
                RequestProfile(202, 302, "RQ-2026", "New", "Affitto|garanzie da verificare|ingresso rapido", "giulio.neri@example.test", "Richiesta affitto da qualificare", -6),
                RequestProfile(203, 303, "RQ-2025", "InMatching", "Trilocale|zona Stadio|box auto", "marta.serra@example.test", "Profilo pronto per matching proprietario", -7),
                RequestProfile(204, 304, "RQ-2024", "InMatching", "Sopralluogo|zona Borello|documenti pronti", "luca.bellini@example.test", "Richiesta valutazione assegnata", -8),
                RequestProfile(205, 305, "RQ-2023", "InMatching", "Villetta|giardino|mutuo avviato", "francesca.ricci@example.test", "Profilo acquisto assegnato", -9),
                RequestProfile(206, 306, "RQ-2022", "InMatching", "Affitto gestito|contratto concordato|arredo parziale", "roberto.fini@example.test", "Ricerca in matching da affinare", -10),
                RequestProfile(207, 307, "RQ-2021", "InMatching", "Casa indipendente|zona Ponte Abbadesse|garage", "irene.vitali@example.test", "Richiesta in matching", -11),
                RequestProfile(208, 308, "RQ-2020", "InMatching", "Vendita appartamento|foto da produrre|prezzo coerente", "carlo.benini@example.test", "Profilo vendita in matching", -12),
                RequestProfile(209, 309, "RQ-2019", "Updating", "Bilocale|canone aggiornato|garanzie", "laura.guidi@example.test", "Aggiornare preferenze canone", -13),
                RequestProfile(210, 310, "RQ-2018", "Updating", "Valutazione|obiettivo cambiato|richiamare", "davide.farina@example.test", "Aggiornare obiettivo valutazione", -14));
        }

        // --- Lead preferences ---------------------------------------------------------------

        private static void SeedLeadPreferences(MedriDbContext context, SeedReferences refs)
        {
            context.LeadPreferences.Add(new LeadPreference
            {
                Id = Guid.Parse("32000000-0000-0000-0000-000000000001"),
                LeadId = refs.LeadId,
                MinimumPrice = 150000m,
                MaximumPrice = 300000m,
                DesiredLocation = "Area seed",
                MinimumRooms = 3
            });

            context.LeadPreferences.AddRange(
                new LeadPreference
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
                new LeadPreference
                {
                    Id = Guid.Parse("32000000-0000-0000-0000-000000000102"),
                    LeadId = refs.ClientValuationLeadId,
                    DesiredLocation = "Diegaro",
                    PropertyType = "Casa familiare",
                    ValuationGoal = "Prima valutazione"
                },
                new LeadPreference
                {
                    Id = Guid.Parse("32000000-0000-0000-0000-000000000201"),
                    LeadId = refs.PaoloLeadId,
                    Timing = "Da capire",
                    ValuationGoal = "Vuole capire se vendere entro l'anno",
                    PreferencesAndCompromises = "Zona precisa, metratura indicativa, stato dell'immobile, tempi desiderati, eventuali vincoli familiari."
                },
                new LeadPreference
                {
                    Id = Guid.Parse("32000000-0000-0000-0000-000000000202"),
                    LeadId = refs.NiccoloLeadId,
                    PropertyType = "Bilocale",
                    DesiredLocation = "vicino campus",
                    SustainableBudgetLabel = "max EUR 750",
                    Timing = "Media"
                },
                new LeadPreference
                {
                    Id = Guid.Parse("32000000-0000-0000-0000-000000000203"),
                    LeadId = refs.AnnaLeadId,
                    PropertyType = "Sopralluogo",
                    DesiredLocation = "Cesena - Diegaro",
                    ValuationGoal = "entro mese",
                    Timing = "Alta"
                },
                new LeadPreference
                {
                    Id = Guid.Parse("32000000-0000-0000-0000-000000000204"),
                    LeadId = refs.MarcoLeadId,
                    PropertyToSellStatus = "Documenti",
                    PreferencesAndCompromises = "foto mancanti",
                    ExpectedPriceOrMainQuestion = "prezzo da validare",
                    Timing = "Media"
                });
        }

        // --- Interactions -------------------------------------------------------------------

        private static void SeedInteractions(MedriDbContext context, SeedReferences refs)
        {
            var seedDate = refs.SeedDate;

            context.Interactions.Add(new Interaction
            {
                Id = Guid.Parse("33000000-0000-0000-0000-000000000001"),
                LeadId = refs.LeadId,
                Channel = "Seed",
                Notes = "Interazione seed tecnico",
                OccurredAtUtc = seedDate
            });

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
                },
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

        // --- Appointments -------------------------------------------------------------------

        private static void SeedAppointments(MedriDbContext context, SeedReferences refs)
        {
            var seedDate = refs.SeedDate;

            context.Appointments.Add(new Appointment
            {
                Id = Guid.Parse("35000000-0000-0000-0000-000000000001"),
                LeadId = refs.LeadId,
                PropertyListingId = refs.PropertyListingId,
                AgencyUserId = refs.AgencyUserId,
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

        // --- Listing detail completion + gallery helpers (unchanged behaviour) --------------

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

        // --- Shared seed identifiers (point 4: no scattered literal GUIDs) ------------------

        private sealed class SeedReferences
        {
            public DateTime SeedDate { get; } = new DateTime(2026, 5, 26, 12, 0, 0, DateTimeKind.Utc);

            public Guid AgencyUserId { get; } = Guid.Parse("10000000-0000-0000-0000-000000000001");
            public Guid Martina { get; } = Guid.Parse("10000000-0000-0000-0000-000000000002");
            public Guid Chiara { get; } = Guid.Parse("10000000-0000-0000-0000-000000000003");
            public Guid Lorenzo { get; } = Guid.Parse("10000000-0000-0000-0000-000000000004");
            public Guid Marco { get; } = Guid.Parse("10000000-0000-0000-0000-000000000005");

            // Default linked listing for the technical seed appointment/favorite (a published listing).
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

        private sealed class GeneratedListingBlueprint
        {
            public GeneratedListingBlueprint(
                string saleTitle,
                string rentTitle,
                string displayLocation,
                string zone,
                string propertyType,
                string featureKeys,
                string imageUrl,
                double latitude,
                double longitude,
                string address,
                int rooms,
                int bathrooms,
                int surface,
                string energyClass)
            {
                SaleTitle = saleTitle;
                RentTitle = rentTitle;
                DisplayLocation = displayLocation;
                Zone = zone;
                PropertyType = propertyType;
                FeatureKeys = featureKeys;
                ImageUrl = imageUrl;
                Latitude = latitude;
                Longitude = longitude;
                Address = address;
                Rooms = rooms;
                Bathrooms = bathrooms;
                Surface = surface;
                EnergyClass = energyClass;
            }

            public string SaleTitle { get; }
            public string RentTitle { get; }
            public string DisplayLocation { get; }
            public string Zone { get; }
            public string PropertyType { get; }
            public string FeatureKeys { get; }
            public string ImageUrl { get; }
            public double Latitude { get; }
            public double Longitude { get; }
            public string Address { get; }
            public int Rooms { get; }
            public int Bathrooms { get; }
            public int Surface { get; }
            public string EnergyClass { get; }
        }
    }
}
