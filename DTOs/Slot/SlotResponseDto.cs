namespace AcademicAppoinment.DTOs.Slot
{
    public class SlotResponseDto
    {
        public int AvailabilitySlotId { get; set; }
        public int LecturerId { get; set; }
        public string LecturerName { get; set; } = null!;
        public string? Department { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string MeetingType { get; set; } = null!;
        public string? LocationOrLink { get; set; }
        public bool IsAvailable { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
