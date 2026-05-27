using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebBanDoCauCa.Migrations
{
    /// <inheritdoc />
    public partial class UpdateOrderModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "b7237254-8c44-486a-85b4-7b4455589025",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "3f58a000-f1a2-488f-9198-7c3b2d29345f", "AQAAAAIAAYagAAAAED0A/CjHR0OmlLLFLBY8n2LU7Wd3MioIY+T2mkKumVCBo7LLmuY4rBDXjfg82Ipveg==", "db594a71-b865-4933-ae6d-b9f4f53df4ae" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "b7237254-8c44-486a-85b4-7b4455589025",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "01b4a3eb-c35d-4169-82e8-7791323629cf", "AQAAAAIAAYagAAAAEF58jaseKMeKc8D1JIKt5VzHMjy28SYuO5oWl2tqvJMPPWf7uDa4Gj08uV6iq1cj0A==", "30e45a20-a4c0-4675-be48-4b4c9a87aa78" });
        }
    }
}
