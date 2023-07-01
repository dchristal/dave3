using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace dave3.Migrations
{
    /// <inheritdoc />
    public partial class ControlObjectKey3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_ControlObjects",
                table: "ControlObjects");

            migrationBuilder.DropIndex(
                name: "IX_Controls_ControlName",
                table: "ControlObjects");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "ControlObjects",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .OldAnnotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ControlObjects",
                table: "ControlObjects",
                column: "Name");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_ControlObjects",
                table: "ControlObjects");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "ControlObjects",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ControlObjects",
                table: "ControlObjects",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Controls_ControlName",
                table: "ControlObjects",
                column: "Name",
                unique: true);
        }
    }
}
