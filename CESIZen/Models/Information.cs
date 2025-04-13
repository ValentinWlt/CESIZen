using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CESIZen.Models  // Added namespace declaration
{
    public class Information
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Le titre est obligatoire")]
        [StringLength(100, ErrorMessage = "Le titre ne peut pas dépasser 100 caractères")]
        [Display(Name = "Titre")]
        public required string Titre { get; set; }

        [Required(ErrorMessage = "Le contenu est obligatoire")]
        [Display(Name = "Contenu")]
        [DataType(DataType.MultilineText)]
        public required string Contenu { get; set; }

        [Required]
        [Display(Name = "Date de création")]
        [DataType(DataType.Date)]
        public DateTime DateCreation { get; set; }

        [Display(Name = "Catégorie")]
        public string? Categorie { get; set; }

        [Display(Name = "Publié")]
        public bool EstPublie { get; set; } = true;

        [Display(Name = "Ordre d'affichage")]
        public int OrdreAffichage { get; set; } = 999;

        public virtual ICollection<Utilisateur> Utilisateurs { get; set; } = new List<Utilisateur>();

        [NotMapped]
        public string Resume => Contenu.Length > 150
            ? Contenu.Substring(0, 147) + "..."
            : Contenu;
    }
}
