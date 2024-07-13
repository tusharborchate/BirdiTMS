using System.ComponentModel.DataAnnotations;

namespace BirdiTMS.Models.Entities
{
    public class LoginUser
    {
        [Required(ErrorMessage = "User Name is required")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }
    }
}
