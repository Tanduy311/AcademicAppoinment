namespace AcademicAppoinment.DTOs
{
    public class AppointmentDetailResponse
    {
        public int AppointmentId { get; set; }
        public string Topic { get; set; }
        public string? Description { get; set; }
        public string Status { get; set; }
        public string? LecturerResponse { get; set; }
        public string? CancellationReason { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public StudentResponse Student { get; set; }
        public LecturerResponseDto Lecturer { get; set; }
        public SlotResponse Slot { get; set; }
    }

    public class StudentResponse
    {
        public int StudentId { get; set; }
        public string StudentCode { get; set; }
        public string FullName { get; set; }
        public string EmailAddress { get; set; }
        public string? Major { get; set; }
        public string? ClassName { get; set; }
        public string? AcademicYear { get; set; }
    }

    public class LecturerResponseDto
    {
        public int LecturerId { get; set; }
        public string LecturerCode { get; set; }
        public string FullName { get; set; }
        public string EmailAddress { get; set; }
        public string? Department { get; set; }
        public string? Specialization { get; set; }
        public string? OfficeLocation { get; set; }
    }

    public class SlotResponse
    {
        public int AvailabilitySlotId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string MeetingType { get; set; }
        public string? LocationOrLink { get; set; }
    }
}
