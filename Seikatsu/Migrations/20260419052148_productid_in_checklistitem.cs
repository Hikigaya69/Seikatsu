using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Seikatsu.Migrations
{
    /// <inheritdoc />
    public partial class productid_in_checklistitem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ProductId",
                table: "CheckListItems",
                type: "uuid",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProductId",
                table: "CheckListItems");
        }
    }
}
