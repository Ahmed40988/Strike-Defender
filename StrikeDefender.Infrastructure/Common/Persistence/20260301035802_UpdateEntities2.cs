using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StrikeDefender.Infrastructure.Common.Persistence
{
    /// <inheritdoc />
    public partial class UpdateEntities2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AvailableAttacks",
                table: "Subscriptions");

            migrationBuilder.DropColumn(
                name: "AvailableRules",
                table: "Subscriptions");

            migrationBuilder.AddColumn<int>(
                name: "MaxRiskScoreAccess",
                table: "Plans",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MaxRiskScoreAccess",
                table: "Plans");

            migrationBuilder.AddColumn<int>(
                name: "AvailableAttacks",
                table: "Subscriptions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "AvailableRules",
                table: "Subscriptions",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
