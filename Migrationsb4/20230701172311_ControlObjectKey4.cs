using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace dave3.Migrations
{
    /// <inheritdoc />
    public partial class ControlObjectKey4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Id",
                table: "ControlObjects");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "ControlObjects",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
