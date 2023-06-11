using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace dave3.Migrations
{
    /// <inheritdoc />
    public partial class AddControls : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Controls",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MyString = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MyInt = table.Column<int>(type: "int", nullable: false),
                    MyFloat = table.Column<float>(type: "real", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Controls", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Controls");
        }
    }
}
