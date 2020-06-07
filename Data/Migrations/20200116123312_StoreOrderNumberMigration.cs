using Microsoft.EntityFrameworkCore.Migrations;

namespace Ord.WebApi.Data.Migrations
{
    public partial class StoreOrderNumberMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "StoreOrderNumber",
                table: "Orders",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StoreOrderNumber",
                table: "Orders");
        }
    }
}
