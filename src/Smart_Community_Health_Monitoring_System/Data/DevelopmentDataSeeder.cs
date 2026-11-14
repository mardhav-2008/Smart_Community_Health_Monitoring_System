
using Microsoft.EntityFrameworkCore;
using Smart_Community_Health_Monitoring_System.Models.Entities;

namespace Smart_Community_Health_Monitoring_System.Data;

public static class DevelopmentDataSeeder
{
    public static async Task SeedAsync(ApplicationDbContext context)
    {
        // Prevent duplicate sample data when the app restarts.
        if (await context.Communities.AnyAsync())
            return;

        await using var transaction =
            await context.Database.BeginTransactionAsync();

        var now = DateTime.UtcNow;

        var communities = new List<Community>
        {
            new()
            {
                Name = "Riverbend Community",
                District = "Kamrup",
                State = "Assam",
                Description = "Fictional demonstration community."
            },
            new()
            {
                Name = "Hillview Community",
                District = "Ri-Bhoi",
                State = "Meghalaya",
                Description = "Fictional demonstration community."
            },
            new()
            {
                Name = "Green Valley Community",
                District = "Dima Hasao",
                State = "Assam",
                Description = "Fictional demonstration community."
            }
        };

        context.Communities.AddRange(communities);
        await context.SaveChangesAsync();

        var sources = new List<WaterSource>
        {
            new()
            {
                Name = "Riverbend Public Well",
                SourceType = "Well",
                CommunityId = communities[0].Id,
                IsActive = true,
                Latitude = 26.15,
                Longitude = 91.72
            },
            new()
            {
                Name = "Riverbend Community Tap",
                SourceType = "Tap",
                CommunityId = communities[0].Id,
                IsActive = true
            },
            new()
            {
                Name = "Hillview Spring",
                SourceType = "Spring",
                CommunityId = communities[1].Id,
                IsActive = true,
                Latitude = 25.95,
                Longitude = 91.88
            },
            new()
            {
                Name = "Hillview Storage Tank",
                SourceType = "Storage Tank",
                CommunityId = communities[1].Id,
                IsActive = true
            },
            new()
            {
                Name = "Green Valley Hand Pump",
                SourceType = "Hand Pump",
                CommunityId = communities[2].Id,
                IsActive = true
            },
            new()
            {
                Name = "Green Valley Backup Well",
                SourceType = "Well",
                CommunityId = communities[2].Id,
                IsActive = false
            }
        };

        context.WaterSources.AddRange(sources);
        await context.SaveChangesAsync();

        var observations = new List<WaterObservation>
        {
            new()
            {
                WaterSourceId = sources[0].Id,
                Parameter = "pH",
                Value = 7.2m,
                Unit = "pH",
                ObservedAtUtc = now.AddHours(-2),
                IsValidated = true
            },
            new()
            {
                WaterSourceId = sources[0].Id,
                Parameter = "Turbidity",
                Value = 2.1m,
                Unit = "NTU",
                ObservedAtUtc = now.AddHours(-1),
                IsValidated = false
            },
            new()
            {
                WaterSourceId = sources[1].Id,
                Parameter = "pH",
                Value = 6.9m,
                Unit = "pH",
                ObservedAtUtc = now.AddHours(-5),
                IsValidated = true
            },
            new()
            {
                WaterSourceId = sources[2].Id,
                Parameter = "Turbidity",
                Value = 1.4m,
                Unit = "NTU",
                ObservedAtUtc = now.AddHours(-3),
                IsValidated = true
            },
            new()
            {
                WaterSourceId = sources[3].Id,
                Parameter = "Temperature",
                Value = 24.6m,
                Unit = "°C",
                ObservedAtUtc = now.AddHours(-4),
                IsValidated = false
            },
            new()
            {
                WaterSourceId = sources[4].Id,
                Parameter = "Conductivity",
                Value = 210m,
                Unit = "µS/cm",
                ObservedAtUtc = now.AddHours(-6),
                IsValidated = true
            }
        };

        context.WaterObservations.AddRange(observations);

        var reports = new List<HealthReport>
        {
            new()
            {
                CommunityId = communities[0].Id,
                Category = "Gastrointestinal",
                ReportCount = 5,
                ReportedAtUtc = now.AddDays(-2),
                Source = "Demonstration dataset",
                IsVerified = false
            },
            new()
            {
                CommunityId = communities[0].Id,
                Category = "Fever",
                ReportCount = 3,
                ReportedAtUtc = now.AddDays(-1),
                Source = "Demonstration dataset",
                IsVerified = false
            },
            new()
            {
                CommunityId = communities[1].Id,
                Category = "Gastrointestinal",
                ReportCount = 4,
                ReportedAtUtc = now.AddDays(-3),
                Source = "Demonstration dataset",
                IsVerified = false
            },
            new()
            {
                CommunityId = communities[1].Id,
                Category = "Fever",
                ReportCount = 6,
                ReportedAtUtc = now.AddDays(-1),
                Source = "Demonstration dataset",
                IsVerified = false
            },
            new()
            {
                CommunityId = communities[2].Id,
                Category = "Other",
                ReportCount = 2,
                ReportedAtUtc = now.AddDays(-2),
                Source = "Demonstration dataset",
                IsVerified = false
            }
        };

        context.HealthReports.AddRange(reports);

        var alerts = new List<EarlyWarningAlert>
        {
            new()
            {
                CommunityId = communities[0].Id,
                Title = "Sample water-quality review",
                Description =
                    "Illustrative alert for testing the review workflow. " +
                    "This is not a real contamination finding.",
                Severity = AlertSeverity.Moderate,
                Status = AlertStatus.New,
                CreatedAtUtc = now.AddHours(-3)
            },
            new()
            {
                CommunityId = communities[1].Id,
                Title = "Sample health-report review",
                Description =
                    "Illustrative alert for testing the review workflow. " +
                    "This is not a real disease outbreak notification.",
                Severity = AlertSeverity.Low,
                Status = AlertStatus.UnderReview,
                CreatedAtUtc = now.AddHours(-8)
            }
        };

        context.EarlyWarningAlerts.AddRange(alerts);

        await context.SaveChangesAsync();
        await transaction.CommitAsync();
    }
}
