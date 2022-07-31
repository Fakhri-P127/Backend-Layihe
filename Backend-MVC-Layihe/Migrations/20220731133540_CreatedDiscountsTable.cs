using Microsoft.EntityFrameworkCore.Migrations;

namespace Backend_MVC_Layihe.Migrations
{
    public partial class CreatedDiscountsTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DiscountPrice",
                table: "Clothes");

            migrationBuilder.AddColumn<int>(
                name: "DiscountId",
                table: "Clothes",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Discounts",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PercentKey = table.Column<string>(maxLength: 30, nullable: false),
                    PercentValue = table.Column<decimal>(type: "decimal(6,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Discounts", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Clothes_DiscountId",
                table: "Clothes",
                column: "DiscountId");

            migrationBuilder.AddForeignKey(
                name: "FK_Clothes_Discounts_DiscountId",
                table: "Clothes",
                column: "DiscountId",
                principalTable: "Discounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Clothes_Discounts_DiscountId",
                table: "Clothes");

            migrationBuilder.DropTable(
                name: "Discounts");

            migrationBuilder.DropIndex(
                name: "IX_Clothes_DiscountId",
                table: "Clothes");

            migrationBuilder.DropColumn(
                name: "DiscountId",
                table: "Clothes");

            migrationBuilder.AddColumn<decimal>(
                name: "DiscountPrice",
                table: "Clothes",
                type: "decimal(6,2)",
                nullable: false,
                defaultValue: 0m);
        }
    }
}
