using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VaultNetAPIv2.Models
{

    [Index(nameof(UserId))]
    public class Category
    {

        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public Guid UserId { get; set; }

        [Required(ErrorMessage = "Category name is required.")]
        [MaxLength(1000, ErrorMessage = "Category name can be a maximum of 1000 characters long.")]
        public string EncryptedName { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey("UserId")]
        public User User { get; set; }

        public ICollection<VaultItem> VaultItems;

    }
}
