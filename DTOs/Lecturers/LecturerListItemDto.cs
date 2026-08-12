namespace AcademicAppoinment.DTOs.Lecturers
{
    public class LecturerListItemDto
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
        public int UpcomingSlotCount { get; set; }
    }
}
