using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ReviewApi.Infra.Migrations
{
    public partial class FixingRelation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_users_profile_images_profile_image_id",
                table: "users");

            migrationBuilder.DropIndex(
                name: "IX_users_profile_image_id",
                table: "users");

            migrationBuilder.DropColumn(
                name: "profile_image_id",
                table: "users");

            migrationBuilder.CreateIndex(
                name: "IX_profile_images_user_id",
                table: "profile_images",
                column: "user_id",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_profile_images_users_user_id",
                table: "profile_images",
                column: "user_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_profile_images_users_user_id",
                table: "profile_images");

            migrationBuilder.DropIndex(
                name: "IX_profile_images_user_id",
                table: "profile_images");

            migrationBuilder.AddColumn<Guid>(
                name: "profile_image_id",
                table: "users",
                type: "uniqueidentifier",
                maxLength: 36,
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

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
    }
}
