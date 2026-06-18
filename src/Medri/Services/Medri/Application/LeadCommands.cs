using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Medri.Services.Medri.Application
{
    public class SubmitLeadRequestCommand
    {
        private readonly MedriDbContext dbContext;

        public SubmitLeadRequestCommand(MedriDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<Guid> ExecuteAsync(
            LeadRequestDto input,
            CancellationToken cancellationToken = default)
        {
            var now = DateTime.UtcNow;
            var leadId = Guid.NewGuid();
            var publicReference = await CreatePublicReferenceAsync(cancellationToken);
            dbContext.Leads.Add(new Lead
            {
                Id = leadId,
                ClientUserId = input.ClientUserId,
                PublicReference = publicReference,
                FullName = input.FullName,
                Phone = input.Phone,
                Email = input.Email,
                SourceChannel = "Public lead intake",
                RequestType = input.RequestType,
                PreferredContactMode = input.PreferredContactMode,
                PreferredTimeSlot = input.PreferredTimeSlot,
                PreferredDay = input.PreferredDay,
                PreferredTime = input.PreferredTime,
                CreatedAtUtc = now
            });

            dbContext.LeadPreferences.Add(new LeadPreference
            {
                Id = Guid.NewGuid(),
                LeadId = leadId,
                SustainableBudgetLabel = input.SustainableBudget,
                DesiredLocation = input.DesiredLocation,
                AcceptableLocations = input.AcceptableLocations,
                PropertyType = input.PropertyType,
                SearchStage = input.SearchStage,
                FinancingStatus = input.FinancingStatus,
                PropertyToSellStatus = input.PropertyToSellStatus,
                Timing = input.Timing,
                PreferencesAndCompromises = input.PreferencesAndCompromises,
                HouseholdDescription = input.HouseholdDescription,
                WorkStudySituation = input.WorkStudySituation,
                AvailableGuarantees = input.AvailableGuarantees,
                DesiredMoveIn = input.DesiredMoveIn,
                PropertyCondition = input.PropertyCondition,
                Availability = input.Availability,
                ExpectedPriceOrMainQuestion = input.ExpectedPriceOrMainQuestion,
                DesiredContractType = input.DesiredContractType,
                IndicativeSurface = input.IndicativeSurface,
                Appurtenances = input.Appurtenances,
                ValuationGoal = input.ValuationGoal
            });

            dbContext.Appointments.Add(new Appointment
            {
                Id = Guid.NewGuid(),
                LeadId = leadId,
                Status = AppointmentStatuses.Submitted,
                RequestType = RequestTypes.GeneralContact,
                PreferredContactMode = input.PreferredContactMode,
                PreferredTimeSlot = input.PreferredTimeSlot,
                PreferredDay = input.PreferredDay,
                PreferredTime = input.PreferredTime,
                CreatedAtUtc = now
            });

            await dbContext.SaveChangesAsync(cancellationToken);
            return leadId;
        }

        private async Task<string> CreatePublicReferenceAsync(CancellationToken cancellationToken)
        {
            var requestCount = await dbContext.Leads.CountAsync(cancellationToken);
            return $"RQ-{2100 + requestCount}";
        }
    }

    public class CreatePropertyContactRequestCommand
    {
        private readonly MedriDbContext dbContext;

        public CreatePropertyContactRequestCommand(MedriDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<Guid?> ExecuteAsync(
            string slug,
            PropertyContactRequestDto input,
            Guid? appointmentId,
            CancellationToken cancellationToken = default)
        {
            var propertyId = await dbContext.PropertyListings
                .AsNoTracking()
                .Where(property => property.Slug == slug)
                .Select(property => (Guid?)property.Id)
                .FirstOrDefaultAsync(cancellationToken);

            if (!propertyId.HasValue)
            {
                return null;
            }

            if (appointmentId.HasValue)
            {
                var existingAppointment = await dbContext.Appointments
                    .Where(item =>
                        item.Id == appointmentId.Value &&
                        item.PropertyListingId == propertyId.Value &&
                        item.Status == AppointmentStatuses.Draft)
                    .FirstOrDefaultAsync(cancellationToken);
                if (existingAppointment == null)
                {
                    return null;
                }

                var existingLead = await dbContext.Leads
                    .Where(lead => lead.Id == existingAppointment.LeadId)
                    .FirstOrDefaultAsync(cancellationToken);
                if (existingLead == null)
                {
                    return null;
                }

                existingLead.FullName = input.FullName;
                existingLead.ClientUserId ??= input.ClientUserId;
                existingLead.Phone = input.Phone;
                existingLead.Email = input.Email;
                existingLead.PreferredContactMode = input.PreferredContactMode;
                existingLead.PreferredTimeSlot = input.PreferredTimeSlot;
                existingLead.PreferredDay = input.PreferredDay;
                existingLead.PreferredTime = input.PreferredTime;
                existingLead.Notes = input.Message;
                existingAppointment.PreferredContactMode = input.PreferredContactMode;
                existingAppointment.PreferredTimeSlot = input.PreferredTimeSlot;
                existingAppointment.PreferredDay = input.PreferredDay;
                existingAppointment.PreferredTime = input.PreferredTime;
                existingAppointment.Message = input.Message;

                await dbContext.SaveChangesAsync(cancellationToken);
                return existingAppointment.Id;
            }

            var now = DateTime.UtcNow;
            var leadId = Guid.NewGuid();
            var newAppointmentId = Guid.NewGuid();
            var requestCount = await dbContext.Leads.CountAsync(cancellationToken);
            dbContext.Leads.Add(new Lead
            {
                Id = leadId,
                ClientUserId = input.ClientUserId,
                PublicReference = $"RQ-{2100 + requestCount}",
                FullName = input.FullName,
                Phone = input.Phone,
                Email = input.Email,
                SourceChannel = "Property detail",
                RequestType = RequestTypes.PropertyContact,
                PreferredContactMode = input.PreferredContactMode,
                PreferredTimeSlot = input.PreferredTimeSlot,
                PreferredDay = input.PreferredDay,
                PreferredTime = input.PreferredTime,
                Notes = input.Message,
                CreatedAtUtc = now
            });
            dbContext.Appointments.Add(new Appointment
            {
                Id = newAppointmentId,
                LeadId = leadId,
                PropertyListingId = propertyId,
                Status = AppointmentStatuses.Draft,
                RequestType = RequestTypes.PropertyContact,
                PreferredContactMode = input.PreferredContactMode,
                PreferredTimeSlot = input.PreferredTimeSlot,
                PreferredDay = input.PreferredDay,
                PreferredTime = input.PreferredTime,
                Message = input.Message,
                CreatedAtUtc = now
            });

            await dbContext.SaveChangesAsync(cancellationToken);
            return newAppointmentId;
        }
    }

    public class SubmitPropertyContactRequestCommand
    {
        private readonly MedriDbContext dbContext;

        public SubmitPropertyContactRequestCommand(MedriDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<Guid?> ExecuteAsync(
            string slug,
            Guid appointmentId,
            Guid? clientUserId,
            CancellationToken cancellationToken = default)
        {
            var appointment = await dbContext.Appointments
                .Where(item =>
                    item.Id == appointmentId &&
                    item.PropertyListingId.HasValue)
                .Join(
                    dbContext.PropertyListings.Where(property => property.Slug == slug),
                    item => item.PropertyListingId.Value,
                    property => property.Id,
                    (item, property) => item)
                .FirstOrDefaultAsync(cancellationToken);

            if (appointment == null)
            {
                return null;
            }

            var lead = await dbContext.Leads
                .Where(item => item.Id == appointment.LeadId)
                .FirstOrDefaultAsync(cancellationToken);
            if (lead == null)
            {
                return null;
            }

            if (clientUserId.HasValue && !lead.ClientUserId.HasValue)
            {
                lead.ClientUserId = clientUserId.Value;
            }

            if (appointment.Status == AppointmentStatuses.Submitted)
            {
                await dbContext.SaveChangesAsync(cancellationToken);
                return appointment.LeadId;
            }

            if (appointment.Status != AppointmentStatuses.Draft)
            {
                return null;
            }

            appointment.Status = AppointmentStatuses.Submitted;
            dbContext.Interactions.Add(new Interaction
            {
                Id = Guid.NewGuid(),
                LeadId = appointment.LeadId,
                Channel = "Admin queue",
                Notes = "Nuova richiesta informazioni immobile pronta per la presa in carico.",
                OccurredAtUtc = DateTime.UtcNow
            });
            await dbContext.SaveChangesAsync(cancellationToken);
            return appointment.LeadId;
        }
    }
}
