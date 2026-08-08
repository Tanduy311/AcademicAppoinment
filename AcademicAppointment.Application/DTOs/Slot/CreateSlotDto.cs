using System.ComponentModel.DataAnnotations;

namespace AcademicAppointment.Application.DTOs.Slot
{
    public class CreateSlotDto : IValidatableObject
    {
        [Required(ErrorMessage = "Vui lòng chọn thời gian bắt đầu.")]
        public DateTime StartTime { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn thời gian kết thúc.")]
        public DateTime EndTime { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập hình thức gặp: Online hoặc Offline.")]
        [MaxLength(50)]
        public string MeetingType { get; set; } = null!;

        [MaxLength(500)]
        public string? LocationOrLink { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (StartTime < DateTime.UtcNow)
            {
                yield return new ValidationResult(
                    "Thời gian bắt đầu không được ở quá khứ.",
                    new[] { nameof(StartTime) });
            }

            if (EndTime <= StartTime)
            {
                yield return new ValidationResult(
                    "Thời gian kết thúc phải lớn hơn thời gian bắt đầu.",
                    new[] { nameof(EndTime) });
            }
        }
    }
}

