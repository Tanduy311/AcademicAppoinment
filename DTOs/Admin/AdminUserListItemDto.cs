namespace AcademicAppoinment.DTOs.Admin
{
    public class AdminUserListItemDto
    {
        public int UserId { get; set; }
        public string AccountName { get; set; } = null!;
        public string FullName { get; set; } = null!;
        public string EmailAddress { get; set; } = null!;
        public string? PhoneNumber { get; set; }
        public string RoleName { get; set; } = null!;
        public bool IsActive { get; set; }
        public int? StudentId { get; set; }
        public int? LecturerId { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
