namespace AcademicAppointment.Application.DTOs.Auth
{
    public class CurrentUserDto
    {
        public int UserId { get; set; }
        public string AccountName { get; set; } = null!;
        public string FullName { get; set; } = null!;
        public string EmailAddress { get; set; } = null!;
        public string? PhoneNumber { get; set; }
        public string? RoleName { get; set; }
        public StudentProfileDto? StudentInfo { get; set; }
        public LecturerProfileDto? LecturerInfo { get; set; }
    }

    public class StudentProfileDto
    {
        public int StudentId { get; set; }
        public string StudentCode { get; set; } = null!;
        public string? Major { get; set; }
        public string? ClassName { get; set; }
        public string? AcademicYear { get; set; }
    }

    public class LecturerProfileDto
    {
        public int LecturerId { get; set; }
        public string LecturerCode { get; set; } = null!;
        public string? Department { get; set; }
        public string? Specialization { get; set; }
        public string? OfficeLocation { get; set; }
        public string? ConsultationDescription { get; set; }
    }
}

