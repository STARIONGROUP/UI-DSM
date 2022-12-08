using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UI_DSM.Server.Migrations
{
    public partial class TaskAssignedToMultipleParticipants : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ReviewTask_Participant_IsAssignedToId",
                table: "ReviewTask");

            migrationBuilder.DropIndex(
                name: "IX_ReviewTask_IsAssignedToId",
                table: "ReviewTask");

            migrationBuilder.DropColumn(
                name: "IsAssignedToId",
                table: "ReviewTask");

            migrationBuilder.CreateTable(
                name: "ParticipantReviewTask",
                columns: table => new
                {
                    AssignedTasksId = table.Column<Guid>(type: "uuid", nullable: false),
                    IsAssignedToId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ParticipantReviewTask", x => new { x.AssignedTasksId, x.IsAssignedToId });
                    table.ForeignKey(
                        name: "FK_ParticipantReviewTask_Participant_IsAssignedToId",
                        column: x => x.IsAssignedToId,
                        principalTable: "Participant",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ParticipantReviewTask_ReviewTask_AssignedTasksId",
                        column: x => x.AssignedTasksId,
                        principalTable: "ReviewTask",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

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

            migrationBuilder.CreateIndex(
                name: "IX_ParticipantReviewTask_IsAssignedToId",
                table: "ParticipantReviewTask",
                column: "IsAssignedToId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ParticipantReviewTask");

            migrationBuilder.AddColumn<Guid>(
                name: "IsAssignedToId",
                table: "ReviewTask",
                type: "uuid",
                nullable: true);

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

            migrationBuilder.CreateIndex(
                name: "IX_ReviewTask_IsAssignedToId",
                table: "ReviewTask",
                column: "IsAssignedToId");

            migrationBuilder.AddForeignKey(
                name: "FK_ReviewTask_Participant_IsAssignedToId",
                table: "ReviewTask",
                column: "IsAssignedToId",
                principalTable: "Participant",
                principalColumn: "Id");
        }
    }
}
