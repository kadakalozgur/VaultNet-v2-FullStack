using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VaultNetAPIv2.Models
{

    [Index(nameof(UserId))]
    public class VaultItem
    {

        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public Guid UserId { get; set; }

        [Required(ErrorMessage = "Title is required.")]
        [MaxLength(1000, ErrorMessage = "Title can be a maximum of 1000 characters long.")]
        public string EncryptedTitle { get; set; }

        [Required(ErrorMessage = "An email address or username is required.")]
        [MaxLength(1000, ErrorMessage = "Email addresses or username can be a maximum of 1000 characters long.")]
        public string EncryptedUsernameOrEmail { get; set; }

        [Required(ErrorMessage = "A password is required.")]
        [MaxLength(1000, ErrorMessage = "Passwords can be a maximum of 1000 characters long.")]
        public string EncryptedPassword { get; set; }

        [MaxLength(1000, ErrorMessage = "The domain name can be a maximum of 1000 characters long.")]
        public string EncryptedDomain { get; set; }

        public bool IsFavorite { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey("UserId")]
        public User User { get; set; }

        public Guid? CategoryId { get; set; }

    }
}
