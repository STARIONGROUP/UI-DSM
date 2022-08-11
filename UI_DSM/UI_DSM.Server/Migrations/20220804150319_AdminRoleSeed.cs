using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UI_DSM.Server.Migrations
{
    using System.Diagnostics.CodeAnalysis;

    [ExcludeFromCodeCoverage]
    public partial class AdminRoleSeed : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "AF8956F8-CA85-4DF2-8CB6-C46D0845B987", "fe7b8d25-16f8-47c2-9100-5c064cff2393", "Administrator", "ADMINISTRATOR" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "F3E3BACF-5F7C-4657-88E9-FA904EFB64D7",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "e09699ab-a903-43e2-ac73-b316fd71d5cf", "AQAAAAEAACcQAAAAEAeysVODCLqjegWO2sUPtcq178eAqy196QWmkMRpAnsBzSEltZI1XVk1D2BT0YgmJQ==", "acc66fb4-5feb-4fce-9232-dbf8125be7b9" });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { "AF8956F8-CA85-4DF2-8CB6-C46D0845B987", "F3E3BACF-5F7C-4657-88E9-FA904EFB64D7" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "AF8956F8-CA85-4DF2-8CB6-C46D0845B987", "F3E3BACF-5F7C-4657-88E9-FA904EFB64D7" });

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "AF8956F8-CA85-4DF2-8CB6-C46D0845B987");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "F3E3BACF-5F7C-4657-88E9-FA904EFB64D7",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "6a3d2d13-515a-4f23-99db-2db886289644", "AQAAAAEAACcQAAAAEGU3W5f+n3Rm/8tR1/E4wM5gtRQqdbJ+9ih4sIuYReTNbRMibL+YNq9U039HhFJVpg==", "8abfac94-8bde-445c-8cd1-7766f9277389" });
        }
    }
}
