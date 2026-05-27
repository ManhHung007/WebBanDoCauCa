using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebBanDoCauCa.Migrations
{
    /// <inheritdoc />
    public partial class UpdateIdentityToCustom : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "b7237254-8c44-486a-85b4-7b4455589025",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "0c3b0101-06cb-4ce0-8f60-82702751f309", "AQAAAAIAAYagAAAAEBsTbjaeibjFv58Mc68G0DTMm8S0ZGsNFkV1KUKrA0lba0co2BrBuIkKOV0kkfnkww==", "429ed86d-56db-4f41-a05e-77bac9504931" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "b7237254-8c44-486a-85b4-7b4455589025",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "ab1f44d4-98ab-4d21-bd44-a2de28451a00", "AQAAAAIAAYagAAAAEGOZNfT7HHhJJVJyN5nnHie3okkabBOAfD/ascg2ED6Ec7tL2W64BWCTNp30+vfBCg==", "23bdf941-7528-486b-a2d9-8ad8965175eb" });
        }
    }
}
