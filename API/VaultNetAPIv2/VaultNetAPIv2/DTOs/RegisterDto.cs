using System.ComponentModel.DataAnnotations;

namespace VaultNetAPIv2.DTOs
{
    public class RegisterDto
    {

        [Required(ErrorMessage = "An email address is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address format.")]
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", ErrorMessage = "Invalid email address format.")]
        [MaxLength(150, ErrorMessage = "Email addresses can be a maximum of 150 characters long.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "A password is required.")]
        [MinLength(8, ErrorMessage = "Password must be at least 8 characters long.")]
        [MaxLength(64, ErrorMessage = "Passwords can be a maximum of 64 characters long.")]
        public string Password { get; set; }

    }
}
