using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UI_DSM.Server.Migrations
{
    using System.Diagnostics.CodeAnalysis;

    [ExcludeFromCodeCoverage]
    public partial class ProjectPropertiesAddition : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedOn",
                table: "Project",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedOn",
                table: "Project");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "AF8956F8-CA85-4DF2-8CB6-C46D0845B987",
                column: "ConcurrencyStamp",
                value: "bdaed18c-5e74-46bd-8536-c1255071fd03");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "F3E3BACF-5F7C-4657-88E9-FA904EFB64D7",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "3f1e53a3-5e68-4514-b53a-5f2afba551f1", "AQAAAAEAACcQAAAAEDr2GVdqM38+1tymdhti7jnMdCMq/DgMJicqe7P1sRgZ50asePWsDN40KjHaC59odw==", "facbc854-1ec0-4c4d-8a08-0551243724d4" });
        }
    }
}
