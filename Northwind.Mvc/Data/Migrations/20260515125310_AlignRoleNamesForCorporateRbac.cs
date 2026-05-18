using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Northwind.Mvc.Data.Migrations
{
    /// <inheritdoc />
    public partial class AlignRoleNamesForCorporateRbac : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AppUsers",
                keyColumn: "Id",
                keyValue: 2,
                column: "Role",
                value: "RestaurantOwner");

            migrationBuilder.UpdateData(
                table: "AppUsers",
                keyColumn: "Id",
                keyValue: 3,
                column: "Role",
                value: "Customer");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AppUsers",
                keyColumn: "Id",
                keyValue: 2,
                column: "Role",
                value: "Caterer");

            migrationBuilder.UpdateData(
                table: "AppUsers",
                keyColumn: "Id",
                keyValue: 3,
                column: "Role",
                value: "User");
        }
    }
}
