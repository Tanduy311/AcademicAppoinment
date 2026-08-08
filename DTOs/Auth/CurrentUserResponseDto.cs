using AcademicAppoinment.Models;

namespace AcademicAppoinment.DTOs.Auth
{
    public class CurrentUserResponseDto
    {
        public int UserId { get; set; }
        public string AccountName { get; set; } = null!;
        public string FullName { get; set; } = null!;
        public string EmailAddress { get; set; } = null!;
        public string? PhoneNumber { get; set; }
        public string? RoleName { get; set; }
        public Student? StudentInfo { get; set; }
        public Lecturer? LecturerInfo { get; set; }
    }
}
