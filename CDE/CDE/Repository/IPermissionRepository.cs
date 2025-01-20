using CDE.Models;
using Microsoft.AspNetCore.Identity;

namespace CDE.Repository
{
    public interface IPermissionRepository
    {
        Task<IEnumerable<string>> GetPermissionsByUserAsync(ApplicationUser user);
        Task<IdentityResult> AssignPermissionToRoleAsync(string roleName, string permission);
        Task<IdentityResult> RemovePermissionFromRoleAsync(string roleName, string permission);
        Task<IEnumerable<string>> GetAllPermissionsAsync();
    }
}
