using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Helios.Web.Models {
    public class ChangePasswordViewModel {
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }
}