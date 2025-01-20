using CDE.Models;
using CDE.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace FlightMicroservice.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    
    public class AuthenticateController : ControllerBase
    {
        private readonly IAuthenticateRepository _authenticateRepository;

        public AuthenticateController(IAuthenticateRepository authenticateService)
        {
            _authenticateRepository = authenticateService;
        }
 
        //[HttpPost("login")]
        //public Task<IActionResult> Login([FromBody] LoginModel model)
        //{
        //    return _authenticateRepository.LoginAsync(model);
        //}
        //[HttpPost("login")]
        //public async Task<IActionResult> Login([FromBody] LoginRequest request)
        //{
        //    var accessToken = await _authenticationService.GetAccessTokenFromMicrosoftAsync();
        //    var jwtToken = _authenticationService.GenerateJwtToken(accessToken);

        //    return Ok(new { token = new JwtSecurityTokenHandler().WriteToken(jwtToken) });
        //}
        [HttpPost("login")]
        public async Task<IActionResult> LoginAsync([FromBody] ExternalLoginRequest model)
        {
            if (model == null || string.IsNullOrEmpty(model.Email) || string.IsNullOrEmpty(model.Password))
            {
                return BadRequest("Invalid input data.");
            }

            var result = await _authenticateRepository.LoginExternalAsync(model);

            if (result is OkObjectResult)
            {
                return Ok();
            }

            return Unauthorized();
        }

        // Đăng nhập với Azure AD
        [HttpPost("login-azure")]
        public async Task<IActionResult> LoginWithAzureAsync([FromBody] InternalLoginRequest model)
        {
            if (model == null || string.IsNullOrEmpty(model.Email))
            {
                return BadRequest("Invalid email.");
            }

            var result = await _authenticateRepository.LoginInternalAsync(model);

            if (result is OkObjectResult)
            {
                return Ok();
            }

            return Unauthorized();
        }

        [HttpPost("register")]
        public Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            return _authenticateRepository.RegisterAsync(model);
        }

        [HttpPost("register-admin")]
        public Task<IActionResult> RegisterAdmin([FromBody] RegisterModel model)
        {
            return _authenticateRepository.RegisterAdminAsync(model);
        }

        [HttpPost("refresh-token")]
        public Task<IActionResult> RefreshToken([FromBody] TokenModel tokenModel)
        {
            return _authenticateRepository.RefreshTokenAsync(tokenModel);
        }

        [HttpPost("revoke/{email}")]
        public Task<IActionResult> Revoke(string email)
        {
            return _authenticateRepository.RevokeAsync(email);
        }

        [HttpPost("revoke-all")]
        public Task<IActionResult> RevokeAll()
        {
            return _authenticateRepository.RevokeAllAsync();
        }

        [HttpPost("logout")]
        public Task<IActionResult> Logout()
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            return _authenticateRepository.LogoutAsync(email);
        }
    }
}

