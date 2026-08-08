using AcademicAppoinment.DTOs.Lecturers;
using AcademicAppoinment.DTOs.Slot;
using AcademicAppoinment.Models;
using AcademicAppoinment.Repositories;
using AcademicAppoinment.Services.Interfaces;

namespace AcademicAppoinment.Services
{
    public class LecturerService : ILecturerService
    {
        private readonly IAppRepository _repository;

        public LecturerService(IAppRepository repository)
        {
            _repository = repository;
        }

        public async Task<IReadOnlyList<LecturerListItemDto>> GetLecturersAsync()
        {
            var lecturers = await _repository.GetLecturersAsync();
            var now = DateTime.Now;

            return lecturers.Select(l => new LecturerListItemDto
            {
                LecturerId = l.LecturerId,
                LecturerCode = l.LecturerCode,
                FullName = l.User?.FullName ?? "",
                EmailAddress = l.User?.EmailAddress ?? "",
                PhoneNumber = l.User?.PhoneNumber,
                Department = l.Department,
                Specialization = l.Specialization,
                OfficeLocation = l.OfficeLocation,
                ConsultationDescription = l.ConsultationDescription,
                UpcomingSlotCount = l.AvailabilitySlots.Count(s => s.IsAvailable && s.StartTime > now)
            }).ToList();
        }

        public async Task<LecturerDetailDto> GetLecturerByIdAsync(int lecturerId)
        {
            var lecturer = await _repository.GetLecturerByIdWithDetailsAsync(lecturerId);
            if (lecturer == null)
            {
                throw new KeyNotFoundException("Không tìm thấy giảng viên.");
            }

            var upcomingSlots = lecturer.AvailabilitySlots
                .Where(s => s.IsAvailable && s.StartTime > DateTime.Now)
                .OrderBy(s => s.StartTime)
                .Select(ToSlotResponseDto)
                .ToList();

            return new LecturerDetailDto
            {
                LecturerId = lecturer.LecturerId,
                LecturerCode = lecturer.LecturerCode,
                FullName = lecturer.User?.FullName ?? "",
                EmailAddress = lecturer.User?.EmailAddress ?? "",
                PhoneNumber = lecturer.User?.PhoneNumber,
                Department = lecturer.Department,
                Specialization = lecturer.Specialization,
                OfficeLocation = lecturer.OfficeLocation,
                ConsultationDescription = lecturer.ConsultationDescription,
                UpcomingSlots = upcomingSlots
            };
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
