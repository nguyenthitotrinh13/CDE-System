using CDE.Models;
using CDE.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CDE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;

        public UserController(IUserRepository userRepository, IRoleRepository roleRepository)
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository;
        }

        // Lấy danh sách người dùng
        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userRepository.GetAllUsersAsync();
            return Ok(users);
        }

        // Tìm kiếm người dùng theo tên hoặc vai trò
        [HttpGet("search")]
        public async Task<IActionResult> SearchUsers([FromQuery] string searchTerm)
        {
            var users = await _userRepository.SearchUsersAsync(searchTerm);
            return Ok(users);
        }

        // Lấy thông tin người dùng theo ID
        [HttpGet("{userId}")]
        public async Task<IActionResult> GetUserById(string userId)
        {
            var user = await _userRepository.GetUserByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }
            return Ok(user);
        }

        // Thêm người dùng mới
        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] ApplicationUser user, [FromQuery] string password)
        {
            var result = await _userRepository.CreateUserAsync(user, password);
            if (result.Succeeded)
            {
                return CreatedAtAction(nameof(GetUserById), new { userId = user.Id }, user);
            }
            return BadRequest(result.Errors);
        }

        // Cập nhật thông tin người dùng
        [HttpPut("{userId}")]
        public async Task<IActionResult> UpdateUser(string userId, [FromBody] ApplicationUser user)
        {
            var existingUser = await _userRepository.GetUserByIdAsync(userId);
            if (existingUser == null)
            {
                return NotFound();
            }
            var result = await _userRepository.UpdateUserAsync(user);
            if (result.Succeeded)
            {
                return NoContent();
            }
            return BadRequest(result.Errors);
        }

        // Xóa người dùng
        [HttpDelete("{userId}")]
        public async Task<IActionResult> DeleteUser(string userId)
        {
            var user = await _userRepository.GetUserByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }
            var result = await _userRepository.DeleteUserAsync(user);
            if (result.Succeeded)
            {
                return NoContent();
            }
            return BadRequest(result.Errors);
        }

        // Đặt lại mật khẩu cho người dùng
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromQuery] string userId, [FromQuery] string newPassword)
        {
            var result = await _userRepository.ResetPasswordAsync(userId, newPassword);
            if (result.Succeeded)
            {
                return NoContent();
            }
            return BadRequest(result.Errors);
        }

        // Nhập danh sách người dùng từ file
        //[HttpPost("import")]
        //public async Task<IActionResult> ImportUsers([FromBody] List<UserImportModel> users)
        //{
        //    var result = await _userRepository.ImportUsersFromFileAsync(users);
        //    if (result.Succeeded)
        //    {
        //        return Ok();
        //    }
        //    return BadRequest(result.Errors);
        //}
    }
}
