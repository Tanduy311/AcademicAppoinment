using System.ComponentModel.DataAnnotations;

namespace AcademicAppoinment.DTOs.Auth
{
    public class RegisterStudentDto
    {
        [Required(ErrorMessage = "Vui lòng nhập tên tài khoản.")]
        [MaxLength(100)]
        public string AccountName { get; set; } = null!;

        [Required(ErrorMessage = "Vui lòng nhập mật khẩu.")]
        [MinLength(6, ErrorMessage = "Mật khẩu phải có ít nhất 6 ký tự.")]
        public string Password { get; set; } = null!;

        [Required(ErrorMessage = "Vui lòng nhập họ và tên.")]
        [MaxLength(50)]
        public string FullName { get; set; } = null!;

        [Required(ErrorMessage = "Vui lòng nhập Email.")]
        [EmailAddress(ErrorMessage = "Email không đúng định dạng.")]
        public string EmailAddress { get; set; } = null!;

        [MaxLength(20)]
        public string? PhoneNumber { get; set; }

        // Thông tin riêng của Sinh viên
        [Required(ErrorMessage = "Vui lòng nhập mã sinh viên.")]
        [MaxLength(50)]
        public string StudentCode { get; set; } = null!;

        [MaxLength(150)]
        public string? Major { get; set; }

        [MaxLength(50)]
        public string? ClassName { get; set; }

        [MaxLength(20)]
        public string? AcademicYear { get; set; }
    }
}
