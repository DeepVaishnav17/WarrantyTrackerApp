using Microsoft.EntityFrameworkCore.Migrations;

namespace WarrantyTracker.Migrations
{
    public partial class AddLastWarrantyStatusToAppliance : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "LastWarrantyStatus",
                table: "Appliances",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastWarrantyStatus",
                table: "Appliances");
        }
    }
}
