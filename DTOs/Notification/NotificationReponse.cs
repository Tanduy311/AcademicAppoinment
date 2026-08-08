namespace AcademicAppoinment.DTOs.Notification
{
    public class NotificationResponse
    {
        public int NotificationId { get; set; }

        public int? AppointmentId { get; set; }

        public string Title { get; set; }

        public string Message { get; set; }

        public bool IsRead { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
