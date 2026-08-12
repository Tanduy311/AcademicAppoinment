using AcademicAppoinment.DTOs.Lecturers;
using AcademicAppoinment.DTOs.Slot;
using AcademicAppoinment.Models;
using AcademicAppoinment.Repositories;
using AcademicAppoinment.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace AcademicAppoinment.Services
{
    public class LecturerService : ILecturerService
    {
        private readonly IAppRepository _repository;
        private readonly AppDbContext _context;

        public LecturerService(IAppRepository repository, AppDbContext context)
        {
            _repository = repository;
            _context = context;
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

        public async Task<LecturerDetailDto> GetMyProfileAsync(ClaimsPrincipal user)
        {
            var userId = GetCurrentUserId(user);
            var lecturer = await _repository.GetLecturerWithUserByUserIdAsync(userId);
            if (lecturer == null)
            {
                throw new KeyNotFoundException("Không tìm thấy thông tin giảng viên.");
            }

            return await GetLecturerByIdAsync(lecturer.LecturerId);
        }

        public async Task<LecturerDetailDto> UpdateMyProfileAsync(UpdateLecturerProfileDto dto, ClaimsPrincipal user)
        {
            var userId = GetCurrentUserId(user);
            var lecturer = await _context.Lecturers
                .Include(l => l.User)
                .FirstOrDefaultAsync(l => l.UserId == userId);

            if (lecturer == null || lecturer.User == null)
            {
                throw new KeyNotFoundException("Không tìm thấy thông tin giảng viên.");
            }

            if (!string.IsNullOrWhiteSpace(dto.EmailAddress))
            {
                var normalizedEmail = dto.EmailAddress.Trim();
                var emailExists = await _context.Users.AnyAsync(u =>
                    u.EmailAddress == normalizedEmail && u.UserId != userId);

                if (emailExists)
                {
                    throw new ArgumentException("Email đã được sử dụng.");
                }

                lecturer.User.EmailAddress = normalizedEmail;
            }

            if (!string.IsNullOrWhiteSpace(dto.FullName))
            {
                lecturer.User.FullName = dto.FullName.Trim();
            }

            if (dto.PhoneNumber != null)
            {
                lecturer.User.PhoneNumber = string.IsNullOrWhiteSpace(dto.PhoneNumber) ? null : dto.PhoneNumber.Trim();
            }

            if (dto.Department != null)
            {
                lecturer.Department = string.IsNullOrWhiteSpace(dto.Department) ? null : dto.Department.Trim();
            }

            if (dto.Specialization != null)
            {
                lecturer.Specialization = string.IsNullOrWhiteSpace(dto.Specialization) ? null : dto.Specialization.Trim();
            }

            if (dto.OfficeLocation != null)
            {
                lecturer.OfficeLocation = string.IsNullOrWhiteSpace(dto.OfficeLocation) ? null : dto.OfficeLocation.Trim();
            }

            if (dto.ConsultationDescription != null)
            {
                lecturer.ConsultationDescription = string.IsNullOrWhiteSpace(dto.ConsultationDescription) ? null : dto.ConsultationDescription.Trim();
            }

            await _repository.SaveChangesAsync();
            return await GetLecturerByIdAsync(lecturer.LecturerId);
        }

        private static int GetCurrentUserId(ClaimsPrincipal user)
        {
            var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdClaim, out var userId))
            {
                throw new UnauthorizedAccessException("Không tìm thấy thông tin tài khoản đăng nhập.");
            }
            return userId;
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
