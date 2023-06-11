using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace dave3.Migrations
{
    /// <inheritdoc />
    public partial class AddControls4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Controls");

            migrationBuilder.CreateTable(
                name: "ControlObjects",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    ControlString = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ControlInt = table.Column<int>(type: "int", nullable: true),
                    ControlFloat = table.Column<float>(type: "real", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ControlObjects", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Controls_ControlName",
                table: "ControlObjects",
                column: "Name",
                unique: true,
                filter: "[Name] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ControlObjects");

            migrationBuilder.CreateTable(
                name: "Controls",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MyFloat = table.Column<float>(type: "real", nullable: false),
                    MyInt = table.Column<int>(type: "int", nullable: false),
                    MyString = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Controls", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Controls_ControlName",
                table: "Controls",
                column: "Name",
                unique: true,
                filter: "[Name] IS NOT NULL");
        }
    }
}
