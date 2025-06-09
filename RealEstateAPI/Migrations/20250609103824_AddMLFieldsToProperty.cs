using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RealEstateAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddMLFieldsToProperty : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Type",
                table: "Properties",
                newName: "NearbyFacility");

            migrationBuilder.AddColumn<int>(
                name: "BuildingAge",
                table: "Properties",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "Price2023",
                table: "Properties",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Price2024",
                table: "Properties",
                type: "decimal(18,2)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BuildingAge",
                table: "Properties");

            migrationBuilder.DropColumn(
                name: "Price2023",
                table: "Properties");

            migrationBuilder.DropColumn(
                name: "Price2024",
                table: "Properties");

            migrationBuilder.RenameColumn(
                name: "NearbyFacility",
                table: "Properties",
                newName: "Type");
        }
    }
}
