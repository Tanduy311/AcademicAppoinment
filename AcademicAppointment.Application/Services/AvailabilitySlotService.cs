using AcademicAppointment.Application.DTOs.Slot;
using AcademicAppointment.Application.Interfaces;
using AcademicAppointment.Domain.Entities;

namespace AcademicAppointment.Application.Services
{
    public class AvailabilitySlotService : IAvailabilitySlotService
    {
        private readonly ILecturerRepository _lecturers;
        private readonly IAvailabilitySlotRepository _availabilitySlots;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IDateTimeProvider _dateTimeProvider;

        public AvailabilitySlotService(
            ILecturerRepository lecturers,
            IAvailabilitySlotRepository availabilitySlots,
            IUnitOfWork unitOfWork,
            IDateTimeProvider dateTimeProvider)
        {
            _lecturers = lecturers;
            _availabilitySlots = availabilitySlots;
            _unitOfWork = unitOfWork;
            _dateTimeProvider = dateTimeProvider;
        }

        public async Task<SlotResponseDto> CreateSlotAsync(int userId, string? lecturerClaimId, string? userNameClaim, CreateSlotDto dto)
        {
            int lecturerId;
            if (!string.IsNullOrEmpty(lecturerClaimId) && int.TryParse(lecturerClaimId, out int parsedLecturerId))
            {
                lecturerId = parsedLecturerId;
            }
            else
            {
                var lecturer = await _lecturers.GetByUserIdAsync(userId);
                if (lecturer == null) throw new InvalidOperationException("Không tìm thấy thông tin giảng viên.");
                lecturerId = lecturer.LecturerId;
            }

            bool isOverlap = await _availabilitySlots.ExistsOverlappingAsync(
                lecturerId,
                dto.StartTime,
                dto.EndTime);

            if (isOverlap)
            {
                throw new InvalidOperationException("Khung giờ này bị trùng với một lịch rảnh khác đã tạo.");
            }

            var slot = new AvailabilitySlot
            {
                LecturerId = lecturerId,
                StartTime = dto.StartTime,
                EndTime = dto.EndTime,
                MeetingType = dto.MeetingType,
                LocationOrLink = dto.LocationOrLink,
                IsAvailable = true,
                CreatedAt = _dateTimeProvider.UtcNow
            };

            _availabilitySlots.Add(slot);
            await _unitOfWork.SaveChangesAsync();

            return new SlotResponseDto
            {
                AvailabilitySlotId = slot.AvailabilitySlotId,
                LecturerId = slot.LecturerId,
                LecturerName = userNameClaim ?? "",
                StartTime = slot.StartTime,
                EndTime = slot.EndTime,
                MeetingType = slot.MeetingType,
                LocationOrLink = slot.LocationOrLink,
                IsAvailable = slot.IsAvailable,
                CreatedAt = slot.CreatedAt
            };
        }

        public async Task<List<SlotResponseDto>> GetMySlotsAsync(int userId)
        {
            var lecturer = await _lecturers.GetByUserIdAsync(userId);
            if (lecturer == null) throw new KeyNotFoundException("Không tìm thấy thông tin giảng viên.");

            var slots = await _availabilitySlots.GetByLecturerIdAsync(lecturer.LecturerId);
            return slots.Select(ToSlotResponseDto).ToList();
        }

        public async Task<List<SlotResponseDto>> GetSlotsByLecturerAsync(int lecturerId)
        {
            var slots = await _availabilitySlots.GetAvailableByLecturerIdAsync(
                lecturerId,
                _dateTimeProvider.UtcNow);

            return slots.Select(ToSlotResponseDto).ToList();
        }

        public async Task DeleteSlotAsync(int userId, int slotId)
        {
            var lecturer = await _lecturers.GetByUserIdAsync(userId);
            if (lecturer == null) throw new UnauthorizedAccessException();

            var slot = await _availabilitySlots.GetByIdAsync(slotId);
            if (slot == null) throw new KeyNotFoundException("Không tìm thấy khung giờ này.");

            if (slot.LecturerId != lecturer.LecturerId)
            {
                throw new UnauthorizedAccessException("Bạn không có quyền xóa khung giờ của giảng viên khác.");
            }

            if (!slot.IsAvailable)
            {
                throw new InvalidOperationException("Không thể xóa khung giờ này vì đã có sinh viên đặt lịch.");
            }

            _availabilitySlots.Remove(slot);
            await _unitOfWork.SaveChangesAsync();
        }

        private static SlotResponseDto ToSlotResponseDto(AvailabilitySlot slot)
        {
            return new SlotResponseDto
            {
                AvailabilitySlotId = slot.AvailabilitySlotId,
                LecturerId = slot.LecturerId,
                LecturerName = slot.Lecturer?.User?.FullName ?? "",
                Department = slot.Lecturer?.Department,
                StartTime = slot.StartTime,
                EndTime = slot.EndTime,
                MeetingType = slot.MeetingType,
                LocationOrLink = slot.LocationOrLink,
                IsAvailable = slot.IsAvailable,
                CreatedAt = slot.CreatedAt
            };
        }
    }
}

