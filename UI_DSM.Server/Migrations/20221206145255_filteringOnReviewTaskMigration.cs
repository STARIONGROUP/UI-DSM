using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UI_DSM.Server.Migrations
{
    using System.Diagnostics.CodeAnalysis;

    [ExcludeFromCodeCoverage]
    public partial class filteringOnReviewTaskMigration : Migration
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
                value: "b0844859-6d3d-4b7b-ad79-3e92e4b6c107");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "F3E3BACF-5F7C-4657-88E9-FA904EFB64D7",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "2a9354bc-8f17-400d-adb8-08d97cb919eb", "AQAAAAEAACcQAAAAENG9gEaphBwUS55wBSSN7cw3TQEmN51Shj5xripByj8ROLxGRBPDlXFZRWZjjTaOnw==", "ab9e44d4-89e1-4c62-8014-2fe78423aded" });
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
                value: "e27240d2-1e8d-4612-b12f-96ed7f923f1a");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "F3E3BACF-5F7C-4657-88E9-FA904EFB64D7",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "d899a983-8022-4b61-b34d-f2b3f7db8c23", "AQAAAAEAACcQAAAAEJB11liz3c4hbnMZS4ZLDrPjZ5t+owBT1KVBLSN717dUSn9AktDB+BpjHpLjURahSg==", "b1d85b12-f138-4ddf-9821-8490eb271746" });
        }
    }
}
