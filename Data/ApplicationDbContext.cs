using Microsoft.EntityFrameworkCore;
using RaceCompetitionAPI.Models;

namespace RaceCompetitionAPI.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {

        }

        public DbSet<Event> Events { get; set; }
        public DbSet<Enrolment> Enrolments { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Organiser> Organisers { get; set; }
        public DbSet<Participant> Participants { get; set; }
        public DbSet<Result> Results { get; set; }
        public DbSet<Town> Towns { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Routes> Routes { get; set; }
        public DbSet<EventCategory> EventCategories{get; set; }
        public DbSet<Registration> Registrations { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<EventCategory>()
    .HasKey(ec => ec.eventCategoryID);

            modelBuilder.Entity<Routes>()
                .HasKey(r => r.RoutesID);
        }
    }
}
