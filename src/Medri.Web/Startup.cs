using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.IO.Compression;
using System.Linq;
using Medri.Infrastructure;
using Medri.Services;
using Medri.Web.Infrastructure;

namespace Medri.Web
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public IWebHostEnvironment Env { get; set; }

        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Env = env;
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<GoogleMapsOptions>(Configuration.GetSection("GoogleMaps"));
            services.Configure<AgencyContactOptions>(Configuration.GetSection("AgencyContact"));

            services.AddResponseCompression(options =>
            {
                options.EnableForHttps = true;
                options.Providers.Add<BrotliCompressionProvider>();
                options.Providers.Add<GzipCompressionProvider>();
                options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
                    new[] { "image/svg+xml" });
            });
            services.Configure<BrotliCompressionProviderOptions>(o => o.Level = CompressionLevel.Optimal);
            services.Configure<GzipCompressionProviderOptions>(o => o.Level = CompressionLevel.Optimal);

            services.AddMedriData();

            services.AddSession();
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(options =>
            {
                options.LoginPath = "/login";
                options.LogoutPath = "/Login/Logout";
                options.ReturnUrlParameter = "ReturnUrl";
                options.SlidingExpiration = true;
                options.Events.OnRedirectToAccessDenied = context =>
                {
                    context.Response.StatusCode = Microsoft.AspNetCore.Http.StatusCodes.Status403Forbidden;
                    return System.Threading.Tasks.Task.CompletedTask;
                };
            });
            services.AddAuthorization();

            var builder = services.AddMvc(options =>
            {
                options.Filters.Add<AlertsAttribute>();
            });

#if DEBUG
            builder.AddRazorRuntimeCompilation();
#endif

            services.Configure<RazorViewEngineOptions>(options =>
            {
                options.AreaViewLocationFormats.Clear();
                options.AreaViewLocationFormats.Add("/Areas/{2}/{1}/{0}.cshtml");
                options.AreaViewLocationFormats.Add("/Areas/{2}/Views/{1}/{0}.cshtml");
                options.AreaViewLocationFormats.Add("/Areas/{2}/Views/Shared/{0}.cshtml");
                options.AreaViewLocationFormats.Add("/Views/Shared/{0}.cshtml");

                options.ViewLocationFormats.Clear();
                options.ViewLocationFormats.Add("/Features/{1}/{0}.cshtml");
                options.ViewLocationFormats.Add("/Features/Views/{1}/{0}.cshtml");
                options.ViewLocationFormats.Add("/Features/Views/Shared/{0}.cshtml");
                options.ViewLocationFormats.Add("/Views/Shared/{0}.cshtml");
            });

            services.AddMedriWebServices();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            MedriDataInitializer.Initialize(app.ApplicationServices);

            if (!env.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");

                app.UseHsts();
                app.UseHttpsRedirection();
            }

            app.UseResponseCompression();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseSession();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapAreaControllerRoute("admin-lead-create", "Admin", "admin/lead/nuovo", new { controller = "Leads", action = "Create" });
                endpoints.MapAreaControllerRoute("admin-lead-archive", "Admin", "admin/lead/{reference}/archivia", new { controller = "Leads", action = "Archive" });
                endpoints.MapAreaControllerRoute("admin-lead-convert", "Admin", "admin/lead/{reference}/converti", new { controller = "Leads", action = "Convert" });
                endpoints.MapAreaControllerRoute("admin-lead-restore", "Admin", "admin/lead/{reference}/ripristina", new { controller = "Leads", action = "Restore" });
                endpoints.MapAreaControllerRoute("admin-lead-update", "Admin", "admin/lead/{reference}/salva", new { controller = "Leads", action = "Update" });
                endpoints.MapAreaControllerRoute("admin-lead-bulk-assign", "Admin", "admin/lead/assegna", new { controller = "Leads", action = "BulkAssign" });
                endpoints.MapAreaControllerRoute("admin-lead-bulk-convert", "Admin", "admin/lead/converti-selezionati", new { controller = "Leads", action = "BulkConvert" });
                endpoints.MapAreaControllerRoute("admin-lead-bulk-archive", "Admin", "admin/lead/archivia-selezionati", new { controller = "Leads", action = "BulkArchive" });
                endpoints.MapAreaControllerRoute("admin-lead-detail", "Admin", "admin/lead/{reference}", new { controller = "Leads", action = "Detail" });
                endpoints.MapAreaControllerRoute("admin-lead", "Admin", "admin/lead", new { controller = "Leads", action = "Index" });
                endpoints.MapAreaControllerRoute("admin-requests", "Admin", "admin/richieste", new { controller = "Requests", action = "Index" });
                endpoints.MapAreaControllerRoute("admin-request-create", "Admin", "admin/richieste/nuova", new { controller = "Requests", action = "Create" });
                endpoints.MapAreaControllerRoute("admin-request-archive", "Admin", "admin/richieste/{reference}/archivia", new { controller = "Requests", action = "Archive" });
                endpoints.MapAreaControllerRoute("admin-request-update", "Admin", "admin/richieste/{reference}/salva", new { controller = "Requests", action = "Update" });
                endpoints.MapAreaControllerRoute("admin-request-edit", "Admin", "admin/richieste/{reference}", new { controller = "Requests", action = "Edit" });
                endpoints.MapAreaControllerRoute("admin-properties", "Admin", "admin/immobili", new { controller = "Properties", action = "Index" });
                endpoints.MapAreaControllerRoute("admin-property-create", "Admin", "admin/immobili/nuovo", new { controller = "Properties", action = "Create" });
                endpoints.MapAreaControllerRoute("admin-property-geocode", "Admin", "admin/immobili/geocodifica", new { controller = "Properties", action = "Geocode" });
                endpoints.MapAreaControllerRoute("admin-property-ready", "Admin", "admin/immobili/{reference}/segna-pronto", new { controller = "Properties", action = "MarkReady" });
                endpoints.MapAreaControllerRoute("admin-property-update", "Admin", "admin/immobili/{reference}/salva", new { controller = "Properties", action = "Update" });
                endpoints.MapAreaControllerRoute("admin-property-preview", "Admin", "admin/immobili/{reference}/anteprima", new { controller = "Properties", action = "Preview" });
                endpoints.MapAreaControllerRoute("admin-property-preview-map", "Admin", "admin/immobili/{reference}/anteprima-mappa", new { controller = "Properties", action = "PreviewMap" });
                endpoints.MapAreaControllerRoute("admin-property-discard", "Admin", "admin/immobili/{reference}/scarta", new { controller = "Properties", action = "Discard" });
                endpoints.MapAreaControllerRoute("admin-property-edit", "Admin", "admin/immobili/{reference}", new { controller = "Properties", action = "Edit" });
                endpoints.MapAreaControllerRoute("admin", "Admin", "admin/{controller=Dashboard}/{action=Index}/{id?}");
                endpoints.MapAreaControllerRoute("client-saved", "Client", "client-saved", new { controller = "Saved", action = "Index" });
                endpoints.MapAreaControllerRoute("client-requests", "Client", "client-requests", new { controller = "Requests", action = "Index" });
                endpoints.MapAreaControllerRoute("client-notifications-update", "Client", "client-notifications/update", new { controller = "Notifications", action = "Update" });
                endpoints.MapAreaControllerRoute("client-notifications", "Client", "client-notifications", new { controller = "Notifications", action = "Index" });
                endpoints.MapAreaControllerRoute("client-searches-remove", "Client", "client-searches/rimuovi", new { controller = "Searches", action = "Remove" });
                endpoints.MapAreaControllerRoute("client-searches", "Client", "client-searches", new { controller = "Searches", action = "Index" });
                endpoints.MapControllerRoute("login", "login", new { controller = "Login", action = "Login" });
                endpoints.MapControllerRoute("register", "registrazione", new { controller = "Login", action = "Register" });
                endpoints.MapControllerRoute("property-search", "immobili", new { controller = "Search", action = "Index" });
                endpoints.MapControllerRoute("property-request-confirmation", "immobili/{slug}/richiedi/{appointmentId}/inviata", new { controller = "Visit", action = "Confirmation" });
                endpoints.MapControllerRoute("property-request-submit", "immobili/{slug}/richiedi/{appointmentId}/invia", new { controller = "Visit", action = "Submit" });
                endpoints.MapControllerRoute("property-request-review", "immobili/{slug}/richiedi/{appointmentId}/riepilogo", new { controller = "Visit", action = "Review" });
                endpoints.MapControllerRoute("property-request-start", "immobili/{slug}/richiedi/inizia", new { controller = "Visit", action = "Start" });
                endpoints.MapControllerRoute("property-request", "immobili/{slug}/richiedi", new { controller = "Visit", action = "Option" });
                endpoints.MapControllerRoute("property-detail", "immobili/{slug}", new { controller = "Property", action = "Detail" });
                endpoints.MapControllerRoute("lead-pending-complete", "richieste/completa-pendente", new { controller = "LeadIntake", action = "CompletePending" });
                endpoints.MapControllerRoute("lead-confirmation", "richieste/{id}/inviata", new { controller = "LeadIntake", action = "Confirmation" });
                endpoints.MapControllerRoute("lead-buy", "richieste/comprare", new { controller = "LeadIntake", action = "Buy" });
                endpoints.MapControllerRoute("lead-rent", "richieste/affittare", new { controller = "LeadIntake", action = "Rent" });
                endpoints.MapControllerRoute("lead-sell", "richieste/vendere", new { controller = "LeadIntake", action = "Sell" });
                endpoints.MapControllerRoute("lead-rent-out", "richieste/affidare-affitto", new { controller = "LeadIntake", action = "RentOut" });
                endpoints.MapControllerRoute("valuation", "valutazione", new { controller = "LeadIntake", action = "Valuation" });
                endpoints.MapControllerRoute("comparison-remove", "confronto/rimuovi", new { controller = "Comparison", action = "Remove" });
                endpoints.MapControllerRoute("comparison", "confronto", new { controller = "Comparison", action = "Index" });
                endpoints.MapControllerRoute("favorites-add", "preferiti/aggiungi", new { controller = "Favorites", action = "Add" });
                endpoints.MapControllerRoute("favorites-remove", "preferiti/rimuovi", new { controller = "Favorites", action = "Remove" });
                endpoints.MapControllerRoute("saved-search-complete-pending", "ricerche/completa-pendente", new { controller = "SavedSearches", action = "CompletePending" });
                endpoints.MapControllerRoute("saved-search", "ricerche/salva", new { controller = "SavedSearches", action = "Save" });
                endpoints.MapControllerRoute("default", "{controller=Home}/{action=Index}");
            });
        }
    }
}
