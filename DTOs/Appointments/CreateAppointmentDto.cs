using System.ComponentModel.DataAnnotations;

namespace AcademicAppoinment.DTOs.Appointments
{
    public class CreateAppointmentDto
    {
        [Required]
        public int AvailabilitySlotId { get; set; }

        [Required]
        [MaxLength(200)]
        public string Topic { get; set; } = null!;

        public string? Description { get; set; }
    }
}
