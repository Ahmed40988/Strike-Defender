using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StrikeDefender.Infrastructure.Common.Persistence
{
    /// <inheritdoc />
    public partial class UpdateAttackresult : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Severity",
                table: "SuccessfulAttacks",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Target",
                table: "SuccessfulAttacks",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Severity",
                table: "SuccessfulAttacks");

            migrationBuilder.DropColumn(
                name: "Target",
                table: "SuccessfulAttacks");
        }
    }
}
