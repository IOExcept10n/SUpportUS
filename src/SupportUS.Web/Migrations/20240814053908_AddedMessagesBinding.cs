using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SupportUS.Web.Migrations
{
    /// <inheritdoc />
    public partial class AddedMessagesBinding : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "BotMessageId",
                table: "Quests",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<int>(
                name: "MailMessageId",
                table: "Quests",
                type: "INTEGER",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BotMessageId",
                table: "Quests");

            migrationBuilder.DropColumn(
                name: "MailMessageId",
                table: "Quests");
        }
    }
}
