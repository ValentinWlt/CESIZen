using System.ComponentModel.DataAnnotations;

namespace CESIZen.Models
{
    public class QuestionnaireStress
    {
        public int Id { get; set; }

        [Required]
        public string Libelle { get; set; }

        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "La valeur doit être positive.")]
        public int Valeur { get; set; }
    }
}
