using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StrikeDefender.Infrastructure.Common.Persistence
{
    /// <inheritdoc />
    public partial class updateWAFRule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "Rules");

            migrationBuilder.AddColumn<Guid>(
                name: "ParsedDataId",
                table: "Rules",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ParsedWafRule",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Target = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Operator = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Pattern = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RuleId = table.Column<int>(type: "int", nullable: false),
                    Phase = table.Column<int>(type: "int", nullable: false),
                    Action = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StatusCode = table.Column<int>(type: "int", nullable: false),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ParsedWafRule", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Rules_ParsedDataId",
                table: "Rules",
                column: "ParsedDataId");

            migrationBuilder.AddForeignKey(
                name: "FK_Rules_ParsedWafRule_ParsedDataId",
                table: "Rules",
                column: "ParsedDataId",
                principalTable: "ParsedWafRule",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Rules_ParsedWafRule_ParsedDataId",
                table: "Rules");

            migrationBuilder.DropTable(
                name: "ParsedWafRule");

            migrationBuilder.DropIndex(
                name: "IX_Rules_ParsedDataId",
                table: "Rules");

            migrationBuilder.DropColumn(
                name: "ParsedDataId",
                table: "Rules");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Rules",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: false,
                defaultValue: "");
        }
    }
}
