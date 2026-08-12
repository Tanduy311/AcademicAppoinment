namespace AcademicAppoinment.DTOs.Admin
{
    public class AdminUserDetailDto
    {
        public int UserId { get; set; }
        public string AccountName { get; set; } = null!;
        public string FullName { get; set; } = null!;
        public string EmailAddress { get; set; } = null!;
        public string? PhoneNumber { get; set; }
        public string RoleName { get; set; } = null!;
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public StudentAdminInfoDto? StudentInfo { get; set; }
        public LecturerAdminInfoDto? LecturerInfo { get; set; }
    }

    public class StudentAdminInfoDto
    {
        public int StudentId { get; set; }
        public string StudentCode { get; set; } = null!;
        public string? Major { get; set; }
        public string? ClassName { get; set; }
        public string? AcademicYear { get; set; }
    }

    public class LecturerAdminInfoDto
    {
        public int LecturerId { get; set; }
        public string LecturerCode { get; set; } = null!;
        public string? Department { get; set; }
        public string? Specialization { get; set; }
        public string? OfficeLocation { get; set; }
        public string? ConsultationDescription { get; set; }
    }
}
