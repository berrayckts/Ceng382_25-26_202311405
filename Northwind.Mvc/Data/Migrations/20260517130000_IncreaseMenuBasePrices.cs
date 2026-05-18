using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Northwind.Mvc.Models;

#nullable disable

namespace Northwind.Mvc.Data.Migrations
{
    [DbContext(typeof(CateringProjectContext))]
    [Migration("20260517130000_IncreaseMenuBasePrices")]
    public partial class IncreaseMenuBasePrices : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                UPDATE [MenuItems]
                SET [BasePrice] = [BasePrice] + 300
                """);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                UPDATE [MenuItems]
                SET [BasePrice] = CASE
                    WHEN [BasePrice] >= 300 THEN [BasePrice] - 300
                    ELSE 0
                END
                """);
        }
    }
}
