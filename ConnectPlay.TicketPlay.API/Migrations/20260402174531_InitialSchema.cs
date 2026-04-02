using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ConnectPlay.TicketPlay.API.Migrations
{
    /// <inheritdoc />
    public partial class InitialSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

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
                    table.UniqueConstraint("UQ_arrangements_Name", x => x.Name);
                    table.PrimaryKey("PK_arrangements", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "halls",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    HallNumber = table.Column<int>(type: "int", nullable: false),
                    Capacity = table.Column<int>(type: "int", nullable: false),
                    WheelchairAccessible = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    Has3DProjector = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.UniqueConstraint("UQ_halls_HallNumber", x => x.HallNumber);
                    table.PrimaryKey("PK_halls", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "movies",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Title = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Description = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DescriptionEn = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Duration = table.Column<int>(type: "int", nullable: false),
                    ReleaseDate = table.Column<DateOnly>(type: "date", nullable: false),
                    PosterUrl = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Language = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    MinimumAge = table.Column<int>(type: "int", nullable: false),
                    Genre = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Tags = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.UniqueConstraint("UQ_movies_Title", x => x.Title);
                    table.PrimaryKey("PK_movies", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "newsletter_subscribers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Email = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Name = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.UniqueConstraint("UQ_newsletter_subscribers_Email", x => x.Email);
                    table.PrimaryKey("PK_newsletter_subscribers", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "orders",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Status = table.Column<byte>(type: "tinyint unsigned", nullable: false),
                    Total = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                    OrderCode = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_orders", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "seats",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    HallId = table.Column<int>(type: "int", nullable: true),
                    Row = table.Column<int>(type: "int", nullable: false),
                    SeatNumber = table.Column<int>(type: "int", nullable: false),
                    IsForWheelchair = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_seats", x => x.Id);
                    table.ForeignKey(
                        name: "FK_seats_halls_HallId",
                        column: x => x.HallId,
                        principalTable: "halls",
                        principalColumn: "Id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "screenings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    MovieId = table.Column<int>(type: "int", nullable: false),
                    HallId = table.Column<int>(type: "int", nullable: false),
                    HasBreak = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    StartTime = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false),
                    SneakPreview = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.UniqueConstraint("UQ_screenings_HallId_StartTime", x => new { x.HallId, x.StartTime });
                    table.PrimaryKey("PK_screenings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_screenings_halls_HallId",
                        column: x => x.HallId,
                        principalTable: "halls",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_screenings_movies_MovieId",
                        column: x => x.MovieId,
                        principalTable: "movies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

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

            migrationBuilder.CreateTable(
                name: "tickets",
                columns: table => new
                {
                    TicketId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    ScreeningId = table.Column<int>(type: "int", nullable: false),
                    SeatId = table.Column<int>(type: "int", nullable: false),
                    TicketType = table.Column<byte>(type: "tinyint unsigned", nullable: false),
                    OrderId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tickets", x => x.TicketId);
                    table.ForeignKey(
                        name: "FK_tickets_orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "orders",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_tickets_screenings_ScreeningId",
                        column: x => x.ScreeningId,
                        principalTable: "screenings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_tickets_seats_SeatId",
                        column: x => x.SeatId,
                        principalTable: "seats",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_halls_HallNumber",
                table: "halls",
                column: "HallNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_movies_Title",
                table: "movies",
                column: "Title",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_order_arrangement_ArrangementId",
                table: "order_arrangement",
                column: "ArrangementId");

            migrationBuilder.CreateIndex(
                name: "IX_order_arrangement_OrderId",
                table: "order_arrangement",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_screenings_HallId",
                table: "screenings",
                column: "HallId");

            migrationBuilder.CreateIndex(
                name: "IX_screenings_MovieId",
                table: "screenings",
                column: "MovieId");

            migrationBuilder.CreateIndex(
                name: "IX_seats_HallId",
                table: "seats",
                column: "HallId");

            migrationBuilder.CreateIndex(
                name: "IX_tickets_OrderId",
                table: "tickets",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_tickets_ScreeningId",
                table: "tickets",
                column: "ScreeningId");

            migrationBuilder.CreateIndex(
                name: "IX_tickets_SeatId",
                table: "tickets",
                column: "SeatId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "newsletter_subscribers");

            migrationBuilder.DropTable(
                name: "order_arrangement");

            migrationBuilder.DropTable(
                name: "tickets");

            migrationBuilder.DropTable(
                name: "arrangements");

            migrationBuilder.DropTable(
                name: "orders");

            migrationBuilder.DropTable(
                name: "screenings");

            migrationBuilder.DropTable(
                name: "seats");

            migrationBuilder.DropTable(
                name: "movies");

            migrationBuilder.DropTable(
                name: "halls");
        }
    }
}
