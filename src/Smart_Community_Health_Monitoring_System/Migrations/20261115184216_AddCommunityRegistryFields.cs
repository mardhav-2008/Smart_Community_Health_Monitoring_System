using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Smart_Community_Health_Monitoring_System.Migrations
{
    /// <inheritdoc />
    public partial class AddCommunityRegistryFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ApproximatePopulation",
                table: "Communities",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ArchivedAtUtc",
                table: "Communities",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsArchived",
                table: "Communities",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<double>(
                name: "Latitude",
                table: "Communities",
                type: "REAL",
                precision: 10,
                scale: 7,
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Longitude",
                table: "Communities",
                type: "REAL",
                precision: 10,
                scale: 7,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PopulationReferenceYear",
                table: "Communities",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Communities_IsArchived",
                table: "Communities",
                column: "IsArchived");

            migrationBuilder.CreateIndex(
                name: "IX_Communities_State_District",
                table: "Communities",
                columns: new[] { "State", "District" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Communities_IsArchived",
                table: "Communities");

            migrationBuilder.DropIndex(
                name: "IX_Communities_State_District",
                table: "Communities");

            migrationBuilder.DropColumn(
                name: "ApproximatePopulation",
                table: "Communities");

            migrationBuilder.DropColumn(
                name: "ArchivedAtUtc",
                table: "Communities");

            migrationBuilder.DropColumn(
                name: "IsArchived",
                table: "Communities");

            migrationBuilder.DropColumn(
                name: "Latitude",
                table: "Communities");

            migrationBuilder.DropColumn(
                name: "Longitude",
                table: "Communities");

            migrationBuilder.DropColumn(
                name: "PopulationReferenceYear",
                table: "Communities");
        }
    }
}
