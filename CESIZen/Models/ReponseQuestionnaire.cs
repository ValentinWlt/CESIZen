using System.ComponentModel.DataAnnotations;

namespace CESIZen.Models
{
    public class ReponseQuestionnaire
    {
        public int Id { get; set; }

        [Required]
        public int? UtilisateurId { get; set; }

        [Display(Name = "Date de réponse")]
        public DateTime DateReponse { get; set; } = DateTime.Now;

        public Utilisateur? Utilisateur { get; set; }

        public virtual ICollection<QuestionnaireStress> EvenementsStress { get; set; } = new List<QuestionnaireStress>();
        public virtual ICollection<ReponseEvenement> ReponsesEvenement { get; set; } = new List<ReponseEvenement>();

    }
}
