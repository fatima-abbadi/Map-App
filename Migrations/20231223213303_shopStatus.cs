using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TestApiJwt.Migrations
{
    public partial class shopStatus : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Shops",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "Shops");
        }
    }
}
