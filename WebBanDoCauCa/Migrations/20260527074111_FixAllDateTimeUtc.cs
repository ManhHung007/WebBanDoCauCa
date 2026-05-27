using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebBanDoCauCa.Migrations
{
    /// <inheritdoc />
    public partial class FixAllDateTimeUtc : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Reviews",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone");

            migrationBuilder.AlterColumn<DateTime>(
                name: "OrderDate",
                table: "Orders",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DateCreated",
                table: "CartItems",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "b7237254-8c44-486a-85b4-7b4455589025",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "4db1bd13-b178-4117-a3bc-b83f6b58f149", "AQAAAAIAAYagAAAAEHBuG8TjfAh3Wp6JyCHvrrZGaghYxOxuk0QfiXmXqBBtITvLLnbSaKXDS9/JrD4aHA==", "35427aa8-b343-46b0-8ee3-8a15ecf3e086" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Reviews",
                type: "timestamp without time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<DateTime>(
                name: "OrderDate",
                table: "Orders",
                type: "timestamp without time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DateCreated",
                table: "CartItems",
                type: "timestamp without time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "b7237254-8c44-486a-85b4-7b4455589025",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "b5174814-ec6b-431e-8095-841de1c3bd88", "AQAAAAIAAYagAAAAEAx28jKmjSNci0uFuzPfYcl24+85bhfLotful6qmZ3lsBkQ+bHR3ubC4De0SDfdxSA==", "69e2f725-ba91-47e4-9c63-0876d8700fa2" });
        }
    }
}
