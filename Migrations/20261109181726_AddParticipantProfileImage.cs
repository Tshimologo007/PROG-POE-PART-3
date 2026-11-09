using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RaceCompetitionAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddParticipantProfileImage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "profileImage",
                table: "Participants",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "profileImage",
                table: "Participants");
        }
    }
}
