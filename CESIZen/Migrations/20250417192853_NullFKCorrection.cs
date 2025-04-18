using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CESIZen.Migrations
{
    /// <inheritdoc />
    public partial class NullFKCorrection : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "UtilisateurId",
                table: "ReponsesQuestionnaire",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");
        }


        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "UtilisateurId",
                table: "ReponsesQuestionnaire",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);
        }

    }
}
