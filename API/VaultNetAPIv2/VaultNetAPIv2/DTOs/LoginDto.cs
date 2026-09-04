using System.ComponentModel.DataAnnotations;

namespace VaultNetAPIv2.DTOs
{
    public class LoginDto
    {

        [Required(ErrorMessage = "An email address is required.")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "A password is required.")]
        public string Password { get; set; }

    }
}
