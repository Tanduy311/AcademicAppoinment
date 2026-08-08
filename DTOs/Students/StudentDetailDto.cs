using AcademicAppoinment.DTOs.Appointments;

namespace AcademicAppoinment.DTOs.Students
{
    public class StudentDetailDto
    {
        public int StudentId { get; set; }
        public string StudentCode { get; set; } = null!;
        public string FullName { get; set; } = null!;
        public string EmailAddress { get; set; } = null!;
        public string? PhoneNumber { get; set; }
        public string? Major { get; set; }
        public string? ClassName { get; set; }
        public string? AcademicYear { get; set; }
        public IReadOnlyList<AppointmentResponseDto> Appointments { get; set; } = Array.Empty<AppointmentResponseDto>();
    }
}
