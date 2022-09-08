using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace UI_DSM.Server.Migrations
{
    using System.Diagnostics.CodeAnalysis;

    [ExcludeFromCodeCoverage]
    public partial class ReviewAndReviewObjectiveMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Participant_Project_EntityContainerId",
                table: "Participant");

            migrationBuilder.CreateTable(
                name: "Review",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    AuthorId = table.Column<Guid>(type: "uuid", nullable: true),
                    ReviewNumber = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Title = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    EntityContainerId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Review", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Review_Entity_Id",
                        column: x => x.Id,
                        principalTable: "Entity",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Review_Participant_AuthorId",
                        column: x => x.AuthorId,
                        principalTable: "Participant",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Review_Project_EntityContainerId",
                        column: x => x.EntityContainerId,
                        principalTable: "Project",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ReviewObjective",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    ReviewObjectiveNumber = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    EntityContainerId = table.Column<Guid>(type: "uuid", nullable: true),
                    AuthorId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReviewObjective", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReviewObjective_Entity_Id",
                        column: x => x.Id,
                        principalTable: "Entity",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ReviewObjective_Participant_AuthorId",
                        column: x => x.AuthorId,
                        principalTable: "Participant",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ReviewObjective_Review_EntityContainerId",
                        column: x => x.EntityContainerId,
                        principalTable: "Review",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "AF8956F8-CA85-4DF2-8CB6-C46D0845B987",
                column: "ConcurrencyStamp",
                value: "c1a9421c-4176-4acb-b473-ebc03780f807");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "F3E3BACF-5F7C-4657-88E9-FA904EFB64D7",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "172b3dd7-4ed4-4b2f-be64-206637a544be", "AQAAAAEAACcQAAAAEA8NqgZEzQJQBXCNn8kdGlZ+LCKDV9ScuiZIXaJJXZ/Eo5lIHa3BLffcJk/tAPBv5Q==", "662edb1d-3e9c-4bdf-91de-0a9050e42dfa" });

            migrationBuilder.CreateIndex(
                name: "IX_Review_AuthorId",
                table: "Review",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_Review_EntityContainerId",
                table: "Review",
                column: "EntityContainerId");

            migrationBuilder.CreateIndex(
                name: "IX_ReviewObjective_AuthorId",
                table: "ReviewObjective",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_ReviewObjective_EntityContainerId",
                table: "ReviewObjective",
                column: "EntityContainerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Participant_Project_EntityContainerId",
                table: "Participant",
                column: "EntityContainerId",
                principalTable: "Project",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Participant_Project_EntityContainerId",
                table: "Participant");

            migrationBuilder.DropTable(
                name: "ReviewObjective");

            migrationBuilder.DropTable(
                name: "Review");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "AF8956F8-CA85-4DF2-8CB6-C46D0845B987",
                column: "ConcurrencyStamp",
                value: "798efa39-ded8-429f-b4df-d4e1d8d82145");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "F3E3BACF-5F7C-4657-88E9-FA904EFB64D7",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "e5c0cf72-9d1f-46a8-b853-51b72e127f31", "AQAAAAEAACcQAAAAEMlzq1qsJpsEi3f5Ky+93vFgnWS+2E4PIAarvvbBnxSy2/FshBWRrVgPH9ZINOLu6w==", "785b2ce0-7ae3-48a8-a4a3-21ac0da08fde" });

            migrationBuilder.AddForeignKey(
                name: "FK_Participant_Project_EntityContainerId",
                table: "Participant",
                column: "EntityContainerId",
                principalTable: "Project",
                principalColumn: "Id");
        }
    }
}
