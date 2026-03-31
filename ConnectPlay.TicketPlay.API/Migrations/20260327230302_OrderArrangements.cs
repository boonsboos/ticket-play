using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ConnectPlay.TicketPlay.API.Migrations
{
    /// <inheritdoc />
    public partial class OrderArrangements : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_tickets_arrangements_ArrangementId",
                table: "tickets");

            migrationBuilder.DropIndex(
                name: "IX_tickets_ArrangementId",
                table: "tickets");

            migrationBuilder.DropColumn(
                name: "ArrangementId",
                table: "tickets");

            migrationBuilder.CreateTable(
                name: "order_arrangement",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    OrderId = table.Column<int>(type: "int", nullable: false),
                    ArrangementId = table.Column<int>(type: "int", nullable: false),
                    Amount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_order_arrangement", x => x.Id);
                    table.ForeignKey(
                        name: "FK_order_arrangement_arrangements_ArrangementId",
                        column: x => x.ArrangementId,
                        principalTable: "arrangements",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_order_arrangement_orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_order_arrangement_ArrangementId",
                table: "order_arrangement",
                column: "ArrangementId");

            migrationBuilder.CreateIndex(
                name: "IX_order_arrangement_OrderId",
                table: "order_arrangement",
                column: "OrderId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "order_arrangement");

            migrationBuilder.AddColumn<int>(
                name: "ArrangementId",
                table: "tickets",
                type: "int",
                nullable: true);

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
    }
}
