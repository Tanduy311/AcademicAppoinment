namespace AcademicAppoinment.DTOs.Notification
{
    public class NotificationResponseDto
    {
        public int NotificationId { get; set; }
        public int UserId { get; set; }
        public int? StudentId { get; set; }
        public int? LecturerId { get; set; }
        public int? AppointmentId { get; set; }
        public string Title { get; set; } = null!;
        public string Message { get; set; } = null!;
        public bool IsRead { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
