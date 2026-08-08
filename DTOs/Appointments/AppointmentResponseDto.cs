namespace AcademicAppoinment.DTOs.Appointments
{
    public class AppointmentResponseDto
    {
        public int AppointmentId { get; set; }

        public int StudentId { get; set; }
        public string StudentName { get; set; } = null!;
        public string? StudentCode { get; set; }

        public int LecturerId { get; set; }
        public string LecturerName { get; set; } = null!;
        public string? Department { get; set; }

        public int AvailabilitySlotId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string MeetingType { get; set; } = null!;
        public string? LocationOrLink { get; set; }

        public string Topic { get; set; } = null!;
        public string? Description { get; set; }

        public string Status { get; set; } = null!;
        public string? LecturerResponse { get; set; }
        public string? CancellationReason { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
