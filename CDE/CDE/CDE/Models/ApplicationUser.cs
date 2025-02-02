using Microsoft.AspNetCore.Identity;

namespace CDE.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiryTime { get; set; }
        public bool IsActive { get; set; } = true;
        public ICollection<IdentityUserRole<string>> UserRoles { get; set; }
    }
}
