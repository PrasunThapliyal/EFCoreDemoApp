using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EFCoreDemoApp.Migrations
{
    /// <inheritdoc />
    public partial class AddBookStoreTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BookStoreId",
                table: "Books",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "BookStoreId",
                table: "Authors",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "BookStores",
                columns: table => new
                {
                    BookStoreId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookStores", x => x.BookStoreId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Books_BookStoreId",
                table: "Books",
                column: "BookStoreId");

            migrationBuilder.CreateIndex(
                name: "IX_Authors_BookStoreId",
                table: "Authors",
                column: "BookStoreId");

            migrationBuilder.AddForeignKey(
                name: "FK_Authors_BookStores_BookStoreId",
                table: "Authors",
                column: "BookStoreId",
                principalTable: "BookStores",
                principalColumn: "BookStoreId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Books_BookStores_BookStoreId",
                table: "Books",
                column: "BookStoreId",
                principalTable: "BookStores",
                principalColumn: "BookStoreId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Authors_BookStores_BookStoreId",
                table: "Authors");

            migrationBuilder.DropForeignKey(
                name: "FK_Books_BookStores_BookStoreId",
                table: "Books");

            migrationBuilder.DropTable(
                name: "BookStores");

            migrationBuilder.DropIndex(
                name: "IX_Books_BookStoreId",
                table: "Books");

            migrationBuilder.DropIndex(
                name: "IX_Authors_BookStoreId",
                table: "Authors");

            migrationBuilder.DropColumn(
                name: "BookStoreId",
                table: "Books");

            migrationBuilder.DropColumn(
                name: "BookStoreId",
                table: "Authors");
        }
    }
}
