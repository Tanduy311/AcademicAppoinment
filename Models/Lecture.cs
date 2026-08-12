using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace AcademicAppoinment.Models
{
    public class Lecturer
    {
        public int LecturerId { get; set; }

        // Khóa ngoại đến User
        public int UserId { get; set; }

        [Required]
        [MaxLength(50)]
        public string LecturerCode { get; set; } = null!;

        [MaxLength(150)]
        public string? Department { get; set; }

        [MaxLength(150)]
        public string? Specialization { get; set; }

        [MaxLength(200)]
        public string? OfficeLocation { get; set; }

        public string? ConsultationDescription { get; set; }

        // Navigation properties

        [JsonIgnore]
        public User? User { get; set; }

        [JsonIgnore]
        public ICollection<AvailabilitySlot> AvailabilitySlots { get; set; }
            = new List<AvailabilitySlot>();

        [JsonIgnore]
        public ICollection<Appointment> Appointments { get; set; }
            = new List<Appointment>();

        //[JsonIgnore]
        //public ICollection<Notification> Notifications { get; set; }
        //    = new List<Notification>();
    }
}
