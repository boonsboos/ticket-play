using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ConnectPlay.TicketPlay.API.Migrations
{
    /// <inheritdoc />
    public partial class Arrangements : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ArrangementId",
                table: "tickets",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "arrangements",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Price = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                    Name = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Type = table.Column<byte>(type: "tinyint unsigned", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_arrangements", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_tickets_ArrangementId",
                table: "tickets",
                column: "ArrangementId");

            migrationBuilder.AddForeignKey(
                name: "FK_tickets_arrangements_ArrangementId",
                table: "tickets",
                column: "ArrangementId",
                principalTable: "arrangements",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_tickets_arrangements_ArrangementId",
                table: "tickets");

            migrationBuilder.DropTable(
                name: "arrangements");

            migrationBuilder.DropIndex(
                name: "IX_tickets_ArrangementId",
                table: "tickets");

            migrationBuilder.DropColumn(
                name: "ArrangementId",
                table: "tickets");
        }
    }
}
