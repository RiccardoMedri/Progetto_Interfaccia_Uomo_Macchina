using Microsoft.Extensions.DependencyInjection;
using System;
using Medri.Services;
using Medri.Web.Areas.Admin.Properties;
using Medri.Web.Features.Home;

namespace Medri.Web
{
    public static class ServiceRegistration
    {
        public static IServiceCollection AddMedriWebServices(this IServiceCollection services)
        {
            services.AddMedriServices();
            services.AddHttpClient<IGoogleReviewsService, GooglePlacesReviewsService>(client =>
            {
                client.BaseAddress = new Uri("https://places.googleapis.com/v1/");
                client.Timeout = TimeSpan.FromSeconds(5);
            });
            services.AddScoped<IAdminPropertyMediaStorage, AdminPropertyMediaStorage>();

            return services;
        }
    }
}
