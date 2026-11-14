
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Smart_Community_Health_Monitoring_System.Models.Entities;
using Smart_Community_Health_Monitoring_System.Models.Identity;

namespace Smart_Community_Health_Monitoring_System.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Community> Communities => Set<Community>();
    public DbSet<WaterSource> WaterSources => Set<WaterSource>();
    public DbSet<WaterObservation> WaterObservations => Set<WaterObservation>();
    public DbSet<HealthReport> HealthReports => Set<HealthReport>();
    public DbSet<EarlyWarningAlert> EarlyWarningAlerts => Set<EarlyWarningAlert>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Community>(entity =>
        {
            entity.Property(x => x.Name).HasMaxLength(150).IsRequired();
            entity.Property(x => x.District).HasMaxLength(150).IsRequired();
            entity.Property(x => x.State).HasMaxLength(100).IsRequired();
        });

        modelBuilder.Entity<WaterSource>(entity =>
        {
            entity.Property(x => x.Name).HasMaxLength(150).IsRequired();
            entity.Property(x => x.SourceType).HasMaxLength(80).IsRequired();

            entity.HasOne(x => x.Community)
                .WithMany(x => x.WaterSources)
                .HasForeignKey(x => x.CommunityId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<WaterObservation>(entity =>
        {
            entity.Property(x => x.Parameter).HasMaxLength(100).IsRequired();
            entity.Property(x => x.Unit).HasMaxLength(30).IsRequired();
            entity.Property(x => x.Value).HasPrecision(12, 4);

            entity.HasOne(x => x.WaterSource)
                .WithMany(x => x.Observations)
                .HasForeignKey(x => x.WaterSourceId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(x => new { x.WaterSourceId, x.ObservedAtUtc });
        });

        modelBuilder.Entity<HealthReport>(entity =>
        {
            entity.Property(x => x.Category).HasMaxLength(120).IsRequired();
            entity.Property(x => x.Source).HasMaxLength(100);

            entity.HasOne(x => x.Community)
                .WithMany(x => x.HealthReports)
                .HasForeignKey(x => x.CommunityId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(x => new { x.CommunityId, x.ReportedAtUtc });
        });

        modelBuilder.Entity<EarlyWarningAlert>(entity =>
        {
            entity.Property(x => x.Title).HasMaxLength(180).IsRequired();
            entity.Property(x => x.Description).HasMaxLength(1000).IsRequired();

            entity.Property(x => x.Severity)
                .HasConversion<string>()
                .HasMaxLength(20);

            entity.Property(x => x.Status)
                .HasConversion<string>()
                .HasMaxLength(20);

            entity.HasOne(x => x.Community)
                .WithMany(x => x.Alerts)
                .HasForeignKey(x => x.CommunityId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
