using System.ComponentModel.DataAnnotations;

namespace AcademicAppoinment.DTOs.Students
{
    public class UpdateStudentProfileDto
    {
        [Required(ErrorMessage = "Họ và tên không được để trống.")]
        [MaxLength(100)]
        public string FullName { get; set; } = null!;

        [Required(ErrorMessage = "Email không được để trống.")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ.")]
        [MaxLength(150)]
        public string EmailAddress { get; set; } = null!;

        [Phone(ErrorMessage = "Số điện thoại không hợp lệ.")]
        [MaxLength(20)]
        public string? PhoneNumber { get; set; }

        [MaxLength(100)]
        public string? Major { get; set; }

        [MaxLength(50)]
        public string? ClassName { get; set; }

        [MaxLength(50)]
        public string? AcademicYear { get; set; }

        [MaxLength(500)]
        public string? AcademicProgressFileUrl { get; set; }
    }
}
