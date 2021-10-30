using Microsoft.EntityFrameworkCore.Migrations;

namespace CCAP.Api.Migrations
{
    public partial class ImprovedTheDomainForCreditCardService : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "LimitRequired",
                table: "CreditCardApplications",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LimitRequired",
                table: "CreditCardApplications");
        }
    }
}
