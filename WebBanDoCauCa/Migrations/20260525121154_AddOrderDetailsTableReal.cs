using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebBanDoCauCa.Migrations
{
    /// <inheritdoc />
    public partial class AddOrderDetailsTableReal : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OrderDetails",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderId = table.Column<int>(type: "int", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderDetails_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrderDetails_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "b7237254-8c44-486a-85b4-7b4455589025",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "84b12670-025d-4265-a13a-b4172fca2195", "AQAAAAIAAYagAAAAEM4+QAJoWCtPZWCyzLRnnVdHK2BPAyktOnbuU9VnSBqdwBWgqhVv3n8lGjG13cub9Q==", "2f03e4e7-c71f-402c-980e-3d64c9fedb3d" });

            migrationBuilder.CreateIndex(
                name: "IX_OrderDetails_OrderId",
                table: "OrderDetails",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderDetails_ProductId",
                table: "OrderDetails",
                column: "ProductId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OrderDetails");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "b7237254-8c44-486a-85b4-7b4455589025",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "6c2da8c3-19da-4c64-aa02-badee4e7c9cf", "AQAAAAIAAYagAAAAECzFod2apu4GmlUVtrp9S8xUuO1Z+nu3990yM3o9D6jCrQzcri6yM9OBtEAYiz1uFg==", "45f9c542-eb6d-4ce5-9315-c3a29f89736f" });
        }
    }
}
