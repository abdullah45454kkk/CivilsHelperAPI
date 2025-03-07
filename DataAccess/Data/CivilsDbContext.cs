using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Models.Emergencies;
using Models.User;

namespace DataAccess.Data
{
    public class CivilsDbContext : IdentityDbContext<LocalUser>
    {
        public CivilsDbContext(DbContextOptions<CivilsDbContext> dbContextOptions) : base(dbContextOptions)
        {
        }

        public DbSet<EmergPerson> EmergPeople { get; set; }
        public DbSet<EmergAnother> EmergAnothers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<EmergPerson>()
                .HasOne(ep => ep.LocalUser)
                .WithMany(u => u.EmergencyPersons)
                .HasForeignKey(ep => ep.UserId);

            modelBuilder.Entity<EmergAnother>()
                .HasOne(ea => ea.LocalUser)
                .WithMany(u => u.EmergencyAnothers)
                .HasForeignKey(ea => ea.UserId);

            // Seed data (without passwords)
            //modelBuilder.Entity<EmergPerson>().HasData(
            //    new EmergPerson { Id = 1, Name = "John Doe", Age = 25, SSN = "123456789", LastSeenLocation = "1234 Main St", LastSeenDateTime = DateTime.Now, Description = "Last seen wearing a blue shirt and jeans", UserId = "1" },
            //    new EmergPerson { Id = 2, Name = "John Rock", Age = 30, SSN = "123456789", LastSeenLocation = "1234 Main St", LastSeenDateTime = DateTime.Now, Description = "Last seen wearing a blue shirt and jeans", UserId = "1" },
            //    new EmergPerson { Id = 3, Name = "John Deep", Age = 40, SSN = "123456789", LastSeenLocation = "1234 Main St", LastSeenDateTime = DateTime.Now, Description = "Last seen wearing a blue shirt and jeans", UserId = "1" }
            //);

            //modelBuilder.Entity<EmergAnother>().HasData(
            //    new EmergAnother { Id = 1, TitleType = "Food", Description = "Need Food", Location = "1234 Main St", UserId = "1" },
            //    new EmergAnother { Id = 2, TitleType = "Food", Description = "Need Food", Location = "1234 Main St", UserId = "1" },
            //    new EmergAnother { Id = 3, TitleType = "Clothes", Description = "Need Clothes", Location = "1234 Main St", UserId = "1" }
            //);
        }
    }
}