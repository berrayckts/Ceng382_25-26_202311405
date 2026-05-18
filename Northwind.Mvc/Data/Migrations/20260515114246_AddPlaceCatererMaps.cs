using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Northwind.Mvc.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddPlaceCatererMaps : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PlaceCatererMaps",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GooglePlaceId = table.Column<string>(type: "nvarchar(160)", maxLength: 160, nullable: false),
                    CatererId = table.Column<int>(type: "int", nullable: false),
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PlaceCatererMaps");
        }
    }
}
