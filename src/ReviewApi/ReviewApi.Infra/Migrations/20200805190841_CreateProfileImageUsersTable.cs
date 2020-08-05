using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ReviewApi.Infra.Migrations
{
    public partial class CreateProfileImageUsersTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "images");

            migrationBuilder.DropColumn(
                name: "image_id",
                table: "users");

            migrationBuilder.AddColumn<Guid>(
                name: "profile_image_id",
                table: "users",
                maxLength: 36,
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "profile_images",
                columns: table => new
                {
                    id = table.Column<Guid>(nullable: false),
                    filename = table.Column<string>(maxLength: 41, nullable: false),
                    filepath = table.Column<string>(maxLength: 300, nullable: false),
                    user_id = table.Column<Guid>(maxLength: 36, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_profile_images", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_users_profile_image_id",
                table: "users",
                column: "profile_image_id",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_users_profile_images_profile_image_id",
                table: "users",
                column: "profile_image_id",
                principalTable: "profile_images",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_users_profile_images_profile_image_id",
                table: "users");

            migrationBuilder.DropTable(
                name: "profile_images");

            migrationBuilder.DropIndex(
                name: "IX_users_profile_image_id",
                table: "users");

            migrationBuilder.DropColumn(
                name: "profile_image_id",
                table: "users");

            migrationBuilder.AddColumn<Guid>(
                name: "image_id",
                table: "users",
                type: "uniqueidentifier",
                maxLength: 36,
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "images",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    filename = table.Column<string>(type: "nvarchar(41)", maxLength: 41, nullable: false),
                    filepath = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    user_id = table.Column<Guid>(type: "uniqueidentifier", maxLength: 36, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_images", x => x.id);
                    table.ForeignKey(
                        name: "FK_images_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_images_user_id",
                table: "images",
                column: "user_id",
                unique: true);
        }
    }
}
