using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Medri.Services.Medri.Application
{
    public class LeadConfirmationQuery
    {
        private readonly MedriDbContext dbContext;

        public LeadConfirmationQuery(MedriDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<LeadConfirmationDto> ExecuteAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            var result = await dbContext.Leads
                .AsNoTracking()
                .Where(lead => lead.Id == id)
                .Select(lead => new LeadConfirmationDto
                {
                    Id = lead.Id,
                    RequestType = lead.RequestType,
                    PreferredContactMode = lead.PreferredContactMode,
                    PreferredTimeSlot = lead.PreferredTimeSlot,
                    PreferredDay = lead.PreferredDay,
                    PreferredTime = lead.PreferredTime
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (result == null)
            {
                return null;
            }

            result.FeaturedProperties = await dbContext.PropertyListings
                .AsNoTracking()
                .OrderBy(property => property.SortOrder)
                .Take(3)
                .Select(property => new PropertySummaryDto
                {
                    Id = property.Id,
                    Slug = property.Slug,
                    Title = property.Title,
                    Status = property.Status,
                    Price = property.Price,
                    Contract = property.Contract,
                    DisplayLocation = property.DisplayLocation,
                    Location = property.Location,
                    ImageUrl = property.ImageUrl
                })
                .ToListAsync(cancellationToken);

            return result;
        }
    }

    public class PropertyContactQuery
    {
        private readonly MedriDbContext dbContext;

        public PropertyContactQuery(MedriDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public Task<VisitPropertySummaryDto> FindPropertyAsync(
            string slug,
            CancellationToken cancellationToken = default)
        {
            return dbContext.PropertyListings
                .AsNoTracking()
                .Where(property => property.Slug == slug)
                .Select(property => ProjectProperty(property))
                .FirstOrDefaultAsync(cancellationToken);
        }

        public Task<VisitReviewDto> FindReviewAsync(
            string slug,
            Guid appointmentId,
            CancellationToken cancellationToken = default)
        {
            return dbContext.Appointments
                .AsNoTracking()
                .Where(appointment =>
                    appointment.Id == appointmentId &&
                    appointment.Status == AppointmentStatuses.Draft &&
                    appointment.PropertyListingId.HasValue)
                .Join(
                    dbContext.Leads.AsNoTracking(),
                    appointment => appointment.LeadId,
                    lead => lead.Id,
                    (appointment, lead) => new { appointment, lead })
                .Join(
                    dbContext.PropertyListings.AsNoTracking().Where(property => property.Slug == slug),
                    item => item.appointment.PropertyListingId.Value,
                    property => property.Id,
                    (item, property) => new VisitReviewDto
                    {
                        AppointmentId = item.appointment.Id,
                        FullName = item.lead.FullName,
                        Phone = item.lead.Phone,
                        Email = item.lead.Email,
                        Message = item.appointment.Message,
                        PreferredContactMode = item.appointment.PreferredContactMode,
                        PreferredTimeSlot = item.appointment.PreferredTimeSlot,
                        PreferredDay = item.appointment.PreferredDay,
                        PreferredTime = item.appointment.PreferredTime,
                        Property = ProjectProperty(property)
                    })
                .FirstOrDefaultAsync(cancellationToken);
        }

        public Task<bool> IsSubmittedAsync(
            string slug,
            Guid appointmentId,
            CancellationToken cancellationToken = default)
        {
            return dbContext.Appointments
                .AsNoTracking()
                .Where(appointment =>
                    appointment.Id == appointmentId &&
                    appointment.Status == AppointmentStatuses.Submitted &&
                    appointment.PropertyListingId.HasValue)
                .Join(
                    dbContext.PropertyListings.AsNoTracking().Where(property => property.Slug == slug),
                    appointment => appointment.PropertyListingId.Value,
                    property => property.Id,
                    (appointment, property) => appointment.Id)
                .AnyAsync(cancellationToken);
        }

        public async Task<VisitConfirmationDto> FindConfirmationAsync(
            string slug,
            Guid appointmentId,
            CancellationToken cancellationToken = default)
        {
            var result = await dbContext.Appointments
                .AsNoTracking()
                .Where(appointment =>
                    appointment.Id == appointmentId &&
                    appointment.Status == AppointmentStatuses.Submitted &&
                    appointment.PropertyListingId.HasValue)
                .Join(
                    dbContext.PropertyListings.AsNoTracking().Where(property => property.Slug == slug),
                    appointment => appointment.PropertyListingId.Value,
                    property => property.Id,
                    (appointment, property) => new VisitConfirmationDto
                    {
                        AppointmentId = appointment.Id,
                        Property = ProjectProperty(property)
                    })
                .FirstOrDefaultAsync(cancellationToken);

            if (result == null)
            {
                return null;
            }

            result.SimilarProperties = await dbContext.PropertyListings
                .AsNoTracking()
                .Where(property => property.Slug != slug)
                .OrderBy(property => property.SortOrder)
                .Take(3)
                .Select(property => new PropertySummaryDto
                {
                    Id = property.Id,
                    Slug = property.Slug,
                    Title = property.Title,
                    Status = property.Status,
                    Price = property.Price,
                    Contract = property.Contract,
                    DisplayLocation = property.DisplayLocation,
                    Location = property.Location,
                    ImageUrl = property.ImageUrl
                })
                .ToListAsync(cancellationToken);

            return result;
        }

        private static VisitPropertySummaryDto ProjectProperty(PropertyListing property)
        {
            return new VisitPropertySummaryDto
            {
                Slug = property.Slug,
                Title = property.Title,
                ImageUrl = property.ImageUrl,
                Price = property.Price,
                Contract = property.Contract,
                DisplayLocation = property.DisplayLocation,
                Location = property.Location,
                SurfaceSquareMeters = property.SurfaceSquareMeters,
                Rooms = property.Rooms,
                Status = property.Status
            };
        }
    }

}
