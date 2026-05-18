using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Northwind.Mvc.Data.Migrations
{
    /// <inheritdoc />
    public partial class ReplaceGooglePlacesWithLeafletLocations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PlaceCatererMaps");

            migrationBuilder.InsertData(
                table: "Caterers",
                columns: new[] { "Id", "AddressLine", "BrandStory", "City", "Email", "IsApproved", "Latitude", "Longitude", "Name", "Phone", "Slug" },
                values: new object[,]
                {
                    { 101, "Galata, Beyoglu, Istanbul", "Golden-hour tasting menus and refined corporate catering near Galata.", "Istanbul", "galata@catera.local", true, 41.0256m, 28.9744m, "Galata Banquet Studio", "+90 212 111 22 33", "galata-banquet-studio" },
                    { 102, "Nisantasi, Sisli, Istanbul", "Boutique dinner service and elegant celebration menus around Nisantasi.", "Istanbul", "nisantasi@catera.local", true, 41.0504m, 28.9910m, "Nisantasi Private Table", "+90 212 222 44 55", "nisantasi-private-table" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Caterers",
                keyColumn: "Id",
                keyValue: 101);

            migrationBuilder.DeleteData(
                table: "Caterers",
                keyColumn: "Id",
                keyValue: 102);

            migrationBuilder.CreateTable(
                name: "PlaceCatererMaps",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CatererId = table.Column<int>(type: "int", nullable: false),
                    GooglePlaceId = table.Column<string>(type: "nvarchar(160)", maxLength: 160, nullable: false),
                    LinkedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlaceCatererMaps", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PlaceCatererMaps_Caterers_CatererId",
                        column: x => x.CatererId,
                        principalTable: "Caterers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PlaceCatererMaps_CatererId",
                table: "PlaceCatererMaps",
                column: "CatererId");

            migrationBuilder.CreateIndex(
                name: "IX_PlaceCatererMaps_GooglePlaceId",
                table: "PlaceCatererMaps",
                column: "GooglePlaceId",
                unique: true);
        }
    }
}
