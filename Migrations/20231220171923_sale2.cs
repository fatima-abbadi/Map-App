using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TestApiJwt.Migrations
{
    public partial class sale2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Sales_Shops_ShopId",
                table: "Sales");

            migrationBuilder.AddForeignKey(
                name: "FK_Sales_Shops_ShopId",
                table: "Sales",
                column: "ShopId",
                principalTable: "Shops",
                principalColumn: "ShopId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Sales_Shops_ShopId",
                table: "Sales");

            migrationBuilder.AddForeignKey(
                name: "FK_Sales_Shops_ShopId",
                table: "Sales",
                column: "ShopId",
                principalTable: "Shops",
                principalColumn: "ShopId");
        }
    }
}
