using Microsoft.EntityFrameworkCore.Migrations;

namespace Backend_MVC_Layihe.Migrations
{
    public partial class UpdatedCartItemName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BasketItems");

            migrationBuilder.CreateTable(
                name: "CartItems",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClothesId = table.Column<int>(nullable: false),
                    Price = table.Column<decimal>(type: "decimal(6,2)", nullable: false),
                    Quantity = table.Column<byte>(nullable: false),
                    ColorId = table.Column<int>(nullable: false),
                    SizeId = table.Column<int>(nullable: false),
                    AppUserId = table.Column<string>(nullable: true),
                    OrderId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CartItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CartItems_AspNetUsers_AppUserId",
                        column: x => x.AppUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CartItems_Clothes_ClothesId",
                        column: x => x.ClothesId,
                        principalTable: "Clothes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CartItems_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CartItems_AppUserId",
                table: "CartItems",
                column: "AppUserId");

            migrationBuilder.CreateIndex(
                name: "IX_CartItems_ClothesId",
                table: "CartItems",
                column: "ClothesId");

            migrationBuilder.CreateIndex(
                name: "IX_CartItems_OrderId",
                table: "CartItems",
                column: "OrderId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CartItems");

            migrationBuilder.CreateTable(
                name: "BasketItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AppUserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    ClothesId = table.Column<int>(type: "int", nullable: false),
                    ColorId = table.Column<int>(type: "int", nullable: false),
                    OrderId = table.Column<int>(type: "int", nullable: true),
                    Price = table.Column<decimal>(type: "decimal(6,2)", nullable: false),
                    Quantity = table.Column<byte>(type: "tinyint", nullable: false),
                    SizeId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BasketItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BasketItems_AspNetUsers_AppUserId",
                        column: x => x.AppUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BasketItems_Clothes_ClothesId",
                        column: x => x.ClothesId,
                        principalTable: "Clothes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BasketItems_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BasketItems_AppUserId",
                table: "BasketItems",
                column: "AppUserId");

            migrationBuilder.CreateIndex(
                name: "IX_BasketItems_ClothesId",
                table: "BasketItems",
                column: "ClothesId");

            migrationBuilder.CreateIndex(
                name: "IX_BasketItems_OrderId",
                table: "BasketItems",
                column: "OrderId");
        }
    }
}
