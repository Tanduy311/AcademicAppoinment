using AcademicAppoinment.DTOs.Slot;

namespace AcademicAppoinment.DTOs.Lecturers
{
    public class LecturerDetailDto
    {
        public int LecturerId { get; set; }
        public string LecturerCode { get; set; } = null!;
        public string FullName { get; set; } = null!;
        public string EmailAddress { get; set; } = null!;
        public string? PhoneNumber { get; set; }
        public string? Department { get; set; }
        public string? Specialization { get; set; }
        public string? OfficeLocation { get; set; }
        public string? ConsultationDescription { get; set; }
        public IReadOnlyList<SlotResponseDto> UpcomingSlots { get; set; } = Array.Empty<SlotResponseDto>();
    }
}
