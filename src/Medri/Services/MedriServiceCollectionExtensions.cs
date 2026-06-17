using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Medri.Services.Medri.Application;
using Medri.Services.Medri.Identity;

namespace Medri.Services
{
    public static class MedriServiceCollectionExtensions
    {
        public static IServiceCollection AddMedriData(this IServiceCollection services)
        {
            services.AddDbContext<MedriDbContext>(options =>
            {
                options.UseInMemoryDatabase(databaseName: "Medri");
            });

            return services;
        }

        public static IServiceCollection AddMedriServices(this IServiceCollection services)
        {
            services.AddScoped<AccountService>();
            services.AddPropertyServices();
            services.AddLeadServices();
            services.AddAdminServices();
            return services;
        }

        public static IServiceCollection AddPropertyServices(this IServiceCollection services)
        {
            services.AddScoped<SearchListingsQuery>();
            services.AddScoped<HomeFeaturedListingsQuery>();
            services.AddScoped<PropertyDetailQuery>();
            services.AddScoped<ComparisonQuery>();
            services.AddScoped<GetFavoritePropertiesQuery>();
            services.AddScoped<FavoritePropertyCountQuery>();
            services.AddScoped<AddFavoritePropertyCommand>();
            services.AddScoped<RemoveFavoritePropertyCommand>();
            return services;
        }

        public static IServiceCollection AddLeadServices(this IServiceCollection services)
        {
            services.AddScoped<SaveClientSearchCommand>();
            services.AddScoped<RemoveClientSavedSearchCommand>();
            services.AddScoped<ClientSavedSearchesQuery>();
            services.AddScoped<SubmitLeadRequestCommand>();
            services.AddScoped<LeadConfirmationQuery>();
            services.AddScoped<PropertyContactQuery>();
            services.AddScoped<CreatePropertyContactRequestCommand>();
            services.AddScoped<SubmitPropertyContactRequestCommand>();
            services.AddScoped<ClientRequestsQuery>();
            services.AddScoped<ClientRequestCountQuery>();
            services.AddScoped<ClientNotificationPreferencesQuery>();
            services.AddScoped<UpdateClientNotificationPreferenceCommand>();
            return services;
        }

        public static IServiceCollection AddAdminServices(this IServiceCollection services)
        {
            services.AddScoped<AdminDashboardQuery>();
            services.AddScoped<AdminCreateLeadCommand>();
            services.AddScoped<AdminCreateRequestCommand>();
            services.AddScoped<AdminCreatePropertyCommand>();
            services.AddScoped<AdminLeadListQuery>();
            services.AddScoped<AdminLeadDetailQuery>();
            services.AddScoped<UpdateAdminLeadDetailCommand>();
            services.AddScoped<ConvertAdminLeadToRequestCommand>();
            services.AddScoped<ArchiveAdminLeadCommand>();
            services.AddScoped<RestoreAdminLeadCommand>();
            services.AddScoped<BulkAssignAdminLeadsCommand>();
            services.AddScoped<BulkConvertAdminLeadsCommand>();
            services.AddScoped<BulkArchiveAdminLeadsCommand>();
            services.AddScoped<AdminRequestListQuery>();
            services.AddScoped<AdminRequestDetailQuery>();
            services.AddScoped<UpdateAdminRequestDetailCommand>();
            services.AddScoped<ArchiveAdminRequestCommand>();
            services.AddScoped<BulkAssignAdminRequestsCommand>();
            services.AddScoped<AdminPropertyListQuery>();
            services.AddScoped<AdminPropertyDetailQuery>();
            services.AddScoped<UpdateAdminPropertyDetailCommand>();
            services.AddScoped<MarkAdminPropertyReadyCommand>();
            services.AddScoped<PublishAdminPropertyCommand>();
            services.AddScoped<ArchiveAdminPropertyCommand>();
            services.AddScoped<DiscardDraftAdminPropertyCommand>();
            services.AddScoped<BulkAssignAdminPropertiesCommand>();
            services.AddScoped<FeatureAdminPropertyCommand>();
            services.AddScoped<MoveFeaturedAdminPropertyCommand>();
            services.AddScoped<RemoveFeaturedAdminPropertyCommand>();
            services.AddScoped<AddAdminPropertyMediaCommand>();
            return services;
        }
    }
}
