using System.ComponentModel.DataAnnotations;

namespace AcademicAppointment.Domain.Entities
{
    public class Lecturer
    {
        public int LecturerId { get; set; }

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

        public User? User { get; set; }

        public ICollection<AvailabilitySlot> AvailabilitySlots { get; set; }
            = new List<AvailabilitySlot>();

        public ICollection<Appointment> Appointments { get; set; }
            = new List<Appointment>();
    }
}

