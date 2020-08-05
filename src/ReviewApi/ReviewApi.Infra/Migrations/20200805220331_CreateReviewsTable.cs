using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ReviewApi.Infra.Migrations
{
    public partial class CreateReviewsTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "reviews",
                columns: table => new
                {
                    id = table.Column<Guid>(nullable: false),
                    title = table.Column<string>(maxLength: 150, nullable: false),
                    text = table.Column<string>(maxLength: 1500, nullable: false),
                    stars = table.Column<int>(nullable: false),
                    creator_id = table.Column<Guid>(maxLength: 36, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_reviews", x => x.id);
                    table.ForeignKey(
                        name: "FK_reviews_users_creator_id",
                        column: x => x.creator_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "review_images",
                columns: table => new
                {
                    id = table.Column<Guid>(nullable: false),
                    filename = table.Column<string>(maxLength: 41, nullable: false),
                    filepath = table.Column<string>(maxLength: 300, nullable: false),
                    review_id = table.Column<Guid>(maxLength: 36, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_review_images", x => x.id);
                    table.ForeignKey(
                        name: "FK_review_images_reviews_review_id",
                        column: x => x.review_id,
                        principalTable: "reviews",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_review_images_review_id",
                table: "review_images",
                column: "review_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_reviews_creator_id",
                table: "reviews",
                column: "creator_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "review_images");

            migrationBuilder.DropTable(
                name: "reviews");
        }
    }
}
