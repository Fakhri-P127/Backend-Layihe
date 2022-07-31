using Microsoft.EntityFrameworkCore.Migrations;

namespace Backend_MVC_Layihe.Migrations
{
    public partial class CreatedClothesInformationTableAndAddedRelationsForIt : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExtraInfo",
                table: "Clothes");

            migrationBuilder.AddColumn<int>(
                name: "ClothesInformationId",
                table: "Clothes",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ClothesInformations",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AdditionalInfo = table.Column<string>(nullable: false),
                    Definition = table.Column<string>(maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClothesInformations", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Clothes_ClothesInformationId",
                table: "Clothes",
                column: "ClothesInformationId");

            migrationBuilder.AddForeignKey(
                name: "FK_Clothes_ClothesInformations_ClothesInformationId",
                table: "Clothes",
                column: "ClothesInformationId",
                principalTable: "ClothesInformations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Clothes_ClothesInformations_ClothesInformationId",
                table: "Clothes");

            migrationBuilder.DropTable(
                name: "ClothesInformations");

            migrationBuilder.DropIndex(
                name: "IX_Clothes_ClothesInformationId",
                table: "Clothes");

            migrationBuilder.DropColumn(
                name: "ClothesInformationId",
                table: "Clothes");

            migrationBuilder.AddColumn<string>(
                name: "ExtraInfo",
                table: "Clothes",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
