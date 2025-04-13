using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace CESIZen.Models
{
    public class Utilisateur : IdentityUser<int>
    {

        [Required]
        [StringLength(50)]
        public string Nom { get; set; }

        [Required]
        [StringLength(50)]
        public string Prenom { get; set; }

        [StringLength(50)]
        public string Statut { get; set; }

        [NotMapped] // Cette annotation indique que la propriété n'est pas stockée en base de données
        public string Mail
        {
            get => Email;
            set => Email = value;
        }

        [NotMapped]
        public string Tel
        {
            get => PhoneNumber;
            set => PhoneNumber = value;
        }

        [ForeignKey("Role")]
        public int? IdRole { get; set; }
        public virtual Role? Role { get; set; }

        public virtual ICollection<Information> Informations { get; set; } = new List<Information>();
        public virtual ICollection<Questionnaire> Questionnaires { get; set; } = new List<Questionnaire>();
    }
}
