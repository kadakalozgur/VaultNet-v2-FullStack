using System.ComponentModel.DataAnnotations;

namespace VaultNetAPIv2.DTOs
{
    public class UpdateEmailDto
    {

        [Required(ErrorMessage = "Email address is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address format.")]
        [MaxLength(150, ErrorMessage = "Email addresses can be a maximum of 150 characters long.")]
        public string NewEmail { get; set; }

    }
}
