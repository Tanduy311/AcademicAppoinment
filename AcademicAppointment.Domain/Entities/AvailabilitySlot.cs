using System.ComponentModel.DataAnnotations;

namespace AcademicAppointment.Domain.Entities
{
    public class AvailabilitySlot
    {
        public int AvailabilitySlotId { get; set; }

        public int LecturerId { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        [Required]
        [MaxLength(50)]
        public string MeetingType { get; set; } = null!;

        [MaxLength(500)]
        public string? LocationOrLink { get; set; }

        public bool IsAvailable { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public Lecturer? Lecturer { get; set; }

        public Appointment? Appointment { get; set; }
    }
}

