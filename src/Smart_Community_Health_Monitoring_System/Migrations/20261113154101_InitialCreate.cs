using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Smart_Community_Health_Monitoring_System.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Communities",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 150, nullable: false),
                    District = table.Column<string>(type: "TEXT", maxLength: 150, nullable: false),
                    State = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Communities", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EarlyWarningAlerts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CommunityId = table.Column<int>(type: "INTEGER", nullable: false),
                    Title = table.Column<string>(type: "TEXT", maxLength: 180, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: false),
                    Severity = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    Status = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ReviewedAtUtc = table.Column<DateTime>(type: "TEXT", nullable: true),
                    ResolvedAtUtc = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EarlyWarningAlerts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EarlyWarningAlerts_Communities_CommunityId",
                        column: x => x.CommunityId,
                        principalTable: "Communities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "HealthReports",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CommunityId = table.Column<int>(type: "INTEGER", nullable: false),
                    Category = table.Column<string>(type: "TEXT", maxLength: 120, nullable: false),
                    ReportCount = table.Column<int>(type: "INTEGER", nullable: false),
                    ReportedAtUtc = table.Column<DateTime>(type: "TEXT", nullable: false),
                    RecordedAtUtc = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Source = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    IsVerified = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HealthReports", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HealthReports_Communities_CommunityId",
                        column: x => x.CommunityId,
                        principalTable: "Communities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "WaterSources",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 150, nullable: false),
                    SourceType = table.Column<string>(type: "TEXT", maxLength: 80, nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    Latitude = table.Column<double>(type: "REAL", nullable: true),
                    Longitude = table.Column<double>(type: "REAL", nullable: true),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    CommunityId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WaterSources", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WaterSources_Communities_CommunityId",
                        column: x => x.CommunityId,
                        principalTable: "Communities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "WaterObservations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    WaterSourceId = table.Column<int>(type: "INTEGER", nullable: false),
                    Parameter = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Value = table.Column<decimal>(type: "TEXT", precision: 12, scale: 4, nullable: false),
                    Unit = table.Column<string>(type: "TEXT", maxLength: 30, nullable: false),
                    ObservedAtUtc = table.Column<DateTime>(type: "TEXT", nullable: false),
                    RecordedAtUtc = table.Column<DateTime>(type: "TEXT", nullable: false),
                    IsValidated = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WaterObservations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WaterObservations_WaterSources_WaterSourceId",
                        column: x => x.WaterSourceId,
                        principalTable: "WaterSources",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EarlyWarningAlerts_CommunityId",
                table: "EarlyWarningAlerts",
                column: "CommunityId");

            migrationBuilder.CreateIndex(
                name: "IX_HealthReports_CommunityId_ReportedAtUtc",
                table: "HealthReports",
                columns: new[] { "CommunityId", "ReportedAtUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_WaterObservations_WaterSourceId_ObservedAtUtc",
                table: "WaterObservations",
                columns: new[] { "WaterSourceId", "ObservedAtUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_WaterSources_CommunityId",
                table: "WaterSources",
                column: "CommunityId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EarlyWarningAlerts");

            migrationBuilder.DropTable(
                name: "HealthReports");

            migrationBuilder.DropTable(
                name: "WaterObservations");

            migrationBuilder.DropTable(
                name: "WaterSources");

            migrationBuilder.DropTable(
                name: "Communities");
        }
    }
}
