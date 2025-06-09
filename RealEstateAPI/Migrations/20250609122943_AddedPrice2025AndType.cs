using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RealEstateAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddedPrice2025AndType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Price2024",
                table: "Properties",
                newName: "Price_2024");

            migrationBuilder.RenameColumn(
                name: "Price2023",
                table: "Properties",
                newName: "Price_2023");

            migrationBuilder.RenameColumn(
                name: "Price",
                table: "Properties",
                newName: "Price_2025");

            migrationBuilder.RenameColumn(
                name: "Floor",
                table: "Properties",
                newName: "Floor_Level");

            migrationBuilder.AlterColumn<decimal>(
                name: "Price_2024",
                table: "Properties",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "Price_2023",
                table: "Properties",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "Properties",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Type",
                table: "Properties");

            migrationBuilder.RenameColumn(
                name: "Price_2024",
                table: "Properties",
                newName: "Price2024");

            migrationBuilder.RenameColumn(
                name: "Price_2023",
                table: "Properties",
                newName: "Price2023");

            migrationBuilder.RenameColumn(
                name: "Price_2025",
                table: "Properties",
                newName: "Price");

            migrationBuilder.RenameColumn(
                name: "Floor_Level",
                table: "Properties",
                newName: "Floor");

            migrationBuilder.AlterColumn<decimal>(
                name: "Price2024",
                table: "Properties",
                type: "decimal(18,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Price2023",
                table: "Properties",
                type: "decimal(18,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");
        }
    }
}
