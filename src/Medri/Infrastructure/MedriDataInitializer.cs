using Microsoft.Extensions.DependencyInjection;
using System;
using Medri.Services;

namespace Medri.Infrastructure
{
    public static class MedriDataInitializer
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<MedriDbContext>();
            DataGenerator.InitializeUsers(dbContext);
            DataGenerator.InitializeMedriDemoData(dbContext);
        }
    }
}
