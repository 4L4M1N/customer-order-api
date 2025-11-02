using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CustomerOrderManagement.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ShoppingCartUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "LastModifiedDate",
                table: "ShoppingCarts",
                newName: "UpdatedOnUtc");

            migrationBuilder.RenameColumn(
                name: "CreatedDate",
                table: "ShoppingCarts",
                newName: "CreatedOnUtc");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UpdatedOnUtc",
                table: "ShoppingCarts",
                newName: "LastModifiedDate");

            migrationBuilder.RenameColumn(
                name: "CreatedOnUtc",
                table: "ShoppingCarts",
                newName: "CreatedDate");
        }
    }
}
