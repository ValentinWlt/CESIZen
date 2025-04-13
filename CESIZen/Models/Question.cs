using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace CESIZen.Models
{
    public class Question
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Texte { get; set; }

        public int Points { get; set; }

        [StringLength(50)]
        public string TypeReponse { get; set; }

        // Navigation properties
        public virtual ICollection<Questionnaire> Questionnaires { get; set; } = new List<Questionnaire>();
    }
}
