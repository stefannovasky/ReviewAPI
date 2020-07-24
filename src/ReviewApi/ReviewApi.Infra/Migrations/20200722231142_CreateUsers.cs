using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ReviewApi.Infra.Migrations
{
    public partial class CreateUsers : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    id = table.Column<Guid>(nullable: false),
                    deleted = table.Column<bool>(nullable: false, defaultValue: false),
                    name = table.Column<string>(maxLength: 150, nullable: false),
                    Email = table.Column<string>(nullable: true),
                    password_hash = table.Column<string>(maxLength: 64, nullable: false),
                    confirmation_code = table.Column<string>(maxLength: 8, nullable: true),
                    confirmed = table.Column<bool>(nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "users");
        }
    }
}
