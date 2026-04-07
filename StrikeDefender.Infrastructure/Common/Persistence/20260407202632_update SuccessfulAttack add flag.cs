using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StrikeDefender.Infrastructure.Common.Persistence
{
    /// <inheritdoc />
    public partial class updateSuccessfulAttackaddflag : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsBlocked",
                table: "SuccessfulAttacks",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsBlocked",
                table: "SuccessfulAttacks");
        }
    }
}
