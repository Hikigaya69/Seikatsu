using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Seikatsu.Migrations
{
    /// <inheritdoc />
    public partial class cartupdates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_RestockCarts_CustomerId",
                table: "RestockCarts");

            migrationBuilder.DropIndex(
                name: "IX_Carts_CustomerId",
                table: "Carts");

            migrationBuilder.CreateIndex(
                name: "IX_RestockCarts_CustomerId",
                table: "RestockCarts",
                column: "CustomerId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Carts_CustomerId",
                table: "Carts",
                column: "CustomerId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_RestockCarts_CustomerId",
                table: "RestockCarts");

            migrationBuilder.DropIndex(
                name: "IX_Carts_CustomerId",
                table: "Carts");

            migrationBuilder.CreateIndex(
                name: "IX_RestockCarts_CustomerId",
                table: "RestockCarts",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Carts_CustomerId",
                table: "Carts",
                column: "CustomerId");
        }
    }
}
