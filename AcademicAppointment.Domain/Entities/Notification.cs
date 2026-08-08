using System.ComponentModel.DataAnnotations;

namespace AcademicAppointment.Domain.Entities
{
    public class Notification
    {
        public int NotificationId { get; set; }

        public int UserId { get; set; }

        public int? StudentId { get; set; }

        public int? LecturerId { get; set; }

        public int? AppointmentId { get; set; }

        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = null!;

        [Required]
        public string Message { get; set; } = null!;

        public bool IsRead { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public User? User { get; set; }

        public Student? Student { get; set; }

        public Lecturer? Lecturer { get; set; }

        public Appointment? Appointment { get; set; }
    }
}

