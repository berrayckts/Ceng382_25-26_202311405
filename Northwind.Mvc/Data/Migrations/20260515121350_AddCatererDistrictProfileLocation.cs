using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Northwind.Mvc.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddCatererDistrictProfileLocation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "District",
                table: "Caterers",
                type: "nvarchar(80)",
                maxLength: 80,
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Caterers",
                keyColumn: "Id",
                keyValue: 1,
                column: "District",
                value: "Besiktas");

            migrationBuilder.UpdateData(
                table: "Caterers",
                keyColumn: "Id",
                keyValue: 101,
                column: "District",
                value: "Beyoglu");

            migrationBuilder.UpdateData(
                table: "Caterers",
                keyColumn: "Id",
                keyValue: 102,
                column: "District",
                value: "Sisli");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "District",
                table: "Caterers");
        }
    }
}
