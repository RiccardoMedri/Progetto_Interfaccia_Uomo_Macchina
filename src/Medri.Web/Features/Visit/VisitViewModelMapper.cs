using System.Linq;
using Medri.Services.Medri.Application;
using Medri.Web.Features.LeadIntake;
using Medri.Web.Features.Property;

namespace Medri.Web.Features.Visit
{
    internal static class VisitViewModelMapper
    {
        public static VisitPropertySummaryViewModel Create(VisitPropertySummaryDto property)
        {
            return new VisitPropertySummaryViewModel
            {
                Slug = property.Slug,
                Contract = property.Contract,
                Title = property.Title,
                ImageUrl = property.ImageUrl,
                PriceLabel = PropertyFormatting.FormatPrice(property.Price, property.Contract),
                DisplayLocation = PropertyFormatting.DisplayOrFallback(property.DisplayLocation, property.Location),
                SurfaceLabel = property.SurfaceSquareMeters + " mq",
                RoomsLabel = property.Rooms + " locali",
                Tag = property.Status
            };
        }

        public static VisitReviewViewModel Create(VisitReviewDto result)
        {
            return new VisitReviewViewModel
            {
                AppointmentId = result.AppointmentId,
                Property = Create(result.Property),
                FullName = result.FullName,
                Phone = result.Phone,
                Email = result.Email,
                Message = result.Message,
                PreferredContactMode = result.PreferredContactMode,
                WhenLabel = LeadIntakeMapper.ToWhenLabel(
                    result.PreferredTimeSlot,
                    result.PreferredDay,
                    result.PreferredTime)
            };
        }

        public static VisitOptionViewModel CreateOption(VisitReviewDto result)
        {
            return new VisitOptionViewModel
            {
                AppointmentId = result.AppointmentId,
                Property = Create(result.Property),
                FullName = result.FullName,
                Phone = result.Phone,
                Email = result.Email,
                Message = result.Message,
                PreferredContactMode = result.PreferredContactMode,
                PreferredTimeSlot = result.PreferredTimeSlot,
                PreferredDay = result.PreferredDay,
                PreferredTime = result.PreferredTime
            };
        }

        public static VisitConfirmationViewModel Create(VisitConfirmationDto result)
        {
            return new VisitConfirmationViewModel
            {
                Property = Create(result.Property),
                SimilarProperties = result.SimilarProperties
                    .Select(PropertyViewModelMapper.CreateSummary)
                    .ToList()
            };
        }

        public static PropertyContactRequestDto Create(VisitOptionViewModel input)
        {
            return new PropertyContactRequestDto
            {
                FullName = input.FullName,
                Phone = input.Phone,
                Email = input.Email,
                PreferredContactMode = input.PreferredContactMode,
                PreferredTimeSlot = input.PreferredTimeSlot,
                PreferredDay = input.PreferredDay,
                PreferredTime = input.PreferredTime,
                Message = input.Message
            };
        }

        public static PropertyContactRequestDto Create(PropertyContactStartViewModel input)
        {
            return new PropertyContactRequestDto
            {
                FullName = input.FullName,
                Phone = input.Phone,
                Email = input.Email,
                Message = input.Message
            };
        }
    }
}
