using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RaceCompetitionAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddRegistrations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "eventCategory",
                table: "Enrolments",
                newName: "eventCategoryID");

            migrationBuilder.CreateTable(
                name: "Registrations",
                columns: table => new
                {
                    registrationID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    userID = table.Column<int>(type: "int", nullable: false),
                    eventID = table.Column<int>(type: "int", nullable: false),
                    registration_Date = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Registrations", x => x.registrationID);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Registrations");

            migrationBuilder.RenameColumn(
                name: "eventCategoryID",
                table: "Enrolments",
                newName: "eventCategory");
        }
    }
}
