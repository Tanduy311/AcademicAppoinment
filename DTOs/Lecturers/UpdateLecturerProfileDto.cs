using System.ComponentModel.DataAnnotations;

namespace AcademicAppoinment.DTOs.Lecturers
{
    public class UpdateLecturerProfileDto
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
        public string? Department { get; set; }

        [MaxLength(200)]
        public string? Specialization { get; set; }

        [MaxLength(200)]
        public string? OfficeLocation { get; set; }

        [MaxLength(1000)]
        public string? ConsultationDescription { get; set; }
    }
}
