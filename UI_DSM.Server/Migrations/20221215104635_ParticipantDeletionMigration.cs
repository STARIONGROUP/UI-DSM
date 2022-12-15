using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UI_DSM.Server.Migrations
{
    [ExcludeFromCodeCoverage]
    public partial class ParticipantDeletionMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Annotation_Participant_AuthorId",
                table: "Annotation");

            migrationBuilder.DropForeignKey(
                name: "FK_Reply_Participant_AuthorId",
                table: "Reply");

            migrationBuilder.DropForeignKey(
                name: "FK_Review_Participant_AuthorId",
                table: "Review");

            migrationBuilder.DropForeignKey(
                name: "FK_ReviewObjective_Participant_AuthorId",
                table: "ReviewObjective");

            migrationBuilder.DropForeignKey(
                name: "FK_ReviewTask_Participant_AuthorId",
                table: "ReviewTask");

            migrationBuilder.DropIndex(
                name: "IX_ReviewTask_AuthorId",
                table: "ReviewTask");

            migrationBuilder.DropIndex(
                name: "IX_ReviewObjective_AuthorId",
                table: "ReviewObjective");

            migrationBuilder.DropIndex(
                name: "IX_Review_AuthorId",
                table: "Review");

            migrationBuilder.DropColumn(
                name: "AuthorId",
                table: "ReviewTask");

            migrationBuilder.DropColumn(
                name: "AuthorId",
                table: "ReviewObjective");

            migrationBuilder.DropColumn(
                name: "AuthorId",
                table: "Review");

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

            migrationBuilder.AddForeignKey(
                name: "FK_Annotation_Participant_AuthorId",
                table: "Annotation",
                column: "AuthorId",
                principalTable: "Participant",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Reply_Participant_AuthorId",
                table: "Reply",
                column: "AuthorId",
                principalTable: "Participant",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Annotation_Participant_AuthorId",
                table: "Annotation");

            migrationBuilder.DropForeignKey(
                name: "FK_Reply_Participant_AuthorId",
                table: "Reply");

            migrationBuilder.AddColumn<Guid>(
                name: "AuthorId",
                table: "ReviewTask",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "AuthorId",
                table: "ReviewObjective",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "AuthorId",
                table: "Review",
                type: "uuid",
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

            migrationBuilder.CreateIndex(
                name: "IX_ReviewTask_AuthorId",
                table: "ReviewTask",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_ReviewObjective_AuthorId",
                table: "ReviewObjective",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_Review_AuthorId",
                table: "Review",
                column: "AuthorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Annotation_Participant_AuthorId",
                table: "Annotation",
                column: "AuthorId",
                principalTable: "Participant",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Reply_Participant_AuthorId",
                table: "Reply",
                column: "AuthorId",
                principalTable: "Participant",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Review_Participant_AuthorId",
                table: "Review",
                column: "AuthorId",
                principalTable: "Participant",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ReviewObjective_Participant_AuthorId",
                table: "ReviewObjective",
                column: "AuthorId",
                principalTable: "Participant",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ReviewTask_Participant_AuthorId",
                table: "ReviewTask",
                column: "AuthorId",
                principalTable: "Participant",
                principalColumn: "Id");
        }
    }
}
