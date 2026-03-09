using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ConnectPlay.TicketPlay.API.Migrations
{
    /// <inheritdoc />
    public partial class AddHallSeats : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_seats_halls_HallId",
                table: "seats");

            migrationBuilder.AlterColumn<int>(
                name: "HallId",
                table: "seats",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_seats_halls_HallId",
                table: "seats",
                column: "HallId",
                principalTable: "halls",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_seats_halls_HallId",
                table: "seats");

            migrationBuilder.AlterColumn<int>(
                name: "HallId",
                table: "seats",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_seats_halls_HallId",
                table: "seats",
                column: "HallId",
                principalTable: "halls",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
