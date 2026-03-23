using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Seikatsu.Migrations
{
    /// <inheritdoc />
    public partial class restockcartitemupdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FrequencyOverride",
                table: "RestockCartItems",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastOrderedAt",
                table: "RestockCartItems",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "NextOrderDate",
                table: "RestockCartItems",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<Guid>(
                name: "ProductId",
                table: "RestockCartItems",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_RestockCartItems_ProductId",
                table: "RestockCartItems",
                column: "ProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_RestockCartItems_Products_ProductId",
                table: "RestockCartItems",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RestockCartItems_Products_ProductId",
                table: "RestockCartItems");

            migrationBuilder.DropIndex(
                name: "IX_RestockCartItems_ProductId",
                table: "RestockCartItems");

            migrationBuilder.DropColumn(
                name: "FrequencyOverride",
                table: "RestockCartItems");

            migrationBuilder.DropColumn(
                name: "LastOrderedAt",
                table: "RestockCartItems");

            migrationBuilder.DropColumn(
                name: "NextOrderDate",
                table: "RestockCartItems");

            migrationBuilder.DropColumn(
                name: "ProductId",
                table: "RestockCartItems");
        }
    }
}
