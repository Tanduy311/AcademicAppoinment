using System.ComponentModel.DataAnnotations;

namespace AcademicAppointment.Domain.Entities
{
    public class Appointment
    {
        public int AppointmentId { get; set; }

        public int StudentId { get; set; }

        public int LecturerId { get; set; }

        public int AvailabilitySlotId { get; set; }

        [Required]
        [MaxLength(200)]
        public string Topic { get; set; } = null!;

        public string? Description { get; set; }

        [Required]
        [MaxLength(50)]
        public string Status { get; set; } = "Pending";

        public string? LecturerResponse { get; set; }

        public string? CancellationReason { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        public Student? Student { get; set; }

        public Lecturer? Lecturer { get; set; }

        public AvailabilitySlot? AvailabilitySlot { get; set; }

        public ICollection<Notification> Notifications { get; set; }
            = new List<Notification>();
    }
}

