using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace AcademicAppoinment.Models
{
    public class AvailabilitySlot
    {
        public int AvailabilitySlotId { get; set; }

        // Khóa ngoại đến Lecturer
        public int LecturerId { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        [Required]
        [MaxLength(50)]
        public string MeetingType { get; set; } = null!;

        [MaxLength(500)]
        public string? LocationOrLink { get; set; }

        public bool IsAvailable { get; set; } = true;

        public bool IsDeleted { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Navigation properties

        [JsonIgnore]
        public Lecturer? Lecturer { get; set; }

        [JsonIgnore]
        public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    }
}
