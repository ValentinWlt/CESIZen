using System.ComponentModel.DataAnnotations;

namespace CESIZen.Models.ViewModels
{

    public class ChangePasswordViewModel
    {
        [Required, DataType(DataType.Password)]
        public string OldPassword { get; set; }

        [Required, DataType(DataType.Password)]
        public string NewPassword { get; set; }

        [Required, DataType(DataType.Password), Compare("NewPassword")]
        public string ConfirmPassword { get; set; }
    }
}