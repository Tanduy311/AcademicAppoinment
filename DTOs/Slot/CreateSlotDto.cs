using System.ComponentModel.DataAnnotations;

namespace AcademicAppoinment.DTOs.Slot
{
    public class CreateSlotDto : IValidatableObject
    {
        [Required(ErrorMessage = "Vui lòng chọn thời gian bắt đầu.")]
        public DateTime StartTime { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn thời gian kết thúc.")]
        public DateTime EndTime { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập hình thức gặp (Online hoặc Offline).")]
        [MaxLength(50)]
        public string MeetingType { get; set; } = null!; // "Online" hoặc "Offline"

        [MaxLength(500)]
        public string? LocationOrLink { get; set; } // Địa điểm phòng học hoặc Link Google Meet/Teams

        // Kiểm tra logic thời gian hợp lệ
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (StartTime < DateTime.Now)
            {
                yield return new ValidationResult("Thời gian bắt đầu không được ở quá khứ.", new[] { nameof(StartTime) });
            }

            if (EndTime <= StartTime)
            {
                yield return new ValidationResult("Thời gian kết thúc phải lớn hơn thời gian bắt đầu.", new[] { nameof(EndTime) });
            }
        }
    }
}
