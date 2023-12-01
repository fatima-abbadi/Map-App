using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TestApiJwt.Migrations
{
    public partial class fav : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Favorites_Shops_ShopId",
                table: "Favorites");

            migrationBuilder.AddForeignKey(
                name: "FK_Favorites_Shops_ShopId",
                table: "Favorites",
                column: "ShopId",
                principalTable: "Shops",
                principalColumn: "ShopId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Favorites_Shops_ShopId",
                table: "Favorites");

            migrationBuilder.AddForeignKey(
                name: "FK_Favorites_Shops_ShopId",
                table: "Favorites",
                column: "ShopId",
                principalTable: "Shops",
                principalColumn: "ShopId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
