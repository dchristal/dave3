using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace dave3.Migrations
{
    /// <inheritdoc />
    public partial class AddDiameter : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Controls_ControlName",
                table: "ControlObjects");

            migrationBuilder.AddColumn<float>(
                name: "Diameter",
                table: "Inventories",
                type: "real",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "ControlObjects",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Controls_ControlName",
                table: "ControlObjects",
                column: "Name",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Controls_ControlName",
                table: "ControlObjects");

            migrationBuilder.DropColumn(
                name: "Diameter",
                table: "Inventories");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "ControlObjects",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.CreateIndex(
                name: "IX_Controls_ControlName",
                table: "ControlObjects",
                column: "Name",
                unique: true,
                filter: "[Name] IS NOT NULL");
        }
    }
}
