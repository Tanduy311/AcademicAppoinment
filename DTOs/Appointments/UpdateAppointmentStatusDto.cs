using System.ComponentModel.DataAnnotations;

namespace AcademicAppoinment.DTOs.Appointments
{
    public class UpdateAppointmentStatusDto
    {
        [Required]
        [MaxLength(50)]
        public string Status { get; set; } = null!;

        [MaxLength(500)]
        public string? LecturerResponse { get; set; }
    }
}
