using System.ComponentModel.DataAnnotations;

namespace CESIZen.Models.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "L'adresse e-mail est obligatoire")]
        [EmailAddress(ErrorMessage = "Adresse e-mail invalide")]
        [Display(Name = "Adresse e-mail")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Le mot de passe est obligatoire")]
        [DataType(DataType.Password)]
        [Display(Name = "Mot de passe")]
        public string Password { get; set; }

        [Display(Name = "Se souvenir de moi")]
        public bool RememberMe { get; set; }
    }
}
