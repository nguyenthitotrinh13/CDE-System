using Microsoft.AspNetCore.Mvc;
using CDE.Models;

namespace CDE.Repository
{
    public interface IAuthenticateRepository
    {
        Task<IActionResult> LoginInternalAsync(InternalLoginRequest model);
        Task<IActionResult> LoginExternalAsync(ExternalLoginRequest model);
        //Task<IActionResult> LoginAsync(LoginModel model);
        Task<IActionResult> RegisterAsync(RegisterModel model);
        Task<IActionResult> RegisterAdminAsync(RegisterModel model);
        Task<IActionResult> RefreshTokenAsync(TokenModel tokenModel);
        Task<IActionResult> RevokeAsync(string email);
        Task<IActionResult> RevokeAllAsync();
        Task<IActionResult> LogoutAsync(string email);
    }
}
