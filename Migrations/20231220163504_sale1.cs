using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TestApiJwt.Migrations
{
    public partial class sale1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DurationInDays",
                table: "Sales",
                newName: "DurationInMinutes");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DurationInMinutes",
                table: "Sales",
                newName: "DurationInDays");
        }
    }
}
