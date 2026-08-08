using System.ComponentModel.DataAnnotations;

namespace AcademicAppoinment.DTOs
{
    public class RejectAppointmentRequest
    {
        [Required]
        [MaxLength(500)]
        public string Reason { get; set; } = null!;
    }

    public class CancelAppointmentByLecturerRequest
    {
        [Required]
        [MaxLength(500)]
        public string Reason { get; set; } = null!;
    }
}
