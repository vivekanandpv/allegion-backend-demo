using Microsoft.EntityFrameworkCore.Migrations;

namespace CCAP.Api.Migrations
{
    public partial class AddedToUserDomain : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsForcedToResetPassword",
                table: "AppUsers",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsLocked",
                table: "AppUsers",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "NWrongAttempts",
                table: "AppUsers",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsForcedToResetPassword",
                table: "AppUsers");

            migrationBuilder.DropColumn(
                name: "IsLocked",
                table: "AppUsers");

            migrationBuilder.DropColumn(
                name: "NWrongAttempts",
                table: "AppUsers");
        }
    }
}
