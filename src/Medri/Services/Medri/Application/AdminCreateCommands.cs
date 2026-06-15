using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Medri.Services.Medri.Application
{
    public sealed class AdminCreateLeadCommand
    {
        private readonly MedriDbContext dbContext;

        public AdminCreateLeadCommand(MedriDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<AdminCreateCommandResult> ExecuteAsync(
            AdminLeadDetailUpdateDto input,
            CancellationToken cancellationToken = default)
        {
            input ??= new AdminLeadDetailUpdateDto();

            var now = DateTime.UtcNow;
            var reference = await AdminReferenceGenerator.NextLeadReferenceAsync(dbContext, cancellationToken);
            var leadId = Guid.NewGuid();

            var lead = new Lead
            {
                Id = leadId,
                InternalReference = reference,
                WorkflowStatus = LeadWorkflowStatuses.New,
                NextAction = ApplicationText.Clean(input.ContactReason) ?? "Completare dati minimi",
                AssignedAgencyUserId = input.AssignedAgencyUserId,
                FullName = ApplicationText.FullName(input.FirstName, input.LastName, "Nuovo lead"),
                Phone = ApplicationText.Clean(input.Phone),
                Email = ApplicationText.Clean(input.Email),
                SourceChannel = ApplicationText.Clean(input.SourceChannel) ?? "Ufficio",
                RequestType = ApplicationText.Clean(input.RequestType) ?? RequestTypes.Buy,
                Notes = ApplicationText.Clean(input.Notes),
                CreatedAtUtc = now,
                UpdatedAtUtc = now
            };

            var preference = new LeadPreference
            {
                Id = Guid.NewGuid(),
                LeadId = leadId,
                Timing = ApplicationText.Clean(input.Urgency),
                DesiredLocation = ApplicationText.Clean(input.DesiredLocation),
                ExpectedPriceOrMainQuestion = ApplicationText.Clean(input.ExpectedPriceOrMainQuestion),
                LinkedPropertyReference = ApplicationText.Clean(input.LinkedPropertyReference),
                ValuationGoal = ApplicationText.Clean(input.ContactReason),
                PreferencesAndCompromises = ApplicationText.Clean(input.NextContactQuestions)
            };

            lead.QualificationPercent = LeadQualificationCalculator.Calculate(lead, preference);

            dbContext.Leads.Add(lead);
            dbContext.LeadPreferences.Add(preference);

            await dbContext.SaveChangesAsync(cancellationToken);
            return AdminCreateCommandResult.Success(reference);
        }

    }

    public sealed class AdminCreateRequestCommand
    {
        private readonly MedriDbContext dbContext;

        public AdminCreateRequestCommand(MedriDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<AdminCreateCommandResult> ExecuteAsync(
            AdminRequestDetailUpdateDto input,
            CancellationToken cancellationToken = default)
        {
            input ??= new AdminRequestDetailUpdateDto();

            var now = DateTime.UtcNow;
            var reference = await AdminReferenceGenerator.NextRequestReferenceAsync(dbContext, cancellationToken);
            var leadId = Guid.NewGuid();

            dbContext.Leads.Add(new Lead
            {
                Id = leadId,
                WorkflowStatus = LeadWorkflowStatuses.Qualified,
                QualificationPercent = 35,
                NextAction = "Completare richiesta",
                AssignedAgencyUserId = input.AssignedAgencyUserId,
                FullName = ApplicationText.FullName(input.FirstName, input.LastName, "Nuova richiesta"),
                Phone = ApplicationText.Clean(input.Phone),
                Email = ApplicationText.Clean(input.Email),
                SourceChannel = ApplicationText.Clean(input.SourceChannel) ?? "Ufficio",
                RequestType = ApplicationText.Clean(input.RequestType) ?? RequestTypes.Buy,
                Notes = ApplicationText.Clean(input.SummaryNotes),
                CreatedAtUtc = now,
                UpdatedAtUtc = now
            });

            dbContext.LeadPreferences.Add(new LeadPreference
            {
                Id = Guid.NewGuid(),
                LeadId = leadId,
                SustainableBudgetLabel = ApplicationText.Clean(input.MaximumBudgetLabel),
                ExpectedPriceOrMainQuestion = ApplicationText.Clean(input.PreferredBudgetLabel),
                DesiredLocation = ApplicationText.Clean(input.DesiredLocation),
                AcceptableLocations = ApplicationText.Clean(input.AcceptableLocations),
                MinimumRooms = input.MinimumRooms,
                Appurtenances = ApplicationText.Clean(input.AccessibilityConstraint),
                DesiredMoveIn = ApplicationText.Clean(input.TimeFrame),
                FinancingStatus = ApplicationText.Clean(input.FinancingStatus),
                PropertyToSellStatus = ApplicationText.Clean(input.PropertyToSellStatus),
                SearchStage = ApplicationText.Clean(input.SummaryNotes),
                HouseholdDescription = ApplicationText.Clean(input.NeedsAfterContact),
                PreferencesAndCompromises = ApplicationText.NormalizeTags(input.DesiredPreferenceTagsText),
                PropertyCondition = ApplicationText.NormalizeTags(input.NegotiablePreferenceTagsText)
            });

            dbContext.SearchProfiles.Add(new SearchProfile
            {
                Id = Guid.NewGuid(),
                LeadId = leadId,
                PublicReference = reference,
                Status = RequestStatus(input.Status),
                CriteriaSummary = CriteriaSummary(input),
                ContactEmail = ApplicationText.Clean(input.Email),
                CreatedAtUtc = now,
                UpdatedAtUtc = now
            });

            await dbContext.SaveChangesAsync(cancellationToken);
            return AdminCreateCommandResult.Success(reference);
        }

        private static string CriteriaSummary(AdminRequestDetailUpdateDto input)
        {
            return string.Join(
                " - ",
                new[]
                {
                    ApplicationText.Clean(input.RequestType) ?? "Richiesta",
                    ApplicationText.Clean(input.DesiredLocation) ?? "zona da completare",
                    ApplicationText.Clean(input.MaximumBudgetLabel) ?? "budget da completare"
                });
        }

        private static string RequestStatus(string value)
        {
            var status = ApplicationText.Clean(value);
            return status == RequestStatuses.Updating ||
                status == RequestStatuses.InMatching ||
                status == RequestStatuses.Archived
                    ? status
                    : RequestStatuses.New;
        }
    }

    public sealed class AdminCreatePropertyCommand
    {
        private readonly MedriDbContext dbContext;

        public AdminCreatePropertyCommand(MedriDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<AdminCreateCommandResult> ExecuteAsync(
            AdminPropertyDetailUpdateDto input,
            CancellationToken cancellationToken = default)
        {
            input ??= new AdminPropertyDetailUpdateDto();

            var now = DateTime.UtcNow;
            var reference = await AdminReferenceGenerator.NextPropertyReferenceAsync(dbContext, cancellationToken);
            var currentMaxSortOrder = await dbContext.PropertyListings
                .IgnoreQueryFilters()
                .AsNoTracking()
                .Where(listing => listing.InternalReference != null)
                .Select(listing => (int?)listing.SortOrder)
                .MaxAsync(cancellationToken);
            var sortOrder = currentMaxSortOrder.GetValueOrDefault() + 1;

            var listing = new PropertyListing
            {
                Id = Guid.NewGuid(),
                InternalReference = reference,
                PublicationStatus = PropertyPublicationStatuses.Incomplete,
                Title = ApplicationText.Clean(input.Title) ?? "Nuovo immobile",
                Slug = $"admin-{reference.ToLowerInvariant()}",
                Location = ApplicationText.Clean(input.DisplayLocation) ?? "Da completare",
                DisplayLocation = ApplicationText.Clean(input.DisplayLocation) ?? "Da completare",
                Price = input.Price,
                Rooms = input.Rooms,
                Bathrooms = input.Bathrooms,
                SurfaceSquareMeters = input.SurfaceSquareMeters,
                Status = ApplicationText.Clean(input.Contract) ?? "Vendita",
                Contract = ApplicationText.Clean(input.Contract) ?? "Vendita",
                PropertyType = ApplicationText.Clean(input.ListingCategory) ?? "Abitazione",
                ListingCategory = ApplicationText.Clean(input.ListingCategory) ?? "Abitazione",
                Zone = ApplicationText.Clean(input.DisplayLocation) ?? "Da completare",
                FeatureKeys = "|admin-draft|",
                ImageUrl = string.Empty,
                SortOrder = sortOrder,
                UpdatedAtUtc = now,
                AssignedAgencyUserId = input.AssignedAgencyUserId,
                Address = ApplicationText.Clean(input.Address),
                BedroomsLabel = ApplicationText.Clean(input.BedroomsLabel),
                FloorLabel = ApplicationText.Clean(input.FloorLabel),
                ElevatorLabel = ApplicationText.Clean(input.ElevatorLabel),
                GarageLabel = ApplicationText.Clean(input.GarageLabel),
                OutdoorSpaceLabel = ApplicationText.Clean(input.OutdoorSpaceLabel),
                EnergyClass = ApplicationText.Clean(input.EnergyClass),
                RequiredWorksLabel = ApplicationText.Clean(input.RequiredWorksLabel),
                CondoFeesLabel = ApplicationText.Clean(input.CondoFeesLabel),
                SummaryTitle = ApplicationText.Clean(input.SummaryTitle),
                SummaryParagraph1 = ApplicationText.Clean(input.Description),
                SummaryParagraph2 = ApplicationText.Clean(input.SummaryParagraph2),
                ReadinessNote = ApplicationText.Clean(input.ReadinessNote),
                CostsNote = ApplicationText.Clean(input.CostsNote),
                ContextNote = ApplicationText.Clean(input.ContextNote),
                DecisionMarginNote = ApplicationText.Clean(input.DecisionMarginNote),
                AvailabilityLabel = ApplicationText.Clean(input.AvailabilityLabel),
                HeatingLabel = ApplicationText.Clean(input.HeatingLabel),
                ConstructionYearLabel = ApplicationText.Clean(input.ConstructionYearLabel),
                BalconyLabel = ApplicationText.Clean(input.BalconyLabel),
                CellarLabel = ApplicationText.Clean(input.CellarLabel),
                NearbyServicesLabel = ApplicationText.Clean(input.NearbyServicesLabel),
                HumanFitNote = ApplicationText.Clean(input.HumanFitNote)
            };

            var completion = AdminPropertyCompletionCalculator.Calculate(
                listing,
                0,
                false);
            AdminPropertyCompletionCalculator.ApplyToListing(listing, completion);
            if (completion.IsComplete)
            {
                listing.PublicationStatus = PropertyPublicationStatuses.Ready;
            }

            dbContext.PropertyListings.Add(listing);

            await dbContext.SaveChangesAsync(cancellationToken);
            return AdminCreateCommandResult.Success(reference);
        }

    }

    public sealed class AdminCreateCommandResult
    {
        public bool Succeeded { get; set; }

        public string Reference { get; set; }

        public static AdminCreateCommandResult Success(string reference)
        {
            return new AdminCreateCommandResult
            {
                Succeeded = true,
                Reference = reference
            };
        }
    }
}
