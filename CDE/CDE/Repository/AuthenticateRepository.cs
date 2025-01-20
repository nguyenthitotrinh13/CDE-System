using CDE.Models;
using CDE.Repository;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Graph;
using Microsoft.Identity.Client;
using Azure.Identity;


namespace FlightMicroservice.Repository
{
    public class AuthenticateRepository : IAuthenticateRepository
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;

        public AuthenticateRepository(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IConfiguration configuration)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
        }

        


        public async Task<IActionResult> LoginInternalAsync(InternalLoginRequest model)
        {
            // Xác thực với Azure AD
            var user = await AuthenticateWithAzureADAsync(model.Email);
            if (user == null) return new UnauthorizedResult();

            // Tạo token và refresh token
            var userRoles = await _userManager.GetRolesAsync(user);
            var authClaims = new List<Claim>
    {
        new Claim(ClaimTypes.Name, user.UserName),
        new Claim(ClaimTypes.Email, user.Email),
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
    };

            foreach (var userRole in userRoles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, userRole));
                var role = await _roleManager.FindByNameAsync(userRole);
                var roleClaims = await _roleManager.GetClaimsAsync(role);

                foreach (var claim in roleClaims)
                {
                    authClaims.Add(claim);
                }
            }

            var token = CreateToken(authClaims);
            var refreshToken = GenerateRefreshToken();
            int.TryParse(_configuration["JWT:RefreshTokenValidityInDays"], out int refreshTokenValidityInDays);

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.Now.AddDays(refreshTokenValidityInDays);
            await _userManager.UpdateAsync(user);

            return new OkObjectResult(new
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                RefreshToken = refreshToken,
                Expiration = token.ValidTo
            });
        }

        public async Task<IActionResult> LoginExternalAsync(ExternalLoginRequest model)
        {
            // Đăng nhập bằng email và mật khẩu
            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user == null || !await _userManager.CheckPasswordAsync(user, model.Password))
            {
                return new UnauthorizedResult();
            }

            // Tạo token và refresh token
            var userRoles = await _userManager.GetRolesAsync(user);
            var authClaims = new List<Claim>
    {
        new Claim(ClaimTypes.Name, user.UserName),
        new Claim(ClaimTypes.Email, user.Email),
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
    };

            foreach (var userRole in userRoles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, userRole));
                var role = await _roleManager.FindByNameAsync(userRole);
                var roleClaims = await _roleManager.GetClaimsAsync(role);

                foreach (var claim in roleClaims)
                {
                    authClaims.Add(claim);
                }
            }

            var token = CreateToken(authClaims);
            var refreshToken = GenerateRefreshToken();
            int.TryParse(_configuration["JWT:RefreshTokenValidityInDays"], out int refreshTokenValidityInDays);

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.Now.AddDays(refreshTokenValidityInDays);
            await _userManager.UpdateAsync(user);

            return new OkObjectResult(new
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                RefreshToken = refreshToken,
                Expiration = token.ValidTo
            });
        }
        public async Task<ApplicationUser> AuthenticateWithAzureADAsync(string email)
        {
            // Tạo đối tượng ClientSecretCredential để lấy token
            var credential = new Azure.Identity.ClientSecretCredential(
                _configuration["AzureAD:TenantId"],
                _configuration["AzureAD:ClientId"],
                _configuration["AzureAD:ClientSecret"]
            );

            // Tạo GraphServiceClient với TokenCredential
            var graphServiceClient = new Microsoft.Graph.GraphServiceClient(credential);

            // Lấy thông tin người dùng từ Azure AD bằng cách lọc theo email
            var users = await graphServiceClient.Users
                .GetAsync(requestConfiguration =>
                {
                    requestConfiguration.QueryParameters.Filter = $"mail eq '{email}'";
                });

            var user = users.Value.FirstOrDefault();

            // Nếu không tìm thấy người dùng, trả về null
            if (user == null)
            {
                return null;
            }

            // Tạo đối tượng ApplicationUser từ thông tin của Azure AD
            var applicationUser = new ApplicationUser
            {
                UserName = user.UserPrincipalName,
                Email = user.UserPrincipalName,
                // Cập nhật thêm các thông tin khác nếu cần
            };

            return applicationUser;
        }



        private JwtSecurityToken CreateToken(List<Claim> authClaims)
        {
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));
            int.TryParse(_configuration["JWT:TokenValidityInMinutes"], out int tokenValidityInMinutes);

            return new JwtSecurityToken(
                issuer: _configuration["JWT:ValidIssuer"],
                audience: _configuration["JWT:ValidAudience"],
                expires: DateTime.Now.AddMinutes(tokenValidityInMinutes),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
            );
        }

        private static string GenerateRefreshToken()
        {
            var randomNumber = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }
        //public async Task<IActionResult> LoginAsync(LoginModel model)
        //{
        //    var user = await _userManager.FindByEmailAsync(model.Email);

        //    if (!user.IsActive)
        //    {
        //        return new UnauthorizedResult();
        //    }
        //    if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
        //    {
        //        var userRoles = await _userManager.GetRolesAsync(user);
        //        var authClaims = new List<Claim>
        //    {
        //        new Claim(ClaimTypes.Name, user.UserName),
        //        new Claim(ClaimTypes.Email, user.Email),
        //        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        //    };

        //        foreach (var userRole in userRoles)
        //        {
        //            authClaims.Add(new Claim(ClaimTypes.Role, userRole));

        //            var role = await _roleManager.FindByNameAsync(userRole);
        //            var roleClaims = await _roleManager.GetClaimsAsync(role);

        //            foreach (var claim in roleClaims)
        //            {
        //                authClaims.Add(claim);
        //            }
        //        }
        //        var token = CreateToken(authClaims);
        //        var refreshToken = GenerateRefreshToken();
        //        int.TryParse(_configuration["JWT:RefreshTokenValidityInDays"], out int refreshTokenValidityInDays);

        //        user.RefreshToken = refreshToken;
        //        user.RefreshTokenExpiryTime = DateTime.Now.AddDays(refreshTokenValidityInDays);

        //        await _userManager.UpdateAsync(user);

        //        return new OkObjectResult(new
        //        {
        //            Token = new JwtSecurityTokenHandler().WriteToken(token),
        //            RefreshToken = refreshToken,
        //            Expiration = token.ValidTo
        //        });
        //    }

        //    return new UnauthorizedResult();
        //}

        public async Task<IActionResult> RegisterAsync(RegisterModel model)
        {
            var userExists = await _userManager.FindByEmailAsync(model.Email);
            if (userExists != null)
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);

            ApplicationUser user = new()
            {
                Email = model.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = model.Username,
                RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(30)
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);

            return new OkObjectResult(new Response { Status = "Success", Message = "User created successfully!" });
        }

        public async Task<IActionResult> RegisterAdminAsync(RegisterModel model)
        {
            var userExists = await _userManager.FindByEmailAsync(model.Email);
            if (userExists != null)
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);

            ApplicationUser user = new()
            {
                Email = model.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = model.Username,
                RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(30)
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);

            if (!await _roleManager.RoleExistsAsync(UserRoles.Admin))
                await _roleManager.CreateAsync(new IdentityRole(UserRoles.Admin));
            if (result.Succeeded)
            {
                await _userManager.AddClaimAsync(user, new Claim("Permission", "EditAll"));
                await _userManager.AddClaimAsync(user, new Claim("Permission", "ViewAll"));
                await _userManager.AddClaimAsync(user, new Claim("Permission", "EditDocuments"));
                await _userManager.AddClaimAsync(user, new Claim("Permission", "ViewDocuments"));
            }
            return new OkObjectResult(new Response { Status = "Success", Message = "Admin created successfully!" });
        }

        public async Task<IActionResult> RefreshTokenAsync(TokenModel tokenModel)
        {
            if (tokenModel is null)
            {
                return new BadRequestObjectResult("Invalid client request");
            }

            var principal = GetPrincipalFromExpiredToken(tokenModel.AccessToken);
            if (principal == null)
            {
                return new BadRequestObjectResult("Invalid access token or refresh token");
            }

            var email = principal.Identity.Name;
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null || user.RefreshToken != tokenModel.RefreshToken || user.RefreshTokenExpiryTime <= DateTime.Now)
            {
                return new BadRequestObjectResult("Invalid access token or refresh token");
            }

            var newAccessToken = CreateToken(principal.Claims.ToList());
            var newRefreshToken = GenerateRefreshToken();

            user.RefreshToken = newRefreshToken;
            await _userManager.UpdateAsync(user);

            return new ObjectResult(new
            {
                accessToken = new JwtSecurityTokenHandler().WriteToken(newAccessToken),
                refreshToken = newRefreshToken
            });
        }

        public async Task<IActionResult> RevokeAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return new BadRequestObjectResult("Invalid Email");

            user.RefreshToken = null;
            await _userManager.UpdateAsync(user);

            return new NoContentResult();
        }

        public async Task<IActionResult> RevokeAllAsync()
        {
            var users = _userManager.Users.ToList();
            foreach (var user in users)
            {
                user.RefreshToken = null;
                await _userManager.UpdateAsync(user);
            }

            return new NoContentResult();
        }

        public async Task<IActionResult> LogoutAsync(string email)
        {

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return new BadRequestObjectResult("User not found");
            }
            user.RefreshToken = null;
            user.RefreshTokenExpiryTime = DateTime.MinValue;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }

            return new NoContentResult();

        }

        //private JwtSecurityToken CreateToken(List<Claim> authClaims)
        //{
        //    var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));
        //    int.TryParse(_configuration["JWT:TokenValidityInMinutes"], out int tokenValidityInMinutes);

        //    return new JwtSecurityToken(
        //        issuer: _configuration["JWT:ValidIssuer"],
        //        audience: _configuration["JWT:ValidAudience"],
        //        expires: DateTime.Now.AddMinutes(tokenValidityInMinutes),
        //        claims: authClaims,
        //        signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
        //    );
        //}

        //private static string GenerateRefreshToken()
        //{
        //    var randomNumber = new byte[64];
        //    using var rng = RandomNumberGenerator.Create();
        //    rng.GetBytes(randomNumber);
        //    return Convert.ToBase64String(randomNumber);
        //}

        private ClaimsPrincipal? GetPrincipalFromExpiredToken(string? token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"])),
                ValidateLifetime = false
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);
            if (securityToken is not JwtSecurityToken jwtSecurityToken || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                throw new SecurityTokenException("Invalid token");

            return principal;
        }

    }
}
