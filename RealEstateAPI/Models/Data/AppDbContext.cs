using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace RealEstateAPI.Models.Data
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        // Real Estate Entities
        public DbSet<Property> Properties { get; set; }
        public DbSet<PriceEstimate> PriceEstimates { get; set; }
        public DbSet<Image> Images { get; set; }
        public DbSet<Favorite> Favorites { get; set; }
        public DbSet<Filter> Filters { get; set; }
        public DbSet<Booking> Bookings { get; set; } // 📌 NEW: Add Bookings Table

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Ensure UserId in all entities matches Identity default type (string)
            modelBuilder.Entity<Property>()
                .Property(p => p.UserId)
                .HasColumnType("nvarchar(450)");

            modelBuilder.Entity<Favorite>()
                .Property(f => f.UserId)
                .HasColumnType("nvarchar(450)");

            modelBuilder.Entity<Filter>()
                .Property(f => f.UserId)
                .HasColumnType("nvarchar(450)");

            // Property - User Relationship
            modelBuilder.Entity<Property>()
                .HasOne(p => p.User)
                .WithMany(u => u.Properties)
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // 📌 NEW: Booking - Property Relationship
            modelBuilder.Entity<Booking>()
                .HasOne(b => b.Property)
                .WithMany(p => p.Bookings)
                .HasForeignKey(b => b.PropertyId)
                .OnDelete(DeleteBehavior.Cascade);

            // 📌 NEW: Booking - User Relationship
            modelBuilder.Entity<Booking>()
                .HasOne(b => b.User)
                .WithMany(u => u.Bookings)
                .HasForeignKey(b => b.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Price Estimate - Property Relationship
            modelBuilder.Entity<PriceEstimate>()
                .HasOne(pe => pe.Property)
                .WithMany(p => p.PriceEstimates)
                .HasForeignKey(pe => pe.PropertyId)
                .OnDelete(DeleteBehavior.Cascade);

            // Image - Property Relationship
            modelBuilder.Entity<Image>()
                .HasOne(i => i.Property)
                .WithMany(p => p.Images)
                .HasForeignKey(i => i.PropertyId)
                .OnDelete(DeleteBehavior.Cascade);

            // Favorite - User Relationship
            modelBuilder.Entity<Favorite>()
                .HasOne(f => f.User)
                .WithMany(u => u.Favorites)
                .HasForeignKey(f => f.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Filter - User Relationship
            modelBuilder.Entity<Filter>()
                .HasOne(f => f.User)
                .WithMany(u => u.Filters)
                .HasForeignKey(f => f.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
