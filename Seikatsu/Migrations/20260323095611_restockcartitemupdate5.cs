using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Seikatsu.Migrations
{
    /// <inheritdoc />
    public partial class restockcartitemupdate5 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Frequency",
                table: "RestockCarts");

            migrationBuilder.RenameColumn(
                name: "FrequencyOverride",
                table: "RestockCartItems",
                newName: "Frequency");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Frequency",
                table: "RestockCartItems",
                newName: "FrequencyOverride");

            migrationBuilder.AddColumn<string>(
                name: "Frequency",
                table: "RestockCarts",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
