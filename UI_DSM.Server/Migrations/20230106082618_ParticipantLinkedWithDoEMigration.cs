using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UI_DSM.Server.Migrations
{
    using System.Diagnostics.CodeAnalysis;

    [ExcludeFromCodeCoverage]
    public partial class ParticipantLinkedWithDoEMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<List<string>>(
                name: "DomainsOfExpertise",
                table: "Participant",
                type: "text[]",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "AF8956F8-CA85-4DF2-8CB6-C46D0845B987",
                column: "ConcurrencyStamp",
                value: "962149c6-daf6-4579-9eaa-bcc459c8ab15");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "F3E3BACF-5F7C-4657-88E9-FA904EFB64D7",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "bc2d4a7e-1022-429e-8d3f-bb9d3648dff2", "AQAAAAEAACcQAAAAEC6sx0s+DGKiAmG0pOKCsI4fLeUVVj9R/G4bGL8RfmFELE1gPKu8IjGldVDHv4g0Og==", "3e0c291f-3a22-4193-8e56-23b8545080d7" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DomainsOfExpertise",
                table: "Participant");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "AF8956F8-CA85-4DF2-8CB6-C46D0845B987",
                column: "ConcurrencyStamp",
                value: "6e50fa06-f019-4cad-8d14-bfe776161785");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "F3E3BACF-5F7C-4657-88E9-FA904EFB64D7",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "2fe79dc3-cfd8-49ca-8ed1-2026cf196392", "AQAAAAEAACcQAAAAEE4p3uoxsgacU7AhV5x9Oa6PDI/ZvAXXjJby92MTj6uVRK/18xrl64UONWSo+aPAcw==", "ea770462-5ca5-4544-9723-77a0a2bae996" });
        }
    }
}
