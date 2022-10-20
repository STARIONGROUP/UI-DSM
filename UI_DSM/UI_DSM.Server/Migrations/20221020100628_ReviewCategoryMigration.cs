using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UI_DSM.Server.Migrations
{
    [ExcludeFromCodeCoverage]
    public partial class ReviewCategoryMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ReviewCategory",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ReviewCategoryName = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    TagColor = table.Column<string>(type: "text", nullable: false),
                    EntityContainerId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReviewCategory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReviewCategory_Entity_EntityContainerId",
                        column: x => x.EntityContainerId,
                        principalTable: "Entity",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ReviewCategory_Entity_Id",
                        column: x => x.Id,
                        principalTable: "Entity",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProjectReviewCategory",
                columns: table => new
                {
                    ProjectsId = table.Column<Guid>(type: "uuid", nullable: false),
                    ReviewCategoriesId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectReviewCategory", x => new { x.ProjectsId, x.ReviewCategoriesId });
                    table.ForeignKey(
                        name: "FK_ProjectReviewCategory_Project_ProjectsId",
                        column: x => x.ProjectsId,
                        principalTable: "Project",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProjectReviewCategory_ReviewCategory_ReviewCategoriesId",
                        column: x => x.ReviewCategoriesId,
                        principalTable: "ReviewCategory",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ReviewCategoryReviewObjective",
                columns: table => new
                {
                    ReviewCategoriesId = table.Column<Guid>(type: "uuid", nullable: false),
                    ReviewObjectivesId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReviewCategoryReviewObjective", x => new { x.ReviewCategoriesId, x.ReviewObjectivesId });
                    table.ForeignKey(
                        name: "FK_ReviewCategoryReviewObjective_ReviewCategory_ReviewCategori~",
                        column: x => x.ReviewCategoriesId,
                        principalTable: "ReviewCategory",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ReviewCategoryReviewObjective_ReviewObjective_ReviewObjecti~",
                        column: x => x.ReviewObjectivesId,
                        principalTable: "ReviewObjective",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "AF8956F8-CA85-4DF2-8CB6-C46D0845B987",
                column: "ConcurrencyStamp",
                value: "a271258f-50c3-4acf-abe9-800e3632630a");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "F3E3BACF-5F7C-4657-88E9-FA904EFB64D7",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "bd6144e3-a706-4cb8-964d-fe58c17d232d", "AQAAAAEAACcQAAAAEJcbhJs60X4e5bkx7OaA6LsVAZu0CNbTlY5lTgYdrGt0z7alCDpHMfIJ/faDNX/rbA==", "01f735b6-742a-4ba9-b6f9-a737e92af6e9" });

            migrationBuilder.CreateIndex(
                name: "IX_ProjectReviewCategory_ReviewCategoriesId",
                table: "ProjectReviewCategory",
                column: "ReviewCategoriesId");

            migrationBuilder.CreateIndex(
                name: "IX_ReviewCategory_EntityContainerId",
                table: "ReviewCategory",
                column: "EntityContainerId");

            migrationBuilder.CreateIndex(
                name: "IX_ReviewCategoryReviewObjective_ReviewObjectivesId",
                table: "ReviewCategoryReviewObjective",
                column: "ReviewObjectivesId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProjectReviewCategory");

            migrationBuilder.DropTable(
                name: "ReviewCategoryReviewObjective");

            migrationBuilder.DropTable(
                name: "ReviewCategory");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "AF8956F8-CA85-4DF2-8CB6-C46D0845B987",
                column: "ConcurrencyStamp",
                value: "23f4084d-3589-4c1e-be85-cb3cb10bd9af");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "F3E3BACF-5F7C-4657-88E9-FA904EFB64D7",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "b8fef53c-f1a5-4bad-b675-b4e357947909", "AQAAAAEAACcQAAAAEHJKhKq/Xx/INLV+6pnMFaUVG6RxYtmgHPuvf1xGnxplhYoLWnEDs0BadIo7C6jV7g==", "f672b5f2-928a-4d0b-b632-d45a627cccf2" });
        }
    }
}
