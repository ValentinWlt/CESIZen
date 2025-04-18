using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace CESIZen.Migrations
{
    /// <inheritdoc />
    public partial class NullFK : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Questionnaires",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Questionnaires",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Questionnaires",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Questionnaires",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Questionnaires",
                keyColumn: "Id",
                keyValue: 5);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Questionnaires",
                columns: new[] { "Id", "Libelle", "ReponseQuestionnaireId", "Valeur" },
                values: new object[,]
                {
                    { 1, "Décès du conjoint", null, 100 },
                    { 2, "Divorce", null, 73 },
                    { 3, "Séparation", null, 65 },
                    { 4, "Prison", null, 63 },
                    { 5, "Mort d'un proche", null, 63 }
                });
        }
    }
}
