using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Northwind.Mvc.Data.Migrations
{
    /// <inheritdoc />
    public partial class FullCaterEasePlatform : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ActorEmail",
                table: "SystemLogs",
                type: "nvarchar(160)",
                maxLength: 160,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "SystemLogs",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CompletedUtc",
                table: "Orders",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeliveryAddress",
                table: "Orders",
                type: "nvarchar(240)",
                maxLength: 240,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EventNotes",
                table: "Orders",
                type: "nvarchar(600)",
                maxLength: 600,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CustomizationSummary",
                table: "OrderLines",
                type: "nvarchar(600)",
                maxLength: 600,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Latitude",
                table: "Customers",
                type: "decimal(9,6)",
                precision: 9,
                scale: 6,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Longitude",
                table: "Customers",
                type: "decimal(9,6)",
                precision: 9,
                scale: 6,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Caterers",
                type: "nvarchar(160)",
                maxLength: 160,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsApproved",
                table: "Caterers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Phone",
                table: "Caterers",
                type: "nvarchar(40)",
                maxLength: 40,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "AppUsers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FullName = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(160)", maxLength: 160, nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Role = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsTwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorCode = table.Column<string>(type: "nvarchar(12)", maxLength: 12, nullable: true),
                    TwoFactorExpiresUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CatererProfileId = table.Column<int>(type: "int", nullable: true),
                    CustomerProfileId = table.Column<int>(type: "int", nullable: true),
                    CreatedUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppUsers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppUsers_Caterers_CatererProfileId",
                        column: x => x.CatererProfileId,
                        principalTable: "Caterers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AppUsers_Customers_CustomerProfileId",
                        column: x => x.CustomerProfileId,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "AppUsers",
                columns: new[] { "Id", "CatererProfileId", "CreatedUtc", "CustomerProfileId", "Email", "FullName", "IsActive", "IsTwoFactorEnabled", "PasswordHash", "Role", "TwoFactorCode", "TwoFactorExpiresUtc" },
                values: new object[] { 1, null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "admin@caterease.local", "CaterEase Admin", true, false, "PBKDF2$100000$AAAAAAAAAAAAAAAAAAAAAA==$iMGgUv1EvOOAQO682i83Gg2X3pL2BniTeSFy0dFj/Vs=", "Admin", null, null });

            migrationBuilder.InsertData(
                table: "Caterers",
                columns: new[] { "Id", "AddressLine", "BrandStory", "City", "Email", "IsApproved", "Latitude", "Longitude", "Name", "Phone", "Slug" },
                values: new object[] { 1, "Besiktas, Istanbul", "Seasonal event catering with polished mezze tables, warm mains, and careful service.", "Istanbul", "caterer@caterease.local", true, 41.0430m, 29.0094m, "Istanbul Garden Catering", "+90 212 000 00 00", "istanbul-garden" });

            migrationBuilder.InsertData(
                table: "CuisineCategories",
                columns: new[] { "Id", "IconKey", "Name", "Slug" },
                values: new object[,]
                {
                    { 1, "briefcase", "Corporate Lunch", "corporate-lunch" },
                    { 2, "sparkles", "Wedding Table", "wedding-table" },
                    { 3, "leaf", "Vegan Feast", "vegan-feast" }
                });

            migrationBuilder.InsertData(
                table: "Customers",
                columns: new[] { "Id", "City", "DisplayName", "Email", "Latitude", "Longitude" },
                values: new object[] { 1, "Istanbul", "Demo User", "user@caterease.local", 41.0082m, 28.9784m });

            migrationBuilder.InsertData(
                table: "AppUsers",
                columns: new[] { "Id", "CatererProfileId", "CreatedUtc", "CustomerProfileId", "Email", "FullName", "IsActive", "IsTwoFactorEnabled", "PasswordHash", "Role", "TwoFactorCode", "TwoFactorExpiresUtc" },
                values: new object[,]
                {
                    { 2, 1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "caterer@caterease.local", "Istanbul Garden Manager", true, false, "PBKDF2$100000$AAAAAAAAAAAAAAAAAAAAAA==$iMGgUv1EvOOAQO682i83Gg2X3pL2BniTeSFy0dFj/Vs=", "Caterer", null, null },
                    { 3, null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, "user@caterease.local", "Demo User", true, true, "PBKDF2$100000$AAAAAAAAAAAAAAAAAAAAAA==$iMGgUv1EvOOAQO682i83Gg2X3pL2BniTeSFy0dFj/Vs=", "User", null, null }
                });

            migrationBuilder.InsertData(
                table: "MenuItems",
                columns: new[] { "Id", "BasePrice", "CategoryId", "CatererId", "Description", "ImagePath", "IsActive", "Name" },
                values: new object[,]
                {
                    { 1, 420m, 1, 1, "A polished mezze selection with warm flatbread, salads, and dips for office events.", "/images/menu/mezze.svg", true, "Executive Mezze Box" },
                    { 2, 780m, 2, 1, "Slow-roasted lamb with seasonal vegetables and event-ready plating.", "/images/menu/lamb.svg", true, "Rosemary Lamb Service" },
                    { 3, 360m, 3, 1, "Plant-forward mains, grains, and bright sauces for mixed guest lists.", "/images/menu/vegan.svg", true, "Green Garden Banquet" }
                });

            migrationBuilder.InsertData(
                table: "MenuItemOptionGroups",
                columns: new[] { "Id", "AllowsMultipleSelections", "IsRequired", "MenuItemId", "Title" },
                values: new object[,]
                {
                    { 1, true, false, 1, "Removable Ingredients" },
                    { 2, true, false, 1, "Optional Extras" },
                    { 3, false, true, 2, "Service Style" }
                });

            migrationBuilder.InsertData(
                table: "MenuItemOptions",
                columns: new[] { "Id", "IsRemovable", "OptionGroupId", "PriceDelta", "Title" },
                values: new object[,]
                {
                    { 1, true, 1, 0m, "Remove walnuts" },
                    { 2, true, 1, 0m, "Remove dairy sauce" },
                    { 3, false, 2, 95m, "Add dessert bites" },
                    { 4, false, 2, 120m, "Add premium drinks" },
                    { 5, false, 3, 0m, "Buffet setup" },
                    { 6, false, 3, 180m, "Plated service" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_AppUsers_CatererProfileId",
                table: "AppUsers",
                column: "CatererProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_AppUsers_CustomerProfileId",
                table: "AppUsers",
                column: "CustomerProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_AppUsers_Email",
                table: "AppUsers",
                column: "Email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppUsers");

            migrationBuilder.DeleteData(
                table: "Customers",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "MenuItemOptions",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "MenuItemOptions",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "MenuItemOptions",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "MenuItemOptions",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "MenuItemOptions",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "MenuItemOptions",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "MenuItems",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "CuisineCategories",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "MenuItemOptionGroups",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "MenuItemOptionGroups",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "MenuItemOptionGroups",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "MenuItems",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "MenuItems",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Caterers",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "CuisineCategories",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "CuisineCategories",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DropColumn(
                name: "ActorEmail",
                table: "SystemLogs");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "SystemLogs");

            migrationBuilder.DropColumn(
                name: "CompletedUtc",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "DeliveryAddress",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "EventNotes",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "CustomizationSummary",
                table: "OrderLines");

            migrationBuilder.DropColumn(
                name: "Latitude",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "Longitude",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "Caterers");

            migrationBuilder.DropColumn(
                name: "IsApproved",
                table: "Caterers");

            migrationBuilder.DropColumn(
                name: "Phone",
                table: "Caterers");
        }
    }
}
