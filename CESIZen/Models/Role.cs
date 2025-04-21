using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CESIZen.Models
{
    public class Role
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string NomRole { get; set; }

        public virtual ICollection<Utilisateur> Utilisateurs { get; set; }
    }
}
