using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Northwind.Mvc.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddMenuOptionSelectionMetadata : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsAvailable",
                table: "MenuItemOptions",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDefault",
                table: "MenuItemOptions",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "MaxSelection",
                table: "MenuItemOptionGroups",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MinSelection",
                table: "MenuItemOptionGroups",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "MenuItemOptionGroups",
                keyColumn: "Id",
                keyValue: 1,
                column: "MaxSelection",
                value: 3);

            migrationBuilder.UpdateData(
                table: "MenuItemOptionGroups",
                keyColumn: "Id",
                keyValue: 2,
                column: "MaxSelection",
                value: 3);

            migrationBuilder.UpdateData(
                table: "MenuItemOptionGroups",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "MaxSelection", "MinSelection" },
                values: new object[] { 1, 1 });

            migrationBuilder.UpdateData(
                table: "MenuItemOptions",
                keyColumn: "Id",
                keyValue: 1,
                column: "IsAvailable",
                value: true);

            migrationBuilder.UpdateData(
                table: "MenuItemOptions",
                keyColumn: "Id",
                keyValue: 2,
                column: "IsAvailable",
                value: true);

            migrationBuilder.UpdateData(
                table: "MenuItemOptions",
                keyColumn: "Id",
                keyValue: 3,
                column: "IsAvailable",
                value: true);

            migrationBuilder.UpdateData(
                table: "MenuItemOptions",
                keyColumn: "Id",
                keyValue: 4,
                column: "IsAvailable",
                value: true);

            migrationBuilder.UpdateData(
                table: "MenuItemOptions",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "IsAvailable", "IsDefault" },
                values: new object[] { true, true });

            migrationBuilder.UpdateData(
                table: "MenuItemOptions",
                keyColumn: "Id",
                keyValue: 6,
                column: "IsAvailable",
                value: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsAvailable",
                table: "MenuItemOptions");

            migrationBuilder.DropColumn(
                name: "IsDefault",
                table: "MenuItemOptions");

            migrationBuilder.DropColumn(
                name: "MaxSelection",
                table: "MenuItemOptionGroups");

            migrationBuilder.DropColumn(
                name: "MinSelection",
                table: "MenuItemOptionGroups");
        }
    }
}
