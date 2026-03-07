using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ConnectPlay.TicketPlay.API.Migrations
{
    /// <inheritdoc />
    public partial class UniqueKeys : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "movies",
                type: "varchar(255)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "UQ_movies_Title",
                table: "movies",
                column: "Title",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UQ_halls_HallNumber",
                table: "halls",
                column: "HallNumber",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "UQ_movies_Title",
                table: "movies");

            migrationBuilder.DropIndex(
                name: "UQ_halls_HallNumber",
                table: "halls");

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "movies",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(255)")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");
        }
    }
}
