using System.ComponentModel.DataAnnotations;

namespace AcademicAppoinment.DTOs.Appointments
{
    public class CancelAppointmentDto
    {
        [MaxLength(500)]
        public string? CancellationReason { get; set; }
    }
}
