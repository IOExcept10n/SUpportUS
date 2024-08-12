using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SupportUS.Web.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Profiles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Coins = table.Column<int>(type: "INTEGER", nullable: false),
                    Rating = table.Column<double>(type: "REAL", nullable: false),
                    Status = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Profiles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Tickets",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Content = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tickets", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Reviews",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    AuthorId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Content = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reviews", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Reviews_Profiles_AuthorId",
                        column: x => x.AuthorId,
                        principalTable: "Profiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Quests",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    CustomerId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: false),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    Price = table.Column<int>(type: "INTEGER", nullable: false),
                    Location = table.Column<string>(type: "TEXT", nullable: false),
                    ExpectedDuration = table.Column<TimeSpan>(type: "TEXT", nullable: true),
                    Deadline = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreationDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ExecutorReviewId = table.Column<Guid>(type: "TEXT", nullable: true),
                    CustomerReviewId = table.Column<Guid>(type: "TEXT", nullable: true),
                    ExecutorId = table.Column<Guid>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Quests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Quests_Profiles_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Profiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Quests_Profiles_ExecutorId",
                        column: x => x.ExecutorId,
                        principalTable: "Profiles",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Quests_Reviews_CustomerReviewId",
                        column: x => x.CustomerReviewId,
                        principalTable: "Reviews",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Quests_Reviews_ExecutorReviewId",
                        column: x => x.ExecutorReviewId,
                        principalTable: "Reviews",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Quests_CustomerId",
                table: "Quests",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Quests_CustomerReviewId",
                table: "Quests",
                column: "CustomerReviewId");

            migrationBuilder.CreateIndex(
                name: "IX_Quests_ExecutorId",
                table: "Quests",
                column: "ExecutorId");

            migrationBuilder.CreateIndex(
                name: "IX_Quests_ExecutorReviewId",
                table: "Quests",
                column: "ExecutorReviewId");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_AuthorId",
                table: "Reviews",
                column: "AuthorId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Quests");

            migrationBuilder.DropTable(
                name: "Tickets");

            migrationBuilder.DropTable(
                name: "Reviews");

            migrationBuilder.DropTable(
                name: "Profiles");
        }
    }
}
