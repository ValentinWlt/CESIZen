namespace CESIZen.Models
{
    public class ReponseEvenement
    {
        public int Id { get; set; }

        public int ReponseQuestionnaireId { get; set; }
        public virtual ReponseQuestionnaire ReponseQuestionnaire { get; set; }

        public int QuestionnaireStressId { get; set; }
        public virtual QuestionnaireStress QuestionnaireStress { get; set; }

    }
}
