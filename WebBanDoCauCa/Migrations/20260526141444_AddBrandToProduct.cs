using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebBanDoCauCa.Migrations
{
    /// <inheritdoc />
    public partial class AddBrandToProduct : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Brand",
                table: "Products",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "b7237254-8c44-486a-85b4-7b4455589025",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "ed16b641-1a9c-41b4-9b44-88a9575a1503", "AQAAAAIAAYagAAAAEJ82BSckD3ogrv///kewOXlNLFfzJvH0IrQcZ8eHQdyS4Bhc+aWtrx1kVnaO/8sRxw==", "4d8f8ba3-d87a-490b-bd50-4f32b8592514" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Brand",
                table: "Products");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "b7237254-8c44-486a-85b4-7b4455589025",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "3f58a000-f1a2-488f-9198-7c3b2d29345f", "AQAAAAIAAYagAAAAED0A/CjHR0OmlLLFLBY8n2LU7Wd3MioIY+T2mkKumVCBo7LLmuY4rBDXjfg82Ipveg==", "db594a71-b865-4933-ae6d-b9f4f53df4ae" });
        }
    }
}
