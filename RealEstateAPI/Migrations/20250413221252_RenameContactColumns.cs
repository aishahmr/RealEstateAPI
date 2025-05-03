using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RealEstateAPI.Migrations
{
    /// <inheritdoc />
    public partial class RenameContactColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
            name: "ContactName",
            table: "Properties",
            newName: "yourName");

            migrationBuilder.RenameColumn(
                name: "ContactPhone",
                table: "Properties",
                newName: "MobilePhone");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
           name: "yourName",
           table: "Properties",
           newName: "ContactName");

            migrationBuilder.RenameColumn(
                name: "MobilePhone",
                table: "Properties",
                newName: "ContactPhone");
        }
    }
}
