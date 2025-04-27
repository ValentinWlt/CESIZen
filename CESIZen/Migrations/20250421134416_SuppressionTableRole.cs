using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CESIZen.Migrations
{
    /// <inheritdoc />
    public partial class SuppressionTableRole : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Roles_IdRole",
                table: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "Roles");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_IdRole",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "IdRole",
                table: "AspNetUsers");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "IdRole",
                table: "AspNetUsers",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NomRole = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_IdRole",
                table: "AspNetUsers",
                column: "IdRole");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Roles_IdRole",
                table: "AspNetUsers",
                column: "IdRole",
                principalTable: "Roles",
                principalColumn: "Id");
        }
    }
}
