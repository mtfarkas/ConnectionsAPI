using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ConnectionsAPI.Database.Migrations
{
    /// <inheritdoc />
    public partial class RenameCategoriesPuzzles2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ConnectionsCard_ConnectionsCategory_CategoryId",
                table: "ConnectionsCard");

            migrationBuilder.DropIndex(
                name: "IX_ConnectionsCard_CategoryId",
                table: "ConnectionsCard");

            migrationBuilder.DropColumn(
                name: "CategoryId",
                table: "ConnectionsCard");

            migrationBuilder.RenameColumn(
                name: "CategoriesCategoryId",
                table: "ConnectionsCard",
                newName: "ConnectionsCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_ConnectionsCard_ConnectionsCategoryId",
                table: "ConnectionsCard",
                column: "ConnectionsCategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_ConnectionsCard_ConnectionsCategory_ConnectionsCategoryId",
                table: "ConnectionsCard",
                column: "ConnectionsCategoryId",
                principalTable: "ConnectionsCategory",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ConnectionsCard_ConnectionsCategory_ConnectionsCategoryId",
                table: "ConnectionsCard");

            migrationBuilder.DropIndex(
                name: "IX_ConnectionsCard_ConnectionsCategoryId",
                table: "ConnectionsCard");

            migrationBuilder.RenameColumn(
                name: "ConnectionsCategoryId",
                table: "ConnectionsCard",
                newName: "CategoriesCategoryId");

            migrationBuilder.AddColumn<int>(
                name: "CategoryId",
                table: "ConnectionsCard",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ConnectionsCard_CategoryId",
                table: "ConnectionsCard",
                column: "CategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_ConnectionsCard_ConnectionsCategory_CategoryId",
                table: "ConnectionsCard",
                column: "CategoryId",
                principalTable: "ConnectionsCategory",
                principalColumn: "Id");
        }
    }
}
