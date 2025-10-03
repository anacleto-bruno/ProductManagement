using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ProductManagement.Migrations
{
    /// <inheritdoc />
    public partial class AddSeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameIndex(
                name: "IX_sizes_sortorder",
                table: "sizes",
                newName: "idx_sizes_sortorder");

            migrationBuilder.RenameIndex(
                name: "IX_sizes_name",
                table: "sizes",
                newName: "idx_sizes_name");

            migrationBuilder.RenameIndex(
                name: "IX_colors_name",
                table: "colors",
                newName: "idx_colors_name");

            migrationBuilder.AlterColumn<int>(
                name: "sortorder",
                table: "sizes",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "hexcode",
                table: "colors",
                type: "character varying(7)",
                maxLength: 7,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(7)",
                oldMaxLength: 7,
                oldNullable: true);

            migrationBuilder.InsertData(
                table: "colors",
                columns: new[] { "id", "createdat", "hexcode", "name" },
                values: new object[,]
                {
                    { 1, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "#FF0000", "Red" },
                    { 2, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "#0000FF", "Blue" },
                    { 3, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "#008000", "Green" },
                    { 4, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "#FFFF00", "Yellow" },
                    { 5, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "#FFA500", "Orange" },
                    { 6, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "#800080", "Purple" },
                    { 7, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "#FFC0CB", "Pink" },
                    { 8, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "#A52A2A", "Brown" },
                    { 9, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "#000000", "Black" },
                    { 10, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "#FFFFFF", "White" },
                    { 11, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "#808080", "Gray" },
                    { 12, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "#000080", "Navy" },
                    { 13, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "#800000", "Maroon" },
                    { 14, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "#008080", "Teal" },
                    { 15, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "#C0C0C0", "Silver" }
                });

            migrationBuilder.InsertData(
                table: "sizes",
                columns: new[] { "id", "code", "createdat", "name", "sortorder" },
                values: new object[,]
                {
                    { 1, null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "XS", 1 },
                    { 2, null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "S", 2 },
                    { 3, null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "M", 3 },
                    { 4, null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "L", 4 },
                    { 5, null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "XL", 5 },
                    { 6, null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "XXL", 6 },
                    { 7, null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "XXXL", 7 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "colors",
                keyColumn: "id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "colors",
                keyColumn: "id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "colors",
                keyColumn: "id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "colors",
                keyColumn: "id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "colors",
                keyColumn: "id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "colors",
                keyColumn: "id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "colors",
                keyColumn: "id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "colors",
                keyColumn: "id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "colors",
                keyColumn: "id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "colors",
                keyColumn: "id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "colors",
                keyColumn: "id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "colors",
                keyColumn: "id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "colors",
                keyColumn: "id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "colors",
                keyColumn: "id",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "colors",
                keyColumn: "id",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "sizes",
                keyColumn: "id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "sizes",
                keyColumn: "id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "sizes",
                keyColumn: "id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "sizes",
                keyColumn: "id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "sizes",
                keyColumn: "id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "sizes",
                keyColumn: "id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "sizes",
                keyColumn: "id",
                keyValue: 7);

            migrationBuilder.RenameIndex(
                name: "idx_sizes_sortorder",
                table: "sizes",
                newName: "IX_sizes_sortorder");

            migrationBuilder.RenameIndex(
                name: "idx_sizes_name",
                table: "sizes",
                newName: "IX_sizes_name");

            migrationBuilder.RenameIndex(
                name: "idx_colors_name",
                table: "colors",
                newName: "IX_colors_name");

            migrationBuilder.AlterColumn<int>(
                name: "sortorder",
                table: "sizes",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<string>(
                name: "hexcode",
                table: "colors",
                type: "character varying(7)",
                maxLength: 7,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(7)",
                oldMaxLength: 7);
        }
    }
}
