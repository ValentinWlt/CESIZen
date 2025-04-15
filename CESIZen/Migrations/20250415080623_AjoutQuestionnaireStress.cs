using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CESIZen.Migrations
{
    /// <inheritdoc />
    public partial class AjoutQuestionnaireStress : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "QuestionnaireUtilisateur");

            migrationBuilder.DropTable(
                name: "QuestionQuestionnaire");

            migrationBuilder.DropTable(
                name: "Questions");

            migrationBuilder.DropColumn(
                name: "DateCreation",
                table: "Questionnaires");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Questionnaires");

            migrationBuilder.DropColumn(
                name: "ResultatNiveauStress",
                table: "Questionnaires");

            migrationBuilder.DropColumn(
                name: "Statut",
                table: "Questionnaires");

            migrationBuilder.DropColumn(
                name: "Titre",
                table: "Questionnaires");

            migrationBuilder.AddColumn<string>(
                name: "Libelle",
                table: "Questionnaires",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "ReponseQuestionnaireId",
                table: "Questionnaires",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Valeur",
                table: "Questionnaires",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "ReponsesQuestionnaire",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UtilisateurId = table.Column<int>(type: "int", nullable: false),
                    DateReponse = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReponsesQuestionnaire", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReponsesQuestionnaire_AspNetUsers_UtilisateurId",
                        column: x => x.UtilisateurId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ReponsesEvenement",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReponseQuestionnaireId = table.Column<int>(type: "int", nullable: false),
                    QuestionnaireStressId = table.Column<int>(type: "int", nullable: false),
                    EstSurvenu = table.Column<bool>(type: "bit", nullable: false),
                    ValeurPoints = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReponsesEvenement", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReponsesEvenement_Questionnaires_QuestionnaireStressId",
                        column: x => x.QuestionnaireStressId,
                        principalTable: "Questionnaires",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ReponsesEvenement_ReponsesQuestionnaire_ReponseQuestionnaireId",
                        column: x => x.ReponseQuestionnaireId,
                        principalTable: "ReponsesQuestionnaire",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Questionnaires_ReponseQuestionnaireId",
                table: "Questionnaires",
                column: "ReponseQuestionnaireId");

            migrationBuilder.CreateIndex(
                name: "IX_ReponsesEvenement_QuestionnaireStressId",
                table: "ReponsesEvenement",
                column: "QuestionnaireStressId");

            migrationBuilder.CreateIndex(
                name: "IX_ReponsesEvenement_ReponseQuestionnaireId",
                table: "ReponsesEvenement",
                column: "ReponseQuestionnaireId");

            migrationBuilder.CreateIndex(
                name: "IX_ReponsesQuestionnaire_UtilisateurId",
                table: "ReponsesQuestionnaire",
                column: "UtilisateurId");

            migrationBuilder.AddForeignKey(
                name: "FK_Questionnaires_ReponsesQuestionnaire_ReponseQuestionnaireId",
                table: "Questionnaires",
                column: "ReponseQuestionnaireId",
                principalTable: "ReponsesQuestionnaire",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Questionnaires_ReponsesQuestionnaire_ReponseQuestionnaireId",
                table: "Questionnaires");

            migrationBuilder.DropTable(
                name: "ReponsesEvenement");

            migrationBuilder.DropTable(
                name: "ReponsesQuestionnaire");

            migrationBuilder.DropIndex(
                name: "IX_Questionnaires_ReponseQuestionnaireId",
                table: "Questionnaires");

            migrationBuilder.DropColumn(
                name: "Libelle",
                table: "Questionnaires");

            migrationBuilder.DropColumn(
                name: "ReponseQuestionnaireId",
                table: "Questionnaires");

            migrationBuilder.DropColumn(
                name: "Valeur",
                table: "Questionnaires");

            migrationBuilder.AddColumn<string>(
                name: "DateCreation",
                table: "Questionnaires",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Questionnaires",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ResultatNiveauStress",
                table: "Questionnaires",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Statut",
                table: "Questionnaires",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Titre",
                table: "Questionnaires",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "QuestionnaireUtilisateur",
                columns: table => new
                {
                    QuestionnairesId = table.Column<int>(type: "int", nullable: false),
                    UtilisateursId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuestionnaireUtilisateur", x => new { x.QuestionnairesId, x.UtilisateursId });
                    table.ForeignKey(
                        name: "FK_QuestionnaireUtilisateur_AspNetUsers_UtilisateursId",
                        column: x => x.UtilisateursId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_QuestionnaireUtilisateur_Questionnaires_QuestionnairesId",
                        column: x => x.QuestionnairesId,
                        principalTable: "Questionnaires",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Questions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Points = table.Column<int>(type: "int", nullable: false),
                    Texte = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    TypeReponse = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Questions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "QuestionQuestionnaire",
                columns: table => new
                {
                    QuestionnairesId = table.Column<int>(type: "int", nullable: false),
                    QuestionsId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuestionQuestionnaire", x => new { x.QuestionnairesId, x.QuestionsId });
                    table.ForeignKey(
                        name: "FK_QuestionQuestionnaire_Questionnaires_QuestionnairesId",
                        column: x => x.QuestionnairesId,
                        principalTable: "Questionnaires",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_QuestionQuestionnaire_Questions_QuestionsId",
                        column: x => x.QuestionsId,
                        principalTable: "Questions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_QuestionnaireUtilisateur_UtilisateursId",
                table: "QuestionnaireUtilisateur",
                column: "UtilisateursId");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionQuestionnaire_QuestionsId",
                table: "QuestionQuestionnaire",
                column: "QuestionsId");
        }
    }
}
