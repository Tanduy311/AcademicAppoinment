using System.ComponentModel.DataAnnotations;

namespace AcademicAppoinment.DTOs
{
    public class CancelAppointmentRequest
    {
        [Required]
        [MaxLength(1000)]
        public string Reason { get; set; }
    }
}