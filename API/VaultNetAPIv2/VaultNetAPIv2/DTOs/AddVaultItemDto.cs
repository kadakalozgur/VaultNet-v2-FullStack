using System.ComponentModel.DataAnnotations;

namespace VaultNetAPIv2.DTOs
{
    public class AddVaultItemDto
    {

        [Required(ErrorMessage = "Title is required.")]
        [MaxLength(100, ErrorMessage = "Title can be a maximum of 100 characters long.")]
        public string Title { get; set; }

        [Required(ErrorMessage = "An email address or username is required.")]
        [MaxLength(150, ErrorMessage = "Email addresses or username can be a maximum of 150 characters long.")]
        public string UsernameOrEmail { get; set; }

        [Required(ErrorMessage = "A password is required.")]
        [MaxLength(256, ErrorMessage = "Passwords can be a maximum of 256 characters long.")]
        public string Password { get; set; }

        [MaxLength(150, ErrorMessage = "The domain name can be a maximum of 150 characters long.")]
        public string Domain { get; set; }

        public Guid? CategoryId { get; set; }

    }
}
