using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace AcademicAppoinment.Models
{
    public class Student
    {
        public int StudentId { get; set; }

        // Khóa ngoại đến User
        public int UserId { get; set; }

        [Required]
        [MaxLength(50)]
        public string StudentCode { get; set; } = null!;


        [MaxLength(150)]
        public string? Major { get; set; }

        [MaxLength(50)]
        public string? ClassName { get; set; }

        [MaxLength(20)]
        public string? AcademicYear { get; set; }

        // Navigation properties

        [JsonIgnore]
        public User? User { get; set; }

        [JsonIgnore]
        public ICollection<Appointment> Appointments { get; set; }
            = new List<Appointment>();

        //[JsonIgnore]
        //public ICollection<Notification> Notifications { get; set; }
        //    = new List<Notification>();
    }
}
