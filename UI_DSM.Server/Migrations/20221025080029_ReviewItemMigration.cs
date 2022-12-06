using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UI_DSM.Server.Migrations
{
    using System.Diagnostics.CodeAnalysis;

    [ExcludeFromCodeCoverage]
    public partial class ReviewItemMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AnnotatableItem_Participant_AuthorId",
                table: "AnnotatableItem");

            migrationBuilder.DropIndex(
                name: "IX_AnnotatableItem_AuthorId",
                table: "AnnotatableItem");

            migrationBuilder.DropColumn(
                name: "AuthorId",
                table: "AnnotatableItem");

            migrationBuilder.DropColumn(
                name: "CreatedOn",
                table: "AnnotatableItem");

            migrationBuilder.AddColumn<Guid>(
                name: "AuthorId",
                table: "ReviewObjective",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedOn",
                table: "ReviewObjective",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Comment",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "View",
                table: "Comment",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "ReviewItem",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ThingId = table.Column<Guid>(type: "uuid", nullable: false),
                    EntityContainerId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReviewItem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReviewItem_AnnotatableItem_Id",
                        column: x => x.Id,
                        principalTable: "AnnotatableItem",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ReviewItem_Review_EntityContainerId",
                        column: x => x.EntityContainerId,
                        principalTable: "Review",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ReviewCategoryReviewItem",
                columns: table => new
                {
                    ReviewCategoriesId = table.Column<Guid>(type: "uuid", nullable: false),
                    ReviewItemsId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReviewCategoryReviewItem", x => new { x.ReviewCategoriesId, x.ReviewItemsId });
                    table.ForeignKey(
                        name: "FK_ReviewCategoryReviewItem_ReviewCategory_ReviewCategoriesId",
                        column: x => x.ReviewCategoriesId,
                        principalTable: "ReviewCategory",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ReviewCategoryReviewItem_ReviewItem_ReviewItemsId",
                        column: x => x.ReviewItemsId,
                        principalTable: "ReviewItem",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "AF8956F8-CA85-4DF2-8CB6-C46D0845B987",
                column: "ConcurrencyStamp",
                value: "932072f3-26ba-4d47-929d-163f2ae5d469");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "F3E3BACF-5F7C-4657-88E9-FA904EFB64D7",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "3549031d-9315-4015-83af-fbe13af47ae5", "AQAAAAEAACcQAAAAEDj2jZAvO/XBOAMvqMq2jz4fL1Px6gRd4dVFqw5R9qGQIt8LY6bj6kHQKaKS7FBL5w==", "ebc97368-4d60-43d1-bf6b-fbcca662c28a" });

            migrationBuilder.CreateIndex(
                name: "IX_ReviewObjective_AuthorId",
                table: "ReviewObjective",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_ReviewCategory_ReviewCategoryName",
                table: "ReviewCategory",
                column: "ReviewCategoryName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ReviewCategoryReviewItem_ReviewItemsId",
                table: "ReviewCategoryReviewItem",
                column: "ReviewItemsId");

            migrationBuilder.CreateIndex(
                name: "IX_ReviewItem_EntityContainerId",
                table: "ReviewItem",
                column: "EntityContainerId");

            migrationBuilder.AddForeignKey(
                name: "FK_ReviewObjective_Participant_AuthorId",
                table: "ReviewObjective",
                column: "AuthorId",
                principalTable: "Participant",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ReviewObjective_Participant_AuthorId",
                table: "ReviewObjective");

            migrationBuilder.DropTable(
                name: "ReviewCategoryReviewItem");

            migrationBuilder.DropTable(
                name: "ReviewItem");

            migrationBuilder.DropIndex(
                name: "IX_ReviewObjective_AuthorId",
                table: "ReviewObjective");

            migrationBuilder.DropIndex(
                name: "IX_ReviewCategory_ReviewCategoryName",
                table: "ReviewCategory");

            migrationBuilder.DropColumn(
                name: "AuthorId",
                table: "ReviewObjective");

            migrationBuilder.DropColumn(
                name: "CreatedOn",
                table: "ReviewObjective");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Comment");

            migrationBuilder.DropColumn(
                name: "View",
                table: "Comment");

            migrationBuilder.AddColumn<Guid>(
                name: "AuthorId",
                table: "AnnotatableItem",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedOn",
                table: "AnnotatableItem",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

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
                name: "IX_AnnotatableItem_AuthorId",
                table: "AnnotatableItem",
                column: "AuthorId");

            migrationBuilder.AddForeignKey(
                name: "FK_AnnotatableItem_Participant_AuthorId",
                table: "AnnotatableItem",
                column: "AuthorId",
                principalTable: "Participant",
                principalColumn: "Id");
        }
    }
}
