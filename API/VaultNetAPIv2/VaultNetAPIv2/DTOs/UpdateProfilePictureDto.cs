using System.ComponentModel.DataAnnotations;

namespace VaultNetAPIv2.DTOs
{
    public class UpdateProfilePictureDto
    {

        [Required(ErrorMessage = "A profile picture is required.")]
        public IFormFile Photo { get; set; }

    }
}
