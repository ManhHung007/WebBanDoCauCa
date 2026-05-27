using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebBanDoCauCa.Migrations
{
    /// <inheritdoc />
    public partial class UpdateOrderSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "b7237254-8c44-486a-85b4-7b4455589025",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "1c98b40e-f974-4d66-813c-a2952c8ad509", "AQAAAAIAAYagAAAAEDHt4EH4ibFhLcwfMO+D3vXzxiMGg2sE+99S5OxU99ilOaalM3pS9zVMKprmr/ibrg==", "34ba0044-ab1d-4d3d-88f1-ec20932d0d5e" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "b7237254-8c44-486a-85b4-7b4455589025",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "0c3b0101-06cb-4ce0-8f60-82702751f309", "AQAAAAIAAYagAAAAEBsTbjaeibjFv58Mc68G0DTMm8S0ZGsNFkV1KUKrA0lba0co2BrBuIkKOV0kkfnkww==", "429ed86d-56db-4f41-a05e-77bac9504931" });
        }
    }
}
