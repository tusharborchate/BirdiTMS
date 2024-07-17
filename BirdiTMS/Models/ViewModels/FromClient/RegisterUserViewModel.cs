using System.ComponentModel.DataAnnotations;

namespace BirdiTMS.Models.ViewModels.FromClient
{
    public class RegisterUserViewModel
    {
        
        [EmailAddress]
        [Required(ErrorMessage = "Email is required")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }
    }
}
