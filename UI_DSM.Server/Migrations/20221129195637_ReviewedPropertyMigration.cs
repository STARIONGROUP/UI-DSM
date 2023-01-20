using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UI_DSM.Server.Migrations
{
    using System.Diagnostics.CodeAnalysis;

    [ExcludeFromCodeCoverage]
    public partial class ReviewedPropertyMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsReviewed",
                table: "ReviewItem",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "AF8956F8-CA85-4DF2-8CB6-C46D0845B987",
                column: "ConcurrencyStamp",
                value: "e27240d2-1e8d-4612-b12f-96ed7f923f1a");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "F3E3BACF-5F7C-4657-88E9-FA904EFB64D7",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "d899a983-8022-4b61-b34d-f2b3f7db8c23", "AQAAAAEAACcQAAAAEJB11liz3c4hbnMZS4ZLDrPjZ5t+owBT1KVBLSN717dUSn9AktDB+BpjHpLjURahSg==", "b1d85b12-f138-4ddf-9821-8490eb271746" });

            migrationBuilder.UpdateData(
                table: "Role",
                keyColumn: "Id",
                keyValue: new Guid("28b83519-fb7c-4a9a-8279-194140bfcfbe"),
                column: "AccessRights",
                value: new[] { 0 });

            migrationBuilder.UpdateData(
                table: "Role",
                keyColumn: "Id",
                keyValue: new Guid("fd580a55-9666-4abe-a02b-3a99478996f7"),
                column: "AccessRights",
                value: new[] { 0, 1, 2, 3, 4, 5, 6 });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsReviewed",
                table: "ReviewItem");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "AF8956F8-CA85-4DF2-8CB6-C46D0845B987",
                column: "ConcurrencyStamp",
                value: "932072f3-26ba-4d47-929d-163f2ae5d469");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "F3E3BACF-5F7C-4657-88E9-FA904EFB64D7",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "3549031d-9315-4015-83af-fbe13af47ae5", "AQAAAAEAACcQAAAAEDj2jZAvO/XBOAMvqMq2jz4fL1Px6gRd4dVFqw5R9qGQIt8LY6bj6kHQKaKS7FBL5w==", "ebc97368-4d60-43d1-bf6b-fbcca662c28a" });

            migrationBuilder.UpdateData(
                table: "Role",
                keyColumn: "Id",
                keyValue: new Guid("28b83519-fb7c-4a9a-8279-194140bfcfbe"),
                column: "AccessRights",
                value: new[] { 4 });

            migrationBuilder.UpdateData(
                table: "Role",
                keyColumn: "Id",
                keyValue: new Guid("fd580a55-9666-4abe-a02b-3a99478996f7"),
                column: "AccessRights",
                value: new[] { 0, 1, 2, 3 });
        }
    }
}
