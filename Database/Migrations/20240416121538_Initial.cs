using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ConnectionsAPI.Database.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Puzzles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CreatedDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    PrintDate = table.Column<string>(type: "TEXT", nullable: false),
                    EditorName = table.Column<string>(type: "TEXT", nullable: false),
                    Index = table.Column<int>(type: "INTEGER", nullable: false),
                    ContentMD5 = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Puzzles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PuzzleCategory",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Color = table.Column<int>(type: "INTEGER", nullable: false),
                    PuzzleId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PuzzleCategory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PuzzleCategory_Puzzles_PuzzleId",
                        column: x => x.PuzzleId,
                        principalTable: "Puzzles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PuzzleCard",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Content = table.Column<string>(type: "TEXT", nullable: false),
                    Position = table.Column<int>(type: "INTEGER", nullable: false),
                    PuzzleCategoryId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PuzzleCard", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PuzzleCard_PuzzleCategory_PuzzleCategoryId",
                        column: x => x.PuzzleCategoryId,
                        principalTable: "PuzzleCategory",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PuzzleCard_PuzzleCategoryId",
                table: "PuzzleCard",
                column: "PuzzleCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_PuzzleCategory_PuzzleId",
                table: "PuzzleCategory",
                column: "PuzzleId");

            migrationBuilder.CreateIndex(
                name: "IX_Puzzles_PrintDate",
                table: "Puzzles",
                column: "PrintDate",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PuzzleCard");

            migrationBuilder.DropTable(
                name: "PuzzleCategory");

            migrationBuilder.DropTable(
                name: "Puzzles");
        }
    }
}
