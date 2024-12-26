using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ConnectionsAPI.Database.Migrations
{
    /// <inheritdoc />
    public partial class RenameTablesCorrectly : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CategoriesCard");

            migrationBuilder.DropTable(
                name: "CategoriesCategory");

            migrationBuilder.CreateTable(
                name: "ConnectionsCategory",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Color = table.Column<int>(type: "INTEGER", nullable: false),
                    ConnectionsPuzzleId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConnectionsCategory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ConnectionsCategory_CategoriesPuzzles_ConnectionsPuzzleId",
                        column: x => x.ConnectionsPuzzleId,
                        principalTable: "CategoriesPuzzles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ConnectionsCard",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Content = table.Column<string>(type: "TEXT", nullable: false),
                    Position = table.Column<int>(type: "INTEGER", nullable: false),
                    CategoriesCategoryId = table.Column<int>(type: "INTEGER", nullable: false),
                    CategoryId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConnectionsCard", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ConnectionsCard_ConnectionsCategory_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "ConnectionsCategory",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ConnectionsCard_CategoryId",
                table: "ConnectionsCard",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_ConnectionsCategory_ConnectionsPuzzleId",
                table: "ConnectionsCategory",
                column: "ConnectionsPuzzleId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ConnectionsCard");

            migrationBuilder.DropTable(
                name: "ConnectionsCategory");

            migrationBuilder.CreateTable(
                name: "CategoriesCategory",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CategoriesPuzzleId = table.Column<int>(type: "INTEGER", nullable: false),
                    Color = table.Column<int>(type: "INTEGER", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CategoriesCategory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CategoriesCategory_CategoriesPuzzles_CategoriesPuzzleId",
                        column: x => x.CategoriesPuzzleId,
                        principalTable: "CategoriesPuzzles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CategoriesCard",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CategoriesCategoryId = table.Column<int>(type: "INTEGER", nullable: false),
                    Content = table.Column<string>(type: "TEXT", nullable: false),
                    Position = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CategoriesCard", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CategoriesCard_CategoriesCategory_CategoriesCategoryId",
                        column: x => x.CategoriesCategoryId,
                        principalTable: "CategoriesCategory",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CategoriesCard_CategoriesCategoryId",
                table: "CategoriesCard",
                column: "CategoriesCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_CategoriesCategory_CategoriesPuzzleId",
                table: "CategoriesCategory",
                column: "CategoriesPuzzleId");
        }
    }
}
