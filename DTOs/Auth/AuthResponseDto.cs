namespace AcademicAppoinment.DTOs.Auth
{
    public class AuthResponseDto
    {
        public string Token { get; set; } = null!;
        public int UserId { get; set; }
        public string AccountName { get; set; } = null!;
        public string FullName { get; set; } = null!;
        public string EmailAddress { get; set; } = null!;
        public string? AvatarUrl { get; set; }
        public string RoleName { get; set; } = null!;
        public int? StudentId { get; set; }
        public int? LecturerId { get; set; }
    }
}
