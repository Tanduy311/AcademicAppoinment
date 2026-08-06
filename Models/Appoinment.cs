using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace AcademicAppoinment.Models
{
    public class Appointment
    {
        public int AppointmentId { get; set; }

        // Khóa ngoại đến Student
        public int StudentId { get; set; }

        // Khóa ngoại đến Lecturer
        public int LecturerId { get; set; }

        // Khóa ngoại đến AvailabilitySlot
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

        // Navigation properties

        [JsonIgnore]
        public Student? Student { get; set; }

        [JsonIgnore]
        public Lecturer? Lecturer { get; set; }

        [JsonIgnore]
        public AvailabilitySlot? AvailabilitySlot { get; set; }

        [JsonIgnore]
        public ICollection<Notification> Notifications { get; set; }
            = new List<Notification>();
    }
}
