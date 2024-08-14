using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SupportUS.Web.Migrations
{
    /// <inheritdoc />
    public partial class AddedDraft : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "CurrentDraftQuest",
                table: "Profiles",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CurrentDraftQuest",
                table: "Profiles");
        }
    }
}
