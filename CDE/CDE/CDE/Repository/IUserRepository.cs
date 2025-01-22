using CDE.Models;
using Microsoft.AspNetCore.Identity;

namespace CDE.Repository
{
    public interface IUserRepository
    {
        Task<ApplicationUser> GetUserByIdAsync(string userId);
        Task<ApplicationUser> GetUserByEmailAsync(string email);
        Task<IEnumerable<ApplicationUser>> GetAllUsersAsync();
        Task<IdentityResult> CreateUserAsync(ApplicationUser user, string password);
        Task<IdentityResult> UpdateUserAsync(ApplicationUser user);
        Task<IdentityResult> DeleteUserAsync(ApplicationUser user);
        Task<IEnumerable<ApplicationUser>> SearchUsersAsync(string searchTerm);
        Task<IdentityResult> ResetPasswordAsync(string userId, string newPassword);
        //Task<IdentityResult> ImportUsersFromFileAsync(IEnumerable<UserImportModel> users);
    }
}
