using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace WebBanDoCauCa.Migrations
{
    /// <inheritdoc />
    public partial class AddDataProtectionKeys : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DataProtectionKeys",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FriendlyName = table.Column<string>(type: "text", nullable: true),
                    Xml = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DataProtectionKeys", x => x.Id);
                });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "b7237254-8c44-486a-85b4-7b4455589025",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "9857fd27-44b6-4c3d-af80-000434baf637", "AQAAAAIAAYagAAAAEGwL3tOaVVDvMpHlGoDDsioRUPzHF7Jf90Hla/0e4/5eWaAcRKWZTB8pYrmrOCUNxw==", "edae3226-cf2d-45c2-aabb-e7852a08c900" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DataProtectionKeys");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "b7237254-8c44-486a-85b4-7b4455589025",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "4db1bd13-b178-4117-a3bc-b83f6b58f149", "AQAAAAIAAYagAAAAEHBuG8TjfAh3Wp6JyCHvrrZGaghYxOxuk0QfiXmXqBBtITvLLnbSaKXDS9/JrD4aHA==", "35427aa8-b343-46b0-8ee3-8a15ecf3e086" });
        }
    }
}
