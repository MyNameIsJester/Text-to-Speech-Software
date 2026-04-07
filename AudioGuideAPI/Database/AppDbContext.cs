using AudioGuideAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace AudioGuideAPI.Database
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Language> Languages => Set<Language>();
        public DbSet<FoodStall> FoodStalls => Set<FoodStall>();
        public DbSet<FoodStallTranslation> FoodStallTranslations => Set<FoodStallTranslation>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Language>(entity =>
            {
                entity.HasKey(x => x.Id);

                entity.HasIndex(x => x.LanguageCode).IsUnique();

                entity.Property(x => x.LanguageCode)
                      .IsRequired()
                      .HasMaxLength(10);

                entity.Property(x => x.DisplayName)
                      .IsRequired()
                      .HasMaxLength(100);

                entity.Property(x => x.FlagIcon)
                      .HasMaxLength(255);

                entity.Property(x => x.IntroText)
                      .HasMaxLength(500);
            });

            modelBuilder.Entity<FoodStall>(entity =>
            {
                entity.HasKey(x => x.Id);

                entity.Property(x => x.ImageUrl)
                      .HasMaxLength(255);

                entity.Property(x => x.Address)
                      .HasMaxLength(255);

                entity.Property(x => x.PriceRange)
                      .HasMaxLength(100);

                entity.Property(x => x.MapLink)
                      .HasMaxLength(500);
            });

            modelBuilder.Entity<FoodStallTranslation>(entity =>
            {
                entity.HasKey(x => x.Id);

                entity.Property(x => x.Name)
                      .IsRequired()
                      .HasMaxLength(200);

                entity.Property(x => x.Description)
                      .HasMaxLength(2000);

                entity.Property(x => x.Specialty)
                      .HasMaxLength(500);

                entity.Property(x => x.AudioUrl)
                      .HasMaxLength(255);

                entity.HasOne(x => x.FoodStall)
                      .WithMany(x => x.Translations)
                      .HasForeignKey(x => x.FoodStallId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(x => x.Language)
                      .WithMany(x => x.FoodStallTranslations)
                      .HasForeignKey(x => x.LanguageId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(x => new { x.FoodStallId, x.LanguageId })
                      .IsUnique();
            });
        }
    }
}