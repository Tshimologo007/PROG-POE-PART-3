using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RaceCompetitionAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddEventBannerImage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "bannerImage",
                table: "Events",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "bannerImage",
                table: "Events");
        }
    }
}
