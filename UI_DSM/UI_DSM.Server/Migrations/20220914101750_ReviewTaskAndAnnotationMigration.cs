using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace UI_DSM.Server.Migrations
{
    [ExcludeFromCodeCoverage]
    public partial class ReviewTaskAndAnnotationMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ReviewObjective_Entity_Id",
                table: "ReviewObjective");

            migrationBuilder.DropForeignKey(
                name: "FK_ReviewObjective_Participant_AuthorId",
                table: "ReviewObjective");

            migrationBuilder.DropIndex(
                name: "IX_ReviewObjective_AuthorId",
                table: "ReviewObjective");

            migrationBuilder.DropColumn(
                name: "AuthorId",
                table: "ReviewObjective");

            migrationBuilder.DropColumn(
                name: "CreatedOn",
                table: "ReviewObjective");

            migrationBuilder.CreateTable(
                name: "AnnotatableItem",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AuthorId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnnotatableItem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AnnotatableItem_Entity_Id",
                        column: x => x.Id,
                        principalTable: "Entity",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AnnotatableItem_Participant_AuthorId",
                        column: x => x.AuthorId,
                        principalTable: "Participant",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Annotation",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AuthorId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Content = table.Column<string>(type: "text", nullable: false),
                    EntityContainerId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Annotation", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Annotation_Entity_Id",
                        column: x => x.Id,
                        principalTable: "Entity",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Annotation_Participant_AuthorId",
                        column: x => x.AuthorId,
                        principalTable: "Participant",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Annotation_Project_EntityContainerId",
                        column: x => x.EntityContainerId,
                        principalTable: "Project",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ReviewTask",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    TaskNumber = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    AuthorId = table.Column<Guid>(type: "uuid", nullable: true),
                    IsAssignedToId = table.Column<Guid>(type: "uuid", nullable: true),
                    EntityContainerId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReviewTask", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReviewTask_Entity_Id",
                        column: x => x.Id,
                        principalTable: "Entity",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ReviewTask_Participant_AuthorId",
                        column: x => x.AuthorId,
                        principalTable: "Participant",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ReviewTask_Participant_IsAssignedToId",
                        column: x => x.IsAssignedToId,
                        principalTable: "Participant",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ReviewTask_ReviewObjective_EntityContainerId",
                        column: x => x.EntityContainerId,
                        principalTable: "ReviewObjective",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AnnotatableItemAnnotation",
                columns: table => new
                {
                    AnnotatableItemsId = table.Column<Guid>(type: "uuid", nullable: false),
                    AnnotationsId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnnotatableItemAnnotation", x => new { x.AnnotatableItemsId, x.AnnotationsId });
                    table.ForeignKey(
                        name: "FK_AnnotatableItemAnnotation_AnnotatableItem_AnnotatableItemsId",
                        column: x => x.AnnotatableItemsId,
                        principalTable: "AnnotatableItem",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AnnotatableItemAnnotation_Annotation_AnnotationsId",
                        column: x => x.AnnotationsId,
                        principalTable: "Annotation",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Comment",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Comment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Comment_Annotation_Id",
                        column: x => x.Id,
                        principalTable: "Annotation",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Feedback",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Feedback", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Feedback_Annotation_Id",
                        column: x => x.Id,
                        principalTable: "Annotation",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Note",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Note", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Note_Annotation_Id",
                        column: x => x.Id,
                        principalTable: "Annotation",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Reply",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AuthorId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Content = table.Column<string>(type: "text", nullable: false),
                    EntityContainerId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reply", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Reply_Comment_EntityContainerId",
                        column: x => x.EntityContainerId,
                        principalTable: "Comment",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Reply_Entity_Id",
                        column: x => x.Id,
                        principalTable: "Entity",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Reply_Participant_AuthorId",
                        column: x => x.AuthorId,
                        principalTable: "Participant",
                        principalColumn: "Id");
                });

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

            migrationBuilder.CreateIndex(
                name: "IX_AnnotatableItem_AuthorId",
                table: "AnnotatableItem",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_AnnotatableItemAnnotation_AnnotationsId",
                table: "AnnotatableItemAnnotation",
                column: "AnnotationsId");

            migrationBuilder.CreateIndex(
                name: "IX_Annotation_AuthorId",
                table: "Annotation",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_Annotation_EntityContainerId",
                table: "Annotation",
                column: "EntityContainerId");

            migrationBuilder.CreateIndex(
                name: "IX_Reply_AuthorId",
                table: "Reply",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_Reply_EntityContainerId",
                table: "Reply",
                column: "EntityContainerId");

            migrationBuilder.CreateIndex(
                name: "IX_ReviewTask_AuthorId",
                table: "ReviewTask",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_ReviewTask_EntityContainerId",
                table: "ReviewTask",
                column: "EntityContainerId");

            migrationBuilder.CreateIndex(
                name: "IX_ReviewTask_IsAssignedToId",
                table: "ReviewTask",
                column: "IsAssignedToId");

            migrationBuilder.AddForeignKey(
                name: "FK_ReviewObjective_AnnotatableItem_Id",
                table: "ReviewObjective",
                column: "Id",
                principalTable: "AnnotatableItem",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ReviewObjective_AnnotatableItem_Id",
                table: "ReviewObjective");

            migrationBuilder.DropTable(
                name: "AnnotatableItemAnnotation");

            migrationBuilder.DropTable(
                name: "Feedback");

            migrationBuilder.DropTable(
                name: "Note");

            migrationBuilder.DropTable(
                name: "Reply");

            migrationBuilder.DropTable(
                name: "ReviewTask");

            migrationBuilder.DropTable(
                name: "AnnotatableItem");

            migrationBuilder.DropTable(
                name: "Comment");

            migrationBuilder.DropTable(
                name: "Annotation");

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
                name: "IX_ReviewObjective_AuthorId",
                table: "ReviewObjective",
                column: "AuthorId");

            migrationBuilder.AddForeignKey(
                name: "FK_ReviewObjective_Entity_Id",
                table: "ReviewObjective",
                column: "Id",
                principalTable: "Entity",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ReviewObjective_Participant_AuthorId",
                table: "ReviewObjective",
                column: "AuthorId",
                principalTable: "Participant",
                principalColumn: "Id");
        }
    }
}
