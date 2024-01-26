using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TestApiJwt.Migrations
{
    public partial class updateOrderHeader : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ShopId",
                table: "OrderHeaders",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_OrderHeaders_ShopId",
                table: "OrderHeaders",
                column: "ShopId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderHeaders_Shops_ShopId",
                table: "OrderHeaders",
                column: "ShopId",
                principalTable: "Shops",
                principalColumn: "ShopId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderHeaders_Shops_ShopId",
                table: "OrderHeaders");

            migrationBuilder.DropIndex(
                name: "IX_OrderHeaders_ShopId",
                table: "OrderHeaders");

            migrationBuilder.DropColumn(
                name: "ShopId",
                table: "OrderHeaders");
        }
    }
}
