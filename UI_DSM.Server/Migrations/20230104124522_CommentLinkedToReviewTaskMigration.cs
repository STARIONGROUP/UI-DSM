using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UI_DSM.Server.Migrations
{
    using System.Diagnostics.CodeAnalysis;

    [ExcludeFromCodeCoverage]
    public partial class CommentLinkedToReviewTaskMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "CreatedInsideId",
                table: "Comment",
                type: "uuid",
                nullable: true);

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

            migrationBuilder.CreateIndex(
                name: "IX_Comment_CreatedInsideId",
                table: "Comment",
                column: "CreatedInsideId");

            migrationBuilder.AddForeignKey(
                name: "FK_Comment_ReviewTask_CreatedInsideId",
                table: "Comment",
                column: "CreatedInsideId",
                principalTable: "ReviewTask",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comment_ReviewTask_CreatedInsideId",
                table: "Comment");

            migrationBuilder.DropIndex(
                name: "IX_Comment_CreatedInsideId",
                table: "Comment");

            migrationBuilder.DropColumn(
                name: "CreatedInsideId",
                table: "Comment");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "AF8956F8-CA85-4DF2-8CB6-C46D0845B987",
                column: "ConcurrencyStamp",
                value: "3fdd6e22-d976-4865-b3f0-24efd6b6bbba");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "F3E3BACF-5F7C-4657-88E9-FA904EFB64D7",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "d67afe67-56f7-471c-860d-3b020cb20e3d", "AQAAAAEAACcQAAAAELeD5p3ywKCB3F/GaTgrIhjHyjFdR9X5n4csTRZZgBmtHpDqh80k7+PivIKrQDUvLg==", "ffe23266-8369-4dff-b82a-6d6267c2dff3" });
        }
    }
}
