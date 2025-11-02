using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CustomerOrderManagement.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addtoekn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                table: "ShoppingCarts",
                type: "rowversion",
                rowVersion: true,
                nullable: false,
                defaultValue: new byte[0]);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "ShoppingCarts");
        }
    }
}
