using Microsoft.EntityFrameworkCore.Migrations;

namespace Backend_MVC_Layihe.Migrations
{
    public partial class UpdatedRelationsBetweenClothesColorAndSize : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ColorSizes");

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "SpecialOffers",
                maxLength: 40,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Discount",
                table: "SpecialOffers",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Alternative",
                table: "ClothesImages",
                maxLength: 30,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "ClothesColorSizes",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClothesColorId = table.Column<int>(nullable: false),
                    SizeId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClothesColorSizes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClothesColorSizes_ClothesColors_ClothesColorId",
                        column: x => x.ClothesColorId,
                        principalTable: "ClothesColors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ClothesColorSizes_Sizes_SizeId",
                        column: x => x.SizeId,
                        principalTable: "Sizes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ClothesColorSizes_ClothesColorId",
                table: "ClothesColorSizes",
                column: "ClothesColorId");

            migrationBuilder.CreateIndex(
                name: "IX_ClothesColorSizes_SizeId",
                table: "ClothesColorSizes",
                column: "SizeId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ClothesColorSizes");

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "SpecialOffers",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 40);

            migrationBuilder.AlterColumn<string>(
                name: "Discount",
                table: "SpecialOffers",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "Alternative",
                table: "ClothesImages",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 30);

            migrationBuilder.CreateTable(
                name: "ColorSizes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ColorId = table.Column<int>(type: "int", nullable: false),
                    SizeId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ColorSizes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ColorSizes_Colors_ColorId",
                        column: x => x.ColorId,
                        principalTable: "Colors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ColorSizes_Sizes_SizeId",
                        column: x => x.SizeId,
                        principalTable: "Sizes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ColorSizes_ColorId",
                table: "ColorSizes",
                column: "ColorId");

            migrationBuilder.CreateIndex(
                name: "IX_ColorSizes_SizeId",
                table: "ColorSizes",
                column: "SizeId");
        }
    }
}
