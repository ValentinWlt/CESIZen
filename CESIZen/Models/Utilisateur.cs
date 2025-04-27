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

        [NotMapped]
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

        public virtual ICollection<Information> Informations { get; set; } = new List<Information>();
        public virtual ICollection<ReponseQuestionnaire> ReponsesQuestionnaire { get; set; } = new List<ReponseQuestionnaire>();
    }
}
