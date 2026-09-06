using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace VaultNetAPIv2.Models
{

    [Index(nameof(Email), IsUnique = true)]
    public class User
    {

        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required(ErrorMessage = "An email address is required.")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
        [MaxLength(150,ErrorMessage = "Email addresses can be a maximum of 150 characters long.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "A password is required.")]
        [MaxLength(60, ErrorMessage = "Passwords can be a maximum of 60 characters long.")]
        public string PasswordHash { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [MaxLength(2048, ErrorMessage = "Profile url can be a maximum of 2048 characters long.")]
        public string? ProfilePictureUrl { get; set; }

    }
}
