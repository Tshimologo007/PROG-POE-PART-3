using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RaceCompetitionAPI.Migrations
{
    /// <inheritdoc />
    public partial class FixEventCategoryPrimaryKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_EventCategories",
                table: "EventCategories");

            // Remove the old eventCategoryID column
            migrationBuilder.DropColumn(
                name: "eventCategoryID",
                table: "EventCategories");

            // Recreate it as an automatically generated identity column
            migrationBuilder.AddColumn<int>(
                name: "eventCategoryID",
                table: "EventCategories",
                type: "int",
                nullable: false)
                .Annotation("SqlServer:Identity", "1, 1");

            // Make eventCategoryID the primary key
            migrationBuilder.AddPrimaryKey(
                name: "PK_EventCategories",
                table: "EventCategories",
                column: "eventCategoryID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_EventCategories",
                table: "EventCategories");

            migrationBuilder.DropColumn(
                name: "eventCategoryID",
                table: "EventCategories");

            migrationBuilder.AddColumn<int>(
                name: "eventCategoryID",
                table: "EventCategories",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_EventCategories",
                table: "EventCategories",
                columns: new[] { "eventID", "categoryID" });
        }
    }
}
