using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CESIZen.Migrations
{
    /// <inheritdoc />
    public partial class SuppressionTableDroit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Attribuer");

            migrationBuilder.DropTable(
                name: "Droits");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Droits",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TypeDroit = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Droits", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Attribuer",
                columns: table => new
                {
                    DroitsId = table.Column<int>(type: "int", nullable: false),
                    RolesId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Attribuer", x => new { x.DroitsId, x.RolesId });
                    table.ForeignKey(
                        name: "FK_Attribuer_Droits_DroitsId",
                        column: x => x.DroitsId,
                        principalTable: "Droits",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Attribuer_Roles_RolesId",
                        column: x => x.RolesId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Attribuer_RolesId",
                table: "Attribuer",
                column: "RolesId");
        }
    }
}
