using CDE.Models;
using CDE.Repository;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CDE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PermissionController : ControllerBase
    {
        private readonly IPermissionRepository _permissionRepository;
        private readonly UserManager<ApplicationUser> _userManager;

        public PermissionController(IPermissionRepository permissionRepository, UserManager<ApplicationUser> userManager)
        {
            _permissionRepository = permissionRepository;
            _userManager = userManager;
        }

        // Lấy tất cả quyền của người dùng
        [HttpGet("user/{userId}/permissions")]
        public async Task<IActionResult> GetPermissionsByUserAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            var permissions = await _permissionRepository.GetPermissionsByUserAsync(user);
            return Ok(permissions);  // Trả về danh sách quyền của người dùng
        }

        // Lấy tất cả quyền
        [HttpGet("permissions")]
        public async Task<IActionResult> GetAllPermissionsAsync()
        {
            var permissions = await _permissionRepository.GetAllPermissionsAsync();
            return Ok(permissions);  // Trả về tất cả quyền (claims của các vai trò)
        }

        // Gán quyền cho vai trò
        [HttpPost("roles/{roleName}/permissions")]
        public async Task<IActionResult> AssignPermissionToRoleAsync(string roleName, [FromBody] string permission)
        {
            var result = await _permissionRepository.AssignPermissionToRoleAsync(roleName, permission);
            if (result.Succeeded)
            {
                return Ok("Permission assigned successfully.");
            }
            return BadRequest("Error assigning permission.");
        }

        // Gỡ quyền khỏi vai trò
        [HttpDelete("roles/{roleName}/permissions")]
        public async Task<IActionResult> RemovePermissionFromRoleAsync(string roleName, [FromBody] string permission)
        {
            var result = await _permissionRepository.RemovePermissionFromRoleAsync(roleName, permission);
            if (result.Succeeded)
            {
                return Ok("Permission removed successfully.");
            }
            return BadRequest("Error removing permission.");
        }
    }
}
