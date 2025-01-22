using CDE.Repository;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CDE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        private readonly IRoleRepository _roleRepository;

        public RoleController(IRoleRepository roleRepository)
        {
            _roleRepository = roleRepository;
        }

        // Lấy danh sách vai trò
        [HttpGet]
        public async Task<IActionResult> GetAllRoles()
        {
            var roles = await _roleRepository.GetAllRolesAsync();
            return Ok(roles);
        }

        // Lấy thông tin vai trò theo ID
        [HttpGet("{roleId}")]
        public async Task<IActionResult> GetRoleById(string roleId)
        {
            var role = await _roleRepository.GetRoleByIdAsync(roleId);
            if (role == null)
            {
                return NotFound();
            }
            return Ok(role);
        }

        // Thêm vai trò mới
        [HttpPost]
        public async Task<IActionResult> CreateRole([FromBody] IdentityRole role)
        {
            var result = await _roleRepository.CreateRoleAsync(role);
            if (result.Succeeded)
            {
                return CreatedAtAction(nameof(GetRoleById), new { roleId = role.Id }, role);
            }
            return BadRequest(result.Errors);
        }

        // Cập nhật vai trò
        [HttpPut("{roleId}")]
        public async Task<IActionResult> UpdateRole(string roleId, [FromBody] IdentityRole role)
        {
            var existingRole = await _roleRepository.GetRoleByIdAsync(roleId);
            if (existingRole == null)
            {
                return NotFound();
            }
            var result = await _roleRepository.UpdateRoleAsync(role);
            if (result.Succeeded)
            {
                return NoContent();
            }
            return BadRequest(result.Errors);
        }

        // Xóa vai trò
        [HttpDelete("{roleId}")]
        public async Task<IActionResult> DeleteRole(string roleId)
        {
            var role = await _roleRepository.GetRoleByIdAsync(roleId);
            if (role == null)
            {
                return NotFound();
            }
            var result = await _roleRepository.DeleteRoleAsync(role);
            if (result.Succeeded)
            {
                return NoContent();
            }
            return BadRequest(result.Errors);
        }

        // Gán vai trò cho người dùng
        //[HttpPost("assign-role")]
        //public async Task<IActionResult> AssignRoleToUser([FromQuery] string userId, [FromQuery] string roleName)
        //{
        //    var result = await _roleRepository.AssignRoleToUserAsync(userId, roleName);
        //    if (result.Succeeded)
        //    {
        //        return NoContent();
        //    }
        //    return BadRequest(result.Errors);
        //}

        //// Loại bỏ vai trò khỏi người dùng
        //[HttpPost("remove-role")]
        //public async Task<IActionResult> RemoveRoleFromUser([FromQuery] string userId, [FromQuery] string roleName)
        //{
        //    var result = await _roleRepository.RemoveRoleFromUserAsync(userId, roleName);
        //    if (result.Succeeded)
        //    {
        //        return NoContent();
        //    }
        //    return BadRequest(result.Errors);
        //}
    }
}
