namespace AcademicAppoinment.DTOs
{
    public class AvailabilitySlotResponse
    {
        public int AvailabilitySlotId { get; set; }
        public int LecturerId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string MeetingType { get; set; } = null!;
        public string? LocationOrLink { get; set; }
        public bool IsAvailable { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
