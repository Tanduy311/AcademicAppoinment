using System.ComponentModel.DataAnnotations;

namespace AcademicAppointment.Domain.Entities
{
    public class Student
    {
        public int StudentId { get; set; }

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

        public User? User { get; set; }

        public ICollection<Appointment> Appointments { get; set; }
            = new List<Appointment>();
    }
}

