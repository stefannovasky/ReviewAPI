using Microsoft.EntityFrameworkCore.Migrations;

namespace ReviewApi.Infra.Migrations
{
    public partial class AddingResetPasswordCodeOnUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "reset_password_code",
                table: "users",
                maxLength: 8,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "reset_password_code",
                table: "users");
        }
    }
}
