using System.ComponentModel.DataAnnotations;

namespace VaultNetAPIv2.DTOs
{
    public class ChangePasswordDto
    {

        [Required(ErrorMessage = "A current password is required.")]
        public string CurrentPassword { get; set; }

        [Required(ErrorMessage = "A new password is required.")]
        [MinLength(8, ErrorMessage = "Password must be at least 8 characters long.")]
        [MaxLength(64, ErrorMessage = "Passwords can be a maximum of 64 characters long.")]
        public string NewPassword { get; set; }

    }
}
