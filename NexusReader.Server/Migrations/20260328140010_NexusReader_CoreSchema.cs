using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NexusReader.Server.Migrations
{
    /// <inheritdoc />
    public partial class NexusReader_CoreSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Chapters_Books_BookModelId",
                table: "Chapters");

            migrationBuilder.DropIndex(
                name: "IX_Chapters_BookModelId",
                table: "Chapters");

            migrationBuilder.RenameColumn(
                name: "BookModelId",
                table: "Chapters",
                newName: "BookId");

            migrationBuilder.AddColumn<int>(
                name: "ChapterNumber",
                table: "Chapters",
                type: "int",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.RenameColumn(
                name: "CoverImage",
                table: "Books",
                newName: "CoverImageUrl");

            migrationBuilder.AddColumn<DateTime>(
                name: "UploadDate",
                table: "Books",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.CreateTable(
                name: "Favorites",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    BookId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Favorites", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Favorites_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Favorites_Books_BookId",
                        column: x => x.BookId,
                        principalTable: "Books",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Chapters_BookId",
                table: "Chapters",
                column: "BookId");

            migrationBuilder.CreateIndex(
                name: "IX_Favorites_BookId",
                table: "Favorites",
                column: "BookId");

            migrationBuilder.CreateIndex(
                name: "IX_Favorites_UserId_BookId",
                table: "Favorites",
                columns: new[] { "UserId", "BookId" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Chapters_Books_BookId",
                table: "Chapters",
                column: "BookId",
                principalTable: "Books",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Chapters_Books_BookId",
                table: "Chapters");

            migrationBuilder.DropTable(
                name: "Favorites");

            migrationBuilder.DropIndex(
                name: "IX_Chapters_BookId",
                table: "Chapters");

            migrationBuilder.DropColumn(
                name: "ChapterNumber",
                table: "Chapters");

            migrationBuilder.DropColumn(
                name: "UploadDate",
                table: "Books");

            migrationBuilder.RenameColumn(
                name: "BookId",
                table: "Chapters",
                newName: "BookModelId");

            migrationBuilder.RenameColumn(
                name: "CoverImageUrl",
                table: "Books",
                newName: "CoverImage");

            migrationBuilder.CreateIndex(
                name: "IX_Chapters_BookModelId",
                table: "Chapters",
                column: "BookModelId");

            migrationBuilder.AddForeignKey(
                name: "FK_Chapters_Books_BookModelId",
                table: "Chapters",
                column: "BookModelId",
                principalTable: "Books",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
