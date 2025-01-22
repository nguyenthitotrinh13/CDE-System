using CDE.Models;
using Microsoft.AspNetCore.Identity;

namespace CDE.Repository
{
    public class PermissionRepository : IPermissionRepository
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public PermissionRepository(RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
        }

        // Lấy tất cả các quyền thông qua vai trò (claims của tất cả vai trò)
        public async Task<IEnumerable<string>> GetAllPermissionsAsync()
        {
            var allRoles = _roleManager.Roles.ToList();  // Lấy tất cả vai trò
            var permissions = new List<string>();

            foreach (var role in allRoles)
            {
                var roleClaims = await _roleManager.GetClaimsAsync(role);  // Lấy claims của mỗi vai trò
                permissions.AddRange(roleClaims.Select(c => c.Value));  // Thêm quyền vào danh sách
            }

            return permissions.Distinct();  // Trả về danh sách quyền không trùng lặp
        }

        // Các phương thức còn lại vẫn giữ nguyên
        public async Task<IdentityResult> AssignPermissionToRoleAsync(string roleName, string permission)
        {
            var role = await _roleManager.FindByNameAsync(roleName);  // Kiểm tra vai trò đã tồn tại chưa
            if (role == null)
            {
                role = new IdentityRole(roleName);  // Nếu vai trò không tồn tại, tạo mới
                await _roleManager.CreateAsync(role);
            }

            // Thêm quyền vào vai trò thông qua claim
            var result = await _roleManager.AddClaimAsync(role, new System.Security.Claims.Claim("permission", permission));
            return result;
        }

        public async Task<IdentityResult> RemovePermissionFromRoleAsync(string roleName, string permission)
        {
            var role = await _roleManager.FindByNameAsync(roleName);
            if (role != null)
            {
                var roleClaims = await _roleManager.GetClaimsAsync(role);
                var claim = roleClaims.FirstOrDefault(c => c.Value == permission);
                if (claim != null)
                {
                    return await _roleManager.RemoveClaimAsync(role, claim);  // Gỡ quyền khỏi vai trò
                }
            }
            return IdentityResult.Failed();  // Nếu không tìm thấy quyền, trả về lỗi
        }

        // Lấy tất cả các quyền của người dùng (thông qua vai trò)
        public async Task<IEnumerable<string>> GetPermissionsByUserAsync(ApplicationUser user)
        {
            var roles = await _userManager.GetRolesAsync(user);  // Lấy các vai trò của người dùng
            var permissions = new List<string>();

            foreach (var role in roles)
            {
                var identityRole = await _roleManager.FindByNameAsync(role);  // Lấy vai trò
                if (identityRole != null)
                {
                    var roleClaims = await _roleManager.GetClaimsAsync(identityRole);  // Lấy claims của vai trò
                    permissions.AddRange(roleClaims.Select(c => c.Value));  // Thêm các claim vào danh sách quyền
                }
            }

            return permissions.Distinct();  // Trả về danh sách quyền không trùng
        }
    }
}

