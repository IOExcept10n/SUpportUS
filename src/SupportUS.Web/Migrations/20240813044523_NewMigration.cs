using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SupportUS.Web.Migrations
{
    /// <inheritdoc />
    public partial class NewMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Quests_Reviews_CustomerReviewId",
                table: "Quests");

            migrationBuilder.DropForeignKey(
                name: "FK_Quests_Reviews_ExecutorReviewId",
                table: "Quests");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "Profiles");

            migrationBuilder.AlterColumn<long>(
                name: "AuthorId",
                table: "Reviews",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "TEXT");

            migrationBuilder.AddColumn<Guid>(
                name: "QuestId",
                table: "Reviews",
                type: "TEXT",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<int>(
                name: "Rating",
                table: "Reviews",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<Guid>(
                name: "ExecutorReviewId",
                table: "Quests",
                type: "TEXT",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "ExecutorId",
                table: "Quests",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "CustomerReviewId",
                table: "Quests",
                type: "TEXT",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "CustomerId",
                table: "Quests",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "Profiles",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "TEXT")
                .Annotation("Sqlite:Autoincrement", true);

            migrationBuilder.AddForeignKey(
                name: "FK_Quests_Reviews_CustomerReviewId",
                table: "Quests",
                column: "CustomerReviewId",
                principalTable: "Reviews",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Quests_Reviews_ExecutorReviewId",
                table: "Quests",
                column: "ExecutorReviewId",
                principalTable: "Reviews",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Quests_Reviews_CustomerReviewId",
                table: "Quests");

            migrationBuilder.DropForeignKey(
                name: "FK_Quests_Reviews_ExecutorReviewId",
                table: "Quests");

            migrationBuilder.DropColumn(
                name: "QuestId",
                table: "Reviews");

            migrationBuilder.DropColumn(
                name: "Rating",
                table: "Reviews");

            migrationBuilder.AlterColumn<Guid>(
                name: "AuthorId",
                table: "Reviews",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "INTEGER");

            migrationBuilder.AlterColumn<Guid>(
                name: "ExecutorReviewId",
                table: "Quests",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<Guid>(
                name: "ExecutorId",
                table: "Quests",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "CustomerReviewId",
                table: "Quests",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<Guid>(
                name: "CustomerId",
                table: "Quests",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "INTEGER");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "Profiles",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "INTEGER")
                .OldAnnotation("Sqlite:Autoincrement", true);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Profiles",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddForeignKey(
                name: "FK_Quests_Reviews_CustomerReviewId",
                table: "Quests",
                column: "CustomerReviewId",
                principalTable: "Reviews",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Quests_Reviews_ExecutorReviewId",
                table: "Quests",
                column: "ExecutorReviewId",
                principalTable: "Reviews",
                principalColumn: "Id");
        }
    }
}
