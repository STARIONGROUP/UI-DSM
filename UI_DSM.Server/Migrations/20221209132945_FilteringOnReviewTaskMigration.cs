using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UI_DSM.Server.Migrations
{
    [ExcludeFromCodeCoverage]
    public partial class FilteringOnReviewTaskMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<List<string>>(
                name: "Prefilters",
                table: "ReviewTask",
                type: "text[]",
                nullable: true);

            migrationBuilder.AddColumn<List<string>>(
                name: "AdditionnalColumnsVisibleAtStart",
                table: "ReviewObjective",
                type: "text[]",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "AF8956F8-CA85-4DF2-8CB6-C46D0845B987",
                column: "ConcurrencyStamp",
                value: "0d8bf40b-45cb-4cfd-a1ce-dbdbf88219e8");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "F3E3BACF-5F7C-4657-88E9-FA904EFB64D7",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "ff096a47-62f8-455b-b627-b23d297597a8", "AQAAAAEAACcQAAAAEM4a/HF5EWVer5aU1tG9ZKgwwwKQ+UFo/MFu5ILvB3VULQIdKEuOr92vFa4PuGxnoQ==", "2ae248b3-7968-479a-9edc-c4fa8b322d38" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Prefilters",
                table: "ReviewTask");

            migrationBuilder.DropColumn(
                name: "AdditionnalColumnsVisibleAtStart",
                table: "ReviewObjective");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "AF8956F8-CA85-4DF2-8CB6-C46D0845B987",
                column: "ConcurrencyStamp",
                value: "f823ead5-321c-4591-a049-991a16afcfc4");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "F3E3BACF-5F7C-4657-88E9-FA904EFB64D7",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "2df769b2-ed52-4d72-86aa-720c69d7a74f", "AQAAAAEAACcQAAAAEB8AU6SiarJ79qlyE/BIQvzcul0tLA3wSdcE8exvVUX4/GF0aumdHajRm56o5HI/gg==", "f7e6f88c-b004-495c-bf3c-032cca3eefad" });
        }
    }
}
