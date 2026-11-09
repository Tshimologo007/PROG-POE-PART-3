using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RaceCompetitionAPI.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    categoryID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    category_Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    description = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.categoryID);
                });

            migrationBuilder.CreateTable(
                name: "Enrolments",
                columns: table => new
                {
                    enrolmentID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    participantID = table.Column<int>(type: "int", nullable: false),
                    eventCategory = table.Column<int>(type: "int", nullable: false),
                    enrolment_Date = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Enrolments", x => x.enrolmentID);
                });

            migrationBuilder.CreateTable(
                name: "EventCategories",
                columns: table => new
                {
                    eventID = table.Column<int>(type: "int", nullable: false),
                    categoryID = table.Column<int>(type: "int", nullable: false),
                    eventCategoryID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventCategories", x => new { x.eventID, x.categoryID });
                });

            migrationBuilder.CreateTable(
                name: "Events",
                columns: table => new
                {
                    eventID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    organiserID = table.Column<int>(type: "int", nullable: false),
                    townID = table.Column<int>(type: "int", nullable: false),
                    event_Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    event_Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    event_Type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    venue = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    race_Distance = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Events", x => x.eventID);
                });

            migrationBuilder.CreateTable(
                name: "Organisers",
                columns: table => new
                {
                    organiserId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    userID = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    organiser_Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    phone_Number = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    address = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Organisers", x => x.organiserId);
                });

            migrationBuilder.CreateTable(
                name: "Participants",
                columns: table => new
                {
                    participantID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    userID = table.Column<int>(type: "int", nullable: false),
                    name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    surname = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Participants", x => x.participantID);
                });

            migrationBuilder.CreateTable(
                name: "Results",
                columns: table => new
                {
                    resultID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    enrolmentID = table.Column<int>(type: "int", nullable: false),
                    finish_time = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    distance = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    completionStatus = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    position = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Results", x => x.resultID);
                });

            migrationBuilder.CreateTable(
                name: "Routes",
                columns: table => new
                {
                    RoutesID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    eventID = table.Column<int>(type: "int", nullable: false),
                    route_Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    route_Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    mapURL = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Routes", x => x.RoutesID);
                });

            migrationBuilder.CreateTable(
                name: "Towns",
                columns: table => new
                {
                    townID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    town_Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    province = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    postal_code = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Towns", x => x.townID);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    userId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    passwordHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    role = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.userId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Categories");

            migrationBuilder.DropTable(
                name: "Enrolments");

            migrationBuilder.DropTable(
                name: "EventCategories");

            migrationBuilder.DropTable(
                name: "Events");

            migrationBuilder.DropTable(
                name: "Organisers");

            migrationBuilder.DropTable(
                name: "Participants");

            migrationBuilder.DropTable(
                name: "Results");

            migrationBuilder.DropTable(
                name: "Routes");

            migrationBuilder.DropTable(
                name: "Towns");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
