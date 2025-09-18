using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProductManagement.Migrations
{
    /// <inheritdoc />
    public partial class AddPerformanceIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_products_brand_price",
                table: "products",
                columns: new[] { "brand", "price" });

            migrationBuilder.CreateIndex(
                name: "IX_products_category_brand",
                table: "products",
                columns: new[] { "category", "brand" });

            migrationBuilder.CreateIndex(
                name: "IX_products_category_price",
                table: "products",
                columns: new[] { "category", "price" });

            migrationBuilder.CreateIndex(
                name: "IX_products_createdat_category",
                table: "products",
                columns: new[] { "createdat", "category" });

            migrationBuilder.CreateIndex(
                name: "IX_products_description",
                table: "products",
                column: "description");

            migrationBuilder.CreateIndex(
                name: "IX_products_name_description_brand",
                table: "products",
                columns: new[] { "name", "description", "brand" });

            migrationBuilder.CreateIndex(
                name: "IX_products_price",
                table: "products",
                column: "price");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_products_brand_price",
                table: "products");

            migrationBuilder.DropIndex(
                name: "IX_products_category_brand",
                table: "products");

            migrationBuilder.DropIndex(
                name: "IX_products_category_price",
                table: "products");

            migrationBuilder.DropIndex(
                name: "IX_products_createdat_category",
                table: "products");

            migrationBuilder.DropIndex(
                name: "IX_products_description",
                table: "products");

            migrationBuilder.DropIndex(
                name: "IX_products_name_description_brand",
                table: "products");

            migrationBuilder.DropIndex(
                name: "IX_products_price",
                table: "products");
        }
    }
}
