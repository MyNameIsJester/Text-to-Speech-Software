using Microsoft.AspNetCore.Identity;

namespace AudioGuideAdmin.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string? FullName { get; set; }
    }
}