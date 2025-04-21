using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CESIZen.Migrations
{
    /// <inheritdoc />
    public partial class SuppressionPropsInutileReponseEvt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EstSurvenu",
                table: "ReponsesEvenement");

            migrationBuilder.DropColumn(
                name: "ValeurPoints",
                table: "ReponsesEvenement");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "EstSurvenu",
                table: "ReponsesEvenement",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "ValeurPoints",
                table: "ReponsesEvenement",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
