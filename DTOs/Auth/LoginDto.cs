using System.ComponentModel.DataAnnotations;

namespace AcademicAppoinment.DTOs.Auth
{
    public class LoginDto
    {
        [Required(ErrorMessage = "Vui lòng nhập tên tài khoản.")]
        public string AccountName { get; set; } = null!;

        [Required(ErrorMessage = "Vui lòng nhập mật khẩu.")]
        public string Password { get; set; } = null!;
    }
}
