using CDE.Models;
using Microsoft.AspNetCore.Identity;

namespace CDE.Repository
{
    public interface IRoleRepository
    {
        Task<IdentityRole> GetRoleByIdAsync(string roleId);
        Task<IdentityRole> GetRoleByNameAsync(string roleName);
        Task<IEnumerable<IdentityRole>> GetAllRolesAsync();
        Task<IdentityResult> CreateRoleAsync(IdentityRole role);
        Task<IdentityResult> UpdateRoleAsync(IdentityRole role);
        Task<IdentityResult> DeleteRoleAsync(IdentityRole role);
        Task<IdentityResult> AssignRoleToUserAsync(ApplicationUser user, string roleName);
        Task<IdentityResult> RemoveRoleFromUserAsync(ApplicationUser user, string roleName);    
    }
}
