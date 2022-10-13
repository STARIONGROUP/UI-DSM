using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UI_DSM.Server.Migrations
{
    using System.Diagnostics.CodeAnalysis;

    [ExcludeFromCodeCoverage]
    public partial class ModelIterationIdMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "IterationId",
                table: "Model",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IterationId",
                table: "Model");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "AF8956F8-CA85-4DF2-8CB6-C46D0845B987",
                column: "ConcurrencyStamp",
                value: "4bf9bc78-c9e5-4037-b979-b8a3589e1ae3");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "F3E3BACF-5F7C-4657-88E9-FA904EFB64D7",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "52ff4961-e8ed-4a44-bf47-23454610c0b1", "AQAAAAEAACcQAAAAEFzoNJqGOaN5FvrKG5vCeK7tFutoBmo8EjOc9sak+MTLs5TIjMk6EOJqHV/SdO/USA==", "c3c2724a-d578-4e1b-98df-c0c38c676f32" });
        }
    }
}
