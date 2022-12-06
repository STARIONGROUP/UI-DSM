using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace UI_DSM.Server.Migrations
{
    using System.Diagnostics.CodeAnalysis;

    [ExcludeFromCodeCoverage]
    public partial class ReviewObjectiveCreationMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Title",
                table: "ReviewObjective");

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "ReviewTask",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<int>(
                name: "AdditionalView",
                table: "ReviewTask",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "HasPrimaryView",
                table: "ReviewTask",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "MainView",
                table: "ReviewTask",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "OptionalView",
                table: "ReviewTask",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int[]>(
                name: "RelatedViews",
                table: "ReviewObjective",
                type: "integer[]",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ReviewObjectiveKind",
                table: "ReviewObjective",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ReviewObjectiveKindNumber",
                table: "ReviewObjective",
                type: "integer",
                nullable: false,
                defaultValue: 0)
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "AF8956F8-CA85-4DF2-8CB6-C46D0845B987",
                column: "ConcurrencyStamp",
                value: "23f4084d-3589-4c1e-be85-cb3cb10bd9af");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "F3E3BACF-5F7C-4657-88E9-FA904EFB64D7",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "b8fef53c-f1a5-4bad-b675-b4e357947909", "AQAAAAEAACcQAAAAEHJKhKq/Xx/INLV+6pnMFaUVG6RxYtmgHPuvf1xGnxplhYoLWnEDs0BadIo7C6jV7g==", "f672b5f2-928a-4d0b-b632-d45a627cccf2" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AdditionalView",
                table: "ReviewTask");

            migrationBuilder.DropColumn(
                name: "HasPrimaryView",
                table: "ReviewTask");

            migrationBuilder.DropColumn(
                name: "MainView",
                table: "ReviewTask");

            migrationBuilder.DropColumn(
                name: "OptionalView",
                table: "ReviewTask");

            migrationBuilder.DropColumn(
                name: "RelatedViews",
                table: "ReviewObjective");

            migrationBuilder.DropColumn(
                name: "ReviewObjectiveKind",
                table: "ReviewObjective");

            migrationBuilder.DropColumn(
                name: "ReviewObjectiveKindNumber",
                table: "ReviewObjective");

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "ReviewTask",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "ReviewObjective",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "AF8956F8-CA85-4DF2-8CB6-C46D0845B987",
                column: "ConcurrencyStamp",
                value: "690fd260-c5aa-47af-8b4a-4c722867ec30");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "F3E3BACF-5F7C-4657-88E9-FA904EFB64D7",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "83154a23-6b41-4086-a2bc-7ced60faeb4b", "AQAAAAEAACcQAAAAEB2JFUctB0PDOVktXBxN9wcZaBLdHLH8r/xuWXeDjfmYu7xLXHENbxL+X7JP57axLQ==", "ad474125-f533-466d-bfdd-da202d312928" });
        }
    }
}
