using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CESIZen.Models
{
    public class Questionnaire
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Titre { get; set; }

        [StringLength(50)]
        public string Description { get; set; }

        [StringLength(50)]
        public string DateCreation { get; set; }

        [StringLength(50)]
        public string Statut { get; set; }

        [StringLength(50)]
        public string ResultatNiveauStress { get; set; }

        // Navigation properties
        public virtual ICollection<Question> Questions { get; set; } = new List<Question>();
        public virtual ICollection<Utilisateur> Utilisateurs { get; set; } = new List<Utilisateur>();
    }
}
