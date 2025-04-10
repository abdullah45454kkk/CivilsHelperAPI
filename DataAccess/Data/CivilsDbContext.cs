using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Models.Donations;
using Models.Emergencies;
using Models.Map;
using Models.User;

namespace DataAccess.Data
{
    public class CivilsDbContext : IdentityDbContext<LocalUser>
    {
        public CivilsDbContext(DbContextOptions<CivilsDbContext> options)
            : base(options)
        {
        }

        public DbSet<EmergPerson> EmergPersons { get; set; }
        public DbSet<EmergAnother> EmergAnothers { get; set; }
        public DbSet<Donation> Donation { get; set; }
        public DbSet<Campaign> Campaigns { get; set; }
        public DbSet<GeographicArea> GeographicAreas { get; set; }
        public DbSet<GeoCoordinate> GeoCoordinates { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure relationships

            // LocalUser to Donation (one-to-many)
            modelBuilder.Entity<Donation>()
                .HasOne(d => d.LocalUser)
                .WithMany(u => u.Donations)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Campaign to Donation (one-to-many)
            modelBuilder.Entity<Donation>()
                .HasOne(d => d.Campaign)
                .WithMany(c => c.Donations)
                .HasForeignKey(d => d.CampaignId)
                .OnDelete(DeleteBehavior.Cascade);

            // LocalUser to EmergPerson (one-to-many)
            modelBuilder.Entity<EmergPerson>()
                .HasOne(ep => ep.LocalUser)
                .WithMany(u => u.EmergencyPersons)
                .HasForeignKey(ep => ep.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // LocalUser to EmergAnother (one-to-many)
            modelBuilder.Entity<EmergAnother>()
                .HasOne(ea => ea.LocalUser)
                .WithMany(u => u.EmergencyAnothers)
                .HasForeignKey(ea => ea.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure GeoCoordinate as an owned entity
            modelBuilder.Entity<GeographicArea>()
                .OwnsOne(ga => ga.NorthWest, nw =>
                {
                    nw.Property(c => c.Latitude).HasColumnName("NorthWest_Latitude");
                    nw.Property(c => c.Longitude).HasColumnName("NorthWest_Longitude");
                });

            modelBuilder.Entity<GeographicArea>()
                .OwnsOne(ga => ga.SouthEast, se =>
                {
                    se.Property(c => c.Latitude).HasColumnName("SouthEast_Latitude");
                    se.Property(c => c.Longitude).HasColumnName("SouthEast_Longitude");
                });

            // Many-to-Many: Self-referencing relationship for RelatedAreas
            modelBuilder.Entity<GeographicArea>()
                .HasMany(ga => ga.RelatedAreas)
                .WithMany()
                .UsingEntity<Dictionary<string, object>>(
                    "GeographicAreaRelatedAreas",
                    j => j.HasOne<GeographicArea>().WithMany().HasForeignKey("RelatedAreaId"),
                    j => j.HasOne<GeographicArea>().WithMany().HasForeignKey("GeographicAreaId"));
        }
    }
}