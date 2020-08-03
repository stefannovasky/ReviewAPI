using Microsoft.EntityFrameworkCore.Migrations;

namespace ReviewApi.Infra.Migrations
{
    public partial class UpdatingImageFileNameLength : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "filename",
                table: "images",
                maxLength: 41,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nchar(36)",
                oldFixedLength: true,
                oldMaxLength: 36);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "filename",
                table: "images",
                type: "nchar(36)",
                fixedLength: true,
                maxLength: 36,
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 41);
        }
    }
}
