using System.ComponentModel.DataAnnotations;

namespace AcademicAppoinment.DTOs
{
    public class CreateAppointmentRequest
    {
        [Required]
        public int AvailabilitySlotId { get; set; }

        [Required]
        [MaxLength(200)]
        public string Topic { get; set; }

        public string? Description { get; set; }
    }
}