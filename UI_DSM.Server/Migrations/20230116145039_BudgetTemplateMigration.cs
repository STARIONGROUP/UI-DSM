using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UI_DSM.Server.Migrations
{
    using System.Diagnostics.CodeAnalysis;

    [ExcludeFromCodeCoverage]
    public partial class BudgetTemplateMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BudgetTemplate",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BudgetName = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BudgetTemplate", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BudgetTemplate_Artifact_Id",
                        column: x => x.Id,
                        principalTable: "Artifact",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "AF8956F8-CA85-4DF2-8CB6-C46D0845B987",
                column: "ConcurrencyStamp",
                value: "99684f7c-56eb-4af2-af93-e8b1989e0329");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "F3E3BACF-5F7C-4657-88E9-FA904EFB64D7",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "3f5f0555-3222-42a0-8ba5-41b60c4e6b13", "AQAAAAEAACcQAAAAECOCiOPNzOiiehFXJT4BOKPEW9WekI7BcoTTfniYiPhO//jn5TLbpMaM8two5v1LoA==", "9a658aab-0d0c-4873-ad8b-e083a5403adb" });

            migrationBuilder.UpdateData(
                table: "Role",
                keyColumn: "Id",
                keyValue: new Guid("fd580a55-9666-4abe-a02b-3a99478996f7"),
                column: "AccessRights",
                value: new[] { 0, 1, 2, 3, 4, 5, 6, 7 });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BudgetTemplate");

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

            migrationBuilder.UpdateData(
                table: "Role",
                keyColumn: "Id",
                keyValue: new Guid("fd580a55-9666-4abe-a02b-3a99478996f7"),
                column: "AccessRights",
                value: new[] { 0, 1, 2, 3, 4, 5, 6 });
        }
    }
}
