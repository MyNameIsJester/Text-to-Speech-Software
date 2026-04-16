using System.ComponentModel.DataAnnotations;

namespace AudioGuideAdmin.ViewModels.Users
{
    public class CreateUserViewModel
    {
        [Required]
        public string FullName { get; set; } = "";

        [Required]
        [EmailAddress]
        public string Email { get; set; } = "";

        [Required]
        [DataType(DataType.Password)]
        [MinLength(6)]
        public string Password { get; set; } = "";

        [Required]
        public string RoleName { get; set; } = "";
    }
}