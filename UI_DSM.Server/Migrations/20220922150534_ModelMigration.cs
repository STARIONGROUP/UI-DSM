using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UI_DSM.Server.Migrations
{
    using System.Diagnostics.CodeAnalysis;

    [ExcludeFromCodeCoverage]
    public partial class ModelMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Artifact",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    FileName = table.Column<string>(type: "text", nullable: true),
                    EntityContainerId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Artifact", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Artifact_Entity_Id",
                        column: x => x.Id,
                        principalTable: "Entity",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Artifact_Project_EntityContainerId",
                        column: x => x.EntityContainerId,
                        principalTable: "Project",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ArtifactReview",
                columns: table => new
                {
                    ArtifactsId = table.Column<Guid>(type: "uuid", nullable: false),
                    ReviewsId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArtifactReview", x => new { x.ArtifactsId, x.ReviewsId });
                    table.ForeignKey(
                        name: "FK_ArtifactReview_Artifact_ArtifactsId",
                        column: x => x.ArtifactsId,
                        principalTable: "Artifact",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ArtifactReview_Review_ReviewsId",
                        column: x => x.ReviewsId,
                        principalTable: "Review",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Model",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ModelName = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Model", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Model_Artifact_Id",
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
                value: "bdaed18c-5e74-46bd-8536-c1255071fd03");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "F3E3BACF-5F7C-4657-88E9-FA904EFB64D7",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "3f1e53a3-5e68-4514-b53a-5f2afba551f1", "AQAAAAEAACcQAAAAEDr2GVdqM38+1tymdhti7jnMdCMq/DgMJicqe7P1sRgZ50asePWsDN40KjHaC59odw==", "facbc854-1ec0-4c4d-8a08-0551243724d4" });

            migrationBuilder.CreateIndex(
                name: "IX_Artifact_EntityContainerId",
                table: "Artifact",
                column: "EntityContainerId");

            migrationBuilder.CreateIndex(
                name: "IX_ArtifactReview_ReviewsId",
                table: "ArtifactReview",
                column: "ReviewsId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ArtifactReview");

            migrationBuilder.DropTable(
                name: "Model");

            migrationBuilder.DropTable(
                name: "Artifact");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "AF8956F8-CA85-4DF2-8CB6-C46D0845B987",
                column: "ConcurrencyStamp",
                value: "9a2b04bc-fbf1-4bbb-aef3-214084843ecd");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "F3E3BACF-5F7C-4657-88E9-FA904EFB64D7",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "4a038e0b-f742-4b04-95fd-2de6f7895f23", "AQAAAAEAACcQAAAAEDQkHLx6aVgzvBDau1UCK8kmQ9nCCLdpmx/7Daop5C5Oo5ntq+2S7l3+WTFjSp0rug==", "ef330ed2-78fa-47d2-b15d-f94b6e75a9c1" });
        }
    }
}
