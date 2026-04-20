using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Seikatsu.Migrations
{
    /// <inheritdoc />
    public partial class role_added_to_customer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Role",
                table: "Customers",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Role",
                table: "Customers");
        }
    }
}
