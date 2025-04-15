using System.ComponentModel.DataAnnotations;

namespace CESIZen.Models.ViewModels { 

public class ManageAccountViewModel
{
    [Required]
    public string Nom { get; set; }

    [Required]
    public string Prenom { get; set; }

    [Required, EmailAddress]
    public string Email { get; set; }

    [Phone]
    public string PhoneNumber { get; set; }
}
}