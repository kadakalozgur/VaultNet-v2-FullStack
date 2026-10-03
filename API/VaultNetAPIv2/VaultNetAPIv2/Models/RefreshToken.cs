using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VaultNetAPIv2.Models
{

    [Index(nameof(UserId))]
    [Index(nameof(TokenHash), IsUnique = true)]
    public class RefreshToken
    {

        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public Guid UserId { get; set; }

        [Required]
        [MaxLength(128)]
        public string TokenHash { get; set; } = string.Empty;

        // Tokenın kendi ömrü
        public DateTime ExpiresAt { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Kullanıcı çıkış yaparsa tokenı iptal ederse.
        public DateTime? RevokedAt { get; set; }

        [ForeignKey("UserId")]
        public User User { get; set; } = null!;

    }
}
