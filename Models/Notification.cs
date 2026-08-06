using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace AcademicAppoinment.Models
{
    public class Notification
    {
        public int NotificationId { get; set; }

        // Người dùng nhận thông báo
        public int UserId { get; set; }

        // Các khóa ngoại này có thể không có
        public int? StudentId { get; set; }

        public int? LecturerId { get; set; }

        public int? AppointmentId { get; set; }

        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = null!;

        [Required]
        public string Message { get; set; } = null!;

        public bool IsRead { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Navigation properties

        [JsonIgnore]
        public User? User { get; set; }

        [JsonIgnore]
        public Student? Student { get; set; }

        [JsonIgnore]
        public Lecturer? Lecturer { get; set; }

        [JsonIgnore]
        public Appointment? Appointment { get; set; }
    }
}
