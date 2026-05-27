using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebBanDoCauCa.Migrations
{
    /// <inheritdoc />
    public partial class AddIsHotToProduct : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsHot",
                table: "Products",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "b7237254-8c44-486a-85b4-7b4455589025",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "01b4a3eb-c35d-4169-82e8-7791323629cf", "AQAAAAIAAYagAAAAEF58jaseKMeKc8D1JIKt5VzHMjy28SYuO5oWl2tqvJMPPWf7uDa4Gj08uV6iq1cj0A==", "30e45a20-a4c0-4675-be48-4b4c9a87aa78" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsHot",
                table: "Products");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "b7237254-8c44-486a-85b4-7b4455589025",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "3047efed-4a7e-403e-9e06-f0fb7cbd6031", "AQAAAAIAAYagAAAAEFG0lWd0dSFbtZBE7ad0dRLvSl9NLX1qP2QwQnahX+GsEB5X1O7HRvcxHtyPeZ6fMQ==", "3d205813-bfff-432b-b69f-fa3635558d06" });
        }
    }
}
