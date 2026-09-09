using System.ComponentModel.DataAnnotations;

namespace VaultNetAPIv2.DTOs
{
    public class DeleteAccountDto
    {

        [Required(ErrorMessage = "A current password is required.")]
        public string CurrentPassword { get; set; }

    }
}
