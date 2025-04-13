using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace CESIZen.Models
{
    public class Droit
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string TypeDroit { get; set; }

        // Navigation properties
        public virtual ICollection<Role> Roles { get; set; } = new List<Role>();
    }
}
