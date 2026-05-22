using Microsoft.AspNetCore.Identity;

namespace SchoolApp.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; } = string.Empty;
        public string? ProfilePicturePath { get; set; }
    }
}
