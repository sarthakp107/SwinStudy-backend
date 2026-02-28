using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SwinStudy.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddSurveyToUsers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "degree",
                table: "users",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "has_submitted_survey",
                table: "users",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "semester",
                table: "users",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "user_units",
                columns: table => new
                {
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    unit_id = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_units", x => new { x.user_id, x.unit_id });
                    table.ForeignKey(
                        name: "FK_user_units_all_units_unit_id",
                        column: x => x.unit_id,
                        principalTable: "all_units",
                        principalColumn: "unit_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_user_units_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_user_units_unit_id",
                table: "user_units",
                column: "unit_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "user_units");

            migrationBuilder.DropColumn(
                name: "degree",
                table: "users");

            migrationBuilder.DropColumn(
                name: "has_submitted_survey",
                table: "users");

            migrationBuilder.DropColumn(
                name: "semester",
                table: "users");
        }
    }
}
