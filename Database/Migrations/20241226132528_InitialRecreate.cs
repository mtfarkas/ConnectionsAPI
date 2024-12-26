using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ConnectionsAPI.Database.Migrations
{
    /// <inheritdoc />
    public partial class InitialRecreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ConnectionsPuzzles",
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
                    table.PrimaryKey("PK_ConnectionsPuzzles", x => x.Id);
                });

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
                        name: "FK_ConnectionsCategory_ConnectionsPuzzles_ConnectionsPuzzleId",
                        column: x => x.ConnectionsPuzzleId,
                        principalTable: "ConnectionsPuzzles",
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
                    ConnectionsCategoryId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConnectionsCard", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ConnectionsCard_ConnectionsCategory_ConnectionsCategoryId",
                        column: x => x.ConnectionsCategoryId,
                        principalTable: "ConnectionsCategory",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ConnectionsCard_ConnectionsCategoryId",
                table: "ConnectionsCard",
                column: "ConnectionsCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_ConnectionsCategory_ConnectionsPuzzleId",
                table: "ConnectionsCategory",
                column: "ConnectionsPuzzleId");

            migrationBuilder.CreateIndex(
                name: "IX_ConnectionsPuzzles_PrintDate",
                table: "ConnectionsPuzzles",
                column: "PrintDate",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ConnectionsCard");

            migrationBuilder.DropTable(
                name: "ConnectionsCategory");

            migrationBuilder.DropTable(
                name: "ConnectionsPuzzles");
        }
    }
}
