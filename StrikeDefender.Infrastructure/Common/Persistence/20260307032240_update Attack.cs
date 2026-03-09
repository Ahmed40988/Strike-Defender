using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StrikeDefender.Infrastructure.Common.Persistence
{
    /// <inheritdoc />
    public partial class updateAttack : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Severity",
                table: "Attacks");

            migrationBuilder.DropColumn(
                name: "Target",
                table: "Attacks");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Severity",
                table: "Attacks",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Target",
                table: "Attacks",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");
        }
    }
}
