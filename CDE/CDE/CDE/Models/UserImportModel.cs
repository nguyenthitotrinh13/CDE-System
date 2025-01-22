namespace CDE.Models
{
    public class UserImportModel
    {
        public string UserName { get; set; }  // Tên đăng nhập
        public string Email { get; set; }     // Địa chỉ email
        public string FullName { get; set; }  // Tên đầy đủ
        public string Password { get; set; }  // Mật khẩu
        public string Role { get; set; }
    }
}
