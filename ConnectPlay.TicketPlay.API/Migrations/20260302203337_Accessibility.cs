using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ConnectPlay.TicketPlay.API.Migrations
{
    /// <inheritdoc />
    public partial class Accessibility : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ThreeDProjector",
                table: "halls",
                newName: "Has3DProjector");

            migrationBuilder.AddColumn<bool>(
                name: "IsForWheelchair",
                table: "seats",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "HasBreak",
                table: "screenings",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsForWheelchair",
                table: "seats");

            migrationBuilder.DropColumn(
                name: "HasBreak",
                table: "screenings");

            migrationBuilder.RenameColumn(
                name: "Has3DProjector",
                table: "halls",
                newName: "ThreeDProjector");
        }
    }
}
