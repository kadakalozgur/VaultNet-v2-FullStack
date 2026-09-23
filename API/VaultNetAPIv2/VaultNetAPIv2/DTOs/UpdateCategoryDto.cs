using System.ComponentModel.DataAnnotations;

namespace VaultNetAPIv2.DTOs
{
    public class UpdateCategoryDto
    {

        [Required(ErrorMessage = "Category name is required.")]
        [MaxLength(100, ErrorMessage = "Category name can be a maximum of 100 characters long.")]
        public string CategoryName { get; set; }

    }
}
