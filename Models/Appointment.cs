using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace AcademicAppoinment.Models
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
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; }

        [JsonIgnore]
        public Student? Student { get; set; }

        [JsonIgnore]
        public Lecturer? Lecturer { get; set; }

        [JsonIgnore]
        public AvailabilitySlot? AvailabilitySlot { get; set; }

        [JsonIgnore]
        public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
    }
}
