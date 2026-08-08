using System.ComponentModel.DataAnnotations;

namespace AcademicAppoinment.DTOs
{
    public class UpdateAvailabilitySlotRequest
    {
        [Required]
        public DateTime StartTime { get; set; }

        [Required]
        public DateTime EndTime { get; set; }

        [Required]
        [MaxLength(50)]
        public string MeetingType { get; set; } = null!;

        [MaxLength(500)]
        public string? LocationOrLink { get; set; }
    }
}
