using System.ComponentModel.DataAnnotations;

namespace VaultNetAPIv2.DTOs
{
    public class LoginDto
    {

        [Required(ErrorMessage = "An email address is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address format.")]
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", ErrorMessage = "Invalid email address format.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "A password is required.")]
        public string Password { get; set; }

    }
}
