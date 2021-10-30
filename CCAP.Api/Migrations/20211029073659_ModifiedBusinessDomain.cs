using Microsoft.EntityFrameworkCore.Migrations;

namespace CCAP.Api.Migrations
{
    public partial class ModifiedBusinessDomain : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CreditCardApplications_AppUsers_AppUserId",
                table: "CreditCardApplications");

            migrationBuilder.DropIndex(
                name: "IX_CreditCardApplications_AppUserId",
                table: "CreditCardApplications");

            migrationBuilder.DropColumn(
                name: "AppUserId",
                table: "CreditCardApplications");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AppUserId",
                table: "CreditCardApplications",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_CreditCardApplications_AppUserId",
                table: "CreditCardApplications",
                column: "AppUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_CreditCardApplications_AppUsers_AppUserId",
                table: "CreditCardApplications",
                column: "AppUserId",
                principalTable: "AppUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
