using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Northwind.Mvc.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddMenuOptionTypeMetadata : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "MenuItemOptions",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETUTCDATE()");

            migrationBuilder.AddColumn<string>(
                name: "OptionType",
                table: "MenuItemOptions",
                type: "nvarchar(24)",
                maxLength: 24,
                nullable: false,
                defaultValue: "Extra");

            migrationBuilder.AddColumn<string>(
                name: "QuantityInfo",
                table: "MenuItemOptions",
                type: "nvarchar(80)",
                maxLength: 80,
                nullable: true);

            migrationBuilder.UpdateData(
                table: "MenuItemOptions",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "OptionType", "QuantityInfo" },
                values: new object[] { new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Removable", null });

            migrationBuilder.UpdateData(
                table: "MenuItemOptions",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "OptionType", "QuantityInfo" },
                values: new object[] { new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Removable", null });

            migrationBuilder.UpdateData(
                table: "MenuItemOptions",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAt", "OptionType", "QuantityInfo" },
                values: new object[] { new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Extra", null });

            migrationBuilder.UpdateData(
                table: "MenuItemOptions",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CreatedAt", "OptionType", "QuantityInfo" },
                values: new object[] { new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Extra", null });

            migrationBuilder.UpdateData(
                table: "MenuItemOptions",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "CreatedAt", "OptionType", "QuantityInfo" },
                values: new object[] { new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Extra", null });

            migrationBuilder.UpdateData(
                table: "MenuItemOptions",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "CreatedAt", "OptionType", "QuantityInfo" },
                values: new object[] { new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Extra", null });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "MenuItemOptions");

            migrationBuilder.DropColumn(
                name: "OptionType",
                table: "MenuItemOptions");

            migrationBuilder.DropColumn(
                name: "QuantityInfo",
                table: "MenuItemOptions");
        }
    }
}
