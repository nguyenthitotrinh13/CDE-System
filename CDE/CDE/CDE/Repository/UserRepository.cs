using CDE.Models;
using Microsoft.AspNetCore.Identity;
using System.Formats.Asn1;
using CsvHelper;
using CsvHelper.Configuration;
using OfficeOpenXml;

namespace CDE.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UserRepository(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<ApplicationUser> GetUserByIdAsync(string userId)
        {
            return await _userManager.FindByIdAsync(userId);
        }

        public async Task<ApplicationUser> GetUserByEmailAsync(string email)
        {
            return await _userManager.FindByEmailAsync(email);
        }

        public async Task<IEnumerable<ApplicationUser>> GetAllUsersAsync()
        {
            return _userManager.Users.ToList();
        }

        public async Task<IdentityResult> CreateUserAsync(ApplicationUser user, string password)
        {
            return await _userManager.CreateAsync(user, password);
        }

        public async Task<IdentityResult> UpdateUserAsync(ApplicationUser user)
        {
            return await _userManager.UpdateAsync(user);
        }

        public async Task<IdentityResult> DeleteUserAsync(ApplicationUser user)
        {
            return await _userManager.DeleteAsync(user);
        }

        public async Task<IEnumerable<ApplicationUser>> SearchUsersAsync(string searchTerm)
        {
            return _userManager.Users.Where(u => u.UserName.Contains(searchTerm) || u.Email.Contains(searchTerm)).ToList();
        }

        public async Task<IdentityResult> ResetPasswordAsync(string userId, string newPassword)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                // Trả về lỗi nếu không tìm thấy người dùng
                return IdentityResult.Failed(new IdentityError { Description = "User not found." });
            }

            // Tạo mã xác nhận (reset token) cho người dùng
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            // Reset mật khẩu với token và mật khẩu mới
            var result = await _userManager.ResetPasswordAsync(user, token, newPassword);

            return result;
        }

        public async Task<IdentityResult> ImportUsersFromFileAsync(IEnumerable<UserImportModel> users)
        {
            var errors = new List<string>();

            foreach (var userImport in users)
            {
                // Kiểm tra nếu người dùng đã tồn tại
                var existingUser = await _userManager.FindByEmailAsync(userImport.Email);
                if (existingUser != null)
                {
                    errors.Add($"User with email {userImport.Email} already exists.");
                    continue;
                }

                // Tạo người dùng mới
                var user = new ApplicationUser
                {
                    UserName = userImport.UserName,
                    Email = userImport.Email,
                    //FullName = userImport.FullName
                };

                var result = await _userManager.CreateAsync(user, userImport.Password);
                if (!result.Succeeded)
                {
                    errors.Add($"Failed to create user {userImport.Email}: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                    continue;
                }

                // Gán vai trò cho người dùng (nếu có)
                if (!string.IsNullOrEmpty(userImport.Role))
                {
                    var roleExist = await _roleManager.RoleExistsAsync(userImport.Role);
                    if (!roleExist)
                    {
                        errors.Add($"Role {userImport.Role} does not exist for user {userImport.Email}");
                        continue;
                    }

                    await _userManager.AddToRoleAsync(user, userImport.Role);
                }
            }

            // Trả về kết quả, có thể trả về thông báo lỗi nếu có
            if (errors.Any())
            {
                return IdentityResult.Failed(errors.Select(e => new IdentityError { Description = e }).ToArray());
            }

            return IdentityResult.Success;
        }

        // Xử lý đọc từ file CSV
        public async Task<IEnumerable<UserImportModel>> ReadUsersFromCsvAsync(string filePath)
        {
            var users = new List<UserImportModel>();

            using (var reader = new StreamReader(filePath))
            using (var csv = new CsvReader(reader, new CsvConfiguration(System.Globalization.CultureInfo.InvariantCulture)))
            {
                var records = csv.GetRecords<UserImportModel>();
                users = records.ToList();
            }

            return users;
        }

        // Xử lý đọc từ file Excel (XLSX)
        public async Task<IEnumerable<UserImportModel>> ReadUsersFromExcelAsync(string filePath)
        {
            var users = new List<UserImportModel>();

            using (var package = new ExcelPackage(new FileInfo(filePath)))
            {
                var worksheet = package.Workbook.Worksheets[0]; // Lấy sheet đầu tiên
                var rowCount = worksheet.Dimension.Rows;

                for (int row = 2; row <= rowCount; row++) // Bỏ qua dòng tiêu đề (dòng 1)
                {
                    var user = new UserImportModel
                    {
                        UserName = worksheet.Cells[row, 1].Text, // Cột 1: UserName
                        Email = worksheet.Cells[row, 2].Text,    // Cột 2: Email
                        FullName = worksheet.Cells[row, 3].Text, // Cột 3: FullName
                        Password = worksheet.Cells[row, 4].Text, // Cột 4: Password
                        Role = worksheet.Cells[row, 5].Text      // Cột 5: Role
                    };

                    users.Add(user);
                }
            }

            return users;
        }
    }
}
