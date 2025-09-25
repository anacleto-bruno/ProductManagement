using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace ProductManagement.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "colors",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    hexcode = table.Column<string>(type: "character varying(7)", maxLength: 7, nullable: true),
                    createdat = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_colors", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "products",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    model = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    brand = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    sku = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    price = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    category = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    createdat = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updatedat = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_products", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "sizes",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    code = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    sortorder = table.Column<int>(type: "integer", nullable: true),
                    createdat = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sizes", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "productcolors",
                columns: table => new
                {
                    productid = table.Column<int>(type: "integer", nullable: false),
                    colorid = table.Column<int>(type: "integer", nullable: false),
                    createdat = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_productcolors", x => new { x.productid, x.colorid });
                    table.ForeignKey(
                        name: "FK_productcolors_colors_colorid",
                        column: x => x.colorid,
                        principalTable: "colors",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_productcolors_products_productid",
                        column: x => x.productid,
                        principalTable: "products",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "productsizes",
                columns: table => new
                {
                    productid = table.Column<int>(type: "integer", nullable: false),
                    sizeid = table.Column<int>(type: "integer", nullable: false),
                    stockquantity = table.Column<int>(type: "integer", nullable: true, defaultValue: 0),
                    createdat = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_productsizes", x => new { x.productid, x.sizeid });
                    table.ForeignKey(
                        name: "FK_productsizes_products_productid",
                        column: x => x.productid,
                        principalTable: "products",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_productsizes_sizes_sizeid",
                        column: x => x.sizeid,
                        principalTable: "sizes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_colors_name",
                table: "colors",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_productcolors_colorid",
                table: "productcolors",
                column: "colorid");

            migrationBuilder.CreateIndex(
                name: "IX_products_brand",
                table: "products",
                column: "brand");

            migrationBuilder.CreateIndex(
                name: "IX_products_brand_model",
                table: "products",
                columns: new[] { "brand", "model" });

            migrationBuilder.CreateIndex(
                name: "IX_products_category",
                table: "products",
                column: "category");

            migrationBuilder.CreateIndex(
                name: "IX_products_category_brand",
                table: "products",
                columns: new[] { "category", "brand" });

            migrationBuilder.CreateIndex(
                name: "IX_products_name",
                table: "products",
                column: "name");

            migrationBuilder.CreateIndex(
                name: "IX_products_price",
                table: "products",
                column: "price");

            migrationBuilder.CreateIndex(
                name: "IX_products_sku",
                table: "products",
                column: "sku",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_productsizes_sizeid",
                table: "productsizes",
                column: "sizeid");

            migrationBuilder.CreateIndex(
                name: "IX_sizes_name",
                table: "sizes",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_sizes_sortorder",
                table: "sizes",
                column: "sortorder");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "productcolors");

            migrationBuilder.DropTable(
                name: "productsizes");

            migrationBuilder.DropTable(
                name: "colors");

            migrationBuilder.DropTable(
                name: "products");

            migrationBuilder.DropTable(
                name: "sizes");
        }
    }
}
