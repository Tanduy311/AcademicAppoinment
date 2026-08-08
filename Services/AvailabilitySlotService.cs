using AcademicAppoinment.DTOs.Slot;
using AcademicAppoinment.Helpers.Exceptions;
using AcademicAppoinment.Models;
using AcademicAppoinment.Repositories;
using AcademicAppoinment.Services.Interfaces;
using System.Security.Claims;

namespace AcademicAppoinment.Services
{
    public class AvailabilitySlotService : IAvailabilitySlotService
    {
        private readonly IAppRepository _repository;

        public AvailabilitySlotService(IAppRepository repository)
        {
            _repository = repository;
        }

        public async Task<SlotResponseDto> CreateSlotAsync(CreateSlotDto dto, ClaimsPrincipal user)
        {
            var lecturer = await GetCurrentLecturerAsync(user);
            if (lecturer == null)
            {
                throw new UnauthorizedAccessException("Không tìm thấy thông tin Giảng viên.");
            }

            var isOverlap = await _repository.HasSlotOverlapAsync(lecturer.LecturerId, dto.StartTime, dto.EndTime);
            if (isOverlap)
            {
                throw new ArgumentException("Khung giờ này bị trùng với một lịch rảnh khác đã tạo.");
            }

            var slot = new AvailabilitySlot
            {
                LecturerId = lecturer.LecturerId,
                StartTime = dto.StartTime,
                EndTime = dto.EndTime,
                MeetingType = dto.MeetingType,
                LocationOrLink = dto.LocationOrLink,
                IsAvailable = true,
                CreatedAt = DateTime.Now
            };

            _repository.AddSlot(slot);
            await _repository.SaveChangesAsync();

            return new SlotResponseDto
            {
                AvailabilitySlotId = slot.AvailabilitySlotId,
                LecturerId = slot.LecturerId,
                LecturerName = lecturer.User?.FullName ?? "",
                Department = lecturer.Department,
                StartTime = slot.StartTime,
                EndTime = slot.EndTime,
                MeetingType = slot.MeetingType,
                LocationOrLink = slot.LocationOrLink,
                IsAvailable = slot.IsAvailable,
                CreatedAt = slot.CreatedAt
            };
        }

        public async Task<IReadOnlyList<SlotResponseDto>> GetMySlotsAsync(ClaimsPrincipal user)
        {
            var lecturer = await GetCurrentLecturerAsync(user);
            if (lecturer == null)
            {
                throw new UnauthorizedAccessException("Không tìm thấy thông tin Giảng viên.");
            }

            var slots = await _repository.GetSlotsByLecturerIdAsync(lecturer.LecturerId);
            return slots.Select(ToSlotResponseDto).ToList();
        }

        public async Task<IReadOnlyList<SlotResponseDto>> GetSlotsByLecturerAsync(int lecturerId)
        {
            var slots = await _repository.GetSlotsByLecturerIdAsync(lecturerId);
            return slots
                .Where(s => s.IsAvailable && s.StartTime > DateTime.Now)
                .OrderBy(s => s.StartTime)
                .Select(ToSlotResponseDto)
                .ToList();
        }

        public async Task<string> DeleteSlotAsync(int id, ClaimsPrincipal user)
        {
            var lecturer = await GetCurrentLecturerAsync(user);
            if (lecturer == null)
            {
                throw new UnauthorizedAccessException("Không tìm thấy thông tin Giảng viên.");
            }

            var slot = await _repository.GetAvailabilitySlotWithLecturerAsync(id);
            if (slot == null)
            {
                throw new KeyNotFoundException("Không tìm thấy khung giờ này.");
            }

            if (slot.LecturerId != lecturer.LecturerId)
            {
                throw new ForbiddenAccessException("Bạn không có quyền xóa khung giờ của giảng viên khác.");
            }

            if (!slot.IsAvailable)
            {
                throw new ArgumentException("Không thể xóa khung giờ này vì đã có Sinh viên đặt lịch.");
            }

            _repository.RemoveSlot(slot);
            await _repository.SaveChangesAsync();

            return "Xóa khung giờ rảnh thành công.";
        }

        private async Task<Lecturer?> GetCurrentLecturerAsync(ClaimsPrincipal user)
        {
            var lecturerIdClaim = user.FindFirst("LecturerId")?.Value;
            if (int.TryParse(lecturerIdClaim, out var lecturerId))
            {
                return await _repository.GetLecturerWithUserByIdAsync(lecturerId);
            }

            var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdClaim, out var userId))
            {
                return null;
            }

            return await _repository.GetLecturerWithUserByUserIdAsync(userId);
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
