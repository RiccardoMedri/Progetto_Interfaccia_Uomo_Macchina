using Microsoft.EntityFrameworkCore;
using Medri.Services.Medri;
using Medri.Services.Medri.Identity;

namespace Medri.Services
{
    public class MedriDbContext : DbContext
    {
        public MedriDbContext()
        {
        }

        public MedriDbContext(DbContextOptions<MedriDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }

        public DbSet<AgencyUser> AgencyUsers { get; set; }

        public DbSet<PropertyListing> PropertyListings { get; set; }

        public DbSet<PropertyMedia> PropertyMedia { get; set; }

        public DbSet<FavoriteProperty> FavoriteProperties { get; set; }

        public DbSet<ClientSavedSearch> ClientSavedSearches { get; set; }

        public DbSet<ClientNotificationPreference> ClientNotificationPreferences { get; set; }

        public DbSet<Lead> Leads { get; set; }

        public DbSet<LeadPreference> LeadPreferences { get; set; }

        public DbSet<SearchProfile> SearchProfiles { get; set; }

        public DbSet<Interaction> Interactions { get; set; }

        public DbSet<Appointment> Appointments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PropertyListing>()
                .HasIndex(property => property.Slug)
                .IsUnique();

            modelBuilder.Entity<PropertyListing>()
                .HasQueryFilter(property =>
                    (property.InternalReference == null &&
                     (property.PublicationStatus == null ||
                      property.PublicationStatus == PropertyPublicationStatuses.Published)) ||
                    property.PublicationStatus == PropertyPublicationStatuses.Published);

            modelBuilder.Entity<PropertyListing>()
                .HasOne<AgencyUser>()
                .WithMany()
                .HasForeignKey(property => property.AssignedAgencyUserId);

            modelBuilder.Entity<PropertyMedia>()
                .HasOne<PropertyListing>()
                .WithMany()
                .HasForeignKey(media => media.PropertyListingId);

            modelBuilder.Entity<FavoriteProperty>()
                .HasIndex(favorite => new { favorite.UserId, favorite.PropertyListingId })
                .IsUnique();

            modelBuilder.Entity<FavoriteProperty>()
                .HasOne<User>()
                .WithMany()
                .HasForeignKey(favorite => favorite.UserId);

            modelBuilder.Entity<FavoriteProperty>()
                .HasOne<PropertyListing>()
                .WithMany()
                .HasForeignKey(favorite => favorite.PropertyListingId);

            modelBuilder.Entity<ClientSavedSearch>()
                .HasOne<User>()
                .WithMany()
                .HasForeignKey(savedSearch => savedSearch.UserId);

            modelBuilder.Entity<ClientNotificationPreference>()
                .HasIndex(preference => new { preference.UserId, preference.Category })
                .IsUnique();

            modelBuilder.Entity<ClientNotificationPreference>()
                .HasOne<User>()
                .WithMany()
                .HasForeignKey(preference => preference.UserId);

            modelBuilder.Entity<LeadPreference>()
                .HasOne<Lead>()
                .WithMany()
                .HasForeignKey(preference => preference.LeadId);

            modelBuilder.Entity<Lead>()
                .HasOne<User>()
                .WithMany()
                .HasForeignKey(lead => lead.ClientUserId);

            modelBuilder.Entity<Lead>()
                .HasOne<AgencyUser>()
                .WithMany()
                .HasForeignKey(lead => lead.AssignedAgencyUserId);

            modelBuilder.Entity<SearchProfile>()
                .HasOne<Lead>()
                .WithMany()
                .HasForeignKey(profile => profile.LeadId);

            modelBuilder.Entity<Interaction>()
                .HasOne<Lead>()
                .WithMany()
                .HasForeignKey(interaction => interaction.LeadId);

            modelBuilder.Entity<Appointment>()
                .HasOne<Lead>()
                .WithMany()
                .HasForeignKey(appointment => appointment.LeadId);

            modelBuilder.Entity<Appointment>()
                .HasOne<PropertyListing>()
                .WithMany()
                .HasForeignKey(appointment => appointment.PropertyListingId);

            modelBuilder.Entity<Appointment>()
                .HasOne<AgencyUser>()
                .WithMany()
                .HasForeignKey(appointment => appointment.AgencyUserId);

            base.OnModelCreating(modelBuilder);
        }
    }
}
