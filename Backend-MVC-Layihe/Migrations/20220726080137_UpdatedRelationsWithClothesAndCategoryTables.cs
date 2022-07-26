using Microsoft.EntityFrameworkCore.Migrations;

namespace Backend_MVC_Layihe.Migrations
{
    public partial class UpdatedRelationsWithClothesAndCategoryTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ClothesCategories");

            migrationBuilder.AddColumn<int>(
                name: "CategoryId",
                table: "Clothes",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Clothes_CategoryId",
                table: "Clothes",
                column: "CategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Clothes_Categories_CategoryId",
                table: "Clothes",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Clothes_Categories_CategoryId",
                table: "Clothes");

            migrationBuilder.DropIndex(
                name: "IX_Clothes_CategoryId",
                table: "Clothes");

            migrationBuilder.DropColumn(
                name: "CategoryId",
                table: "Clothes");

            migrationBuilder.CreateTable(
                name: "ClothesCategories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CategoryId = table.Column<int>(type: "int", nullable: false),
                    ClothesId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClothesCategories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClothesCategories_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ClothesCategories_Clothes_ClothesId",
                        column: x => x.ClothesId,
                        principalTable: "Clothes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ClothesCategories_CategoryId",
                table: "ClothesCategories",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_ClothesCategories_ClothesId",
                table: "ClothesCategories",
                column: "ClothesId");
        }
    }
}
