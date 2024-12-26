using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ConnectionsAPI.Database.Migrations
{
    /// <inheritdoc />
    public partial class RenameCollectionsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ConnectionsCategory_CategoriesPuzzles_ConnectionsPuzzleId",
                table: "ConnectionsCategory");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CategoriesPuzzles",
                table: "CategoriesPuzzles");

            migrationBuilder.RenameTable(
                name: "CategoriesPuzzles",
                newName: "ConnectionsPuzzles");

            migrationBuilder.RenameIndex(
                name: "IX_CategoriesPuzzles_PrintDate",
                table: "ConnectionsPuzzles",
                newName: "IX_ConnectionsPuzzles_PrintDate");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ConnectionsPuzzles",
                table: "ConnectionsPuzzles",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ConnectionsCategory_ConnectionsPuzzles_ConnectionsPuzzleId",
                table: "ConnectionsCategory",
                column: "ConnectionsPuzzleId",
                principalTable: "ConnectionsPuzzles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ConnectionsCategory_ConnectionsPuzzles_ConnectionsPuzzleId",
                table: "ConnectionsCategory");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ConnectionsPuzzles",
                table: "ConnectionsPuzzles");

            migrationBuilder.RenameTable(
                name: "ConnectionsPuzzles",
                newName: "CategoriesPuzzles");

            migrationBuilder.RenameIndex(
                name: "IX_ConnectionsPuzzles_PrintDate",
                table: "CategoriesPuzzles",
                newName: "IX_CategoriesPuzzles_PrintDate");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CategoriesPuzzles",
                table: "CategoriesPuzzles",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ConnectionsCategory_CategoriesPuzzles_ConnectionsPuzzleId",
                table: "ConnectionsCategory",
                column: "ConnectionsPuzzleId",
                principalTable: "CategoriesPuzzles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
