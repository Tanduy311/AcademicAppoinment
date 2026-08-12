using AcademicAppoinment.DTOs.Appointments;
using AcademicAppoinment.DTOs.Students;
using AcademicAppoinment.Helpers.Exceptions;
using AcademicAppoinment.Models;
using AcademicAppoinment.Repositories;
using AcademicAppoinment.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace AcademicAppoinment.Services
{
    public class StudentService : IStudentService
    {
        private readonly IAppRepository _repository;
        private readonly AppDbContext _context;

        public StudentService(IAppRepository repository, AppDbContext context)
        {
            _repository = repository;
            _context = context;
        }

        public async Task<IReadOnlyList<StudentListItemDto>> GetStudentsAsync()
        {
            var students = await _repository.GetStudentsAsync();
            return students.Select(s => new StudentListItemDto
            {
                StudentId = s.StudentId,
                StudentCode = s.StudentCode,
                FullName = s.User?.FullName ?? "",
                EmailAddress = s.User?.EmailAddress ?? "",
                PhoneNumber = s.User?.PhoneNumber,
                Major = s.Major,
                ClassName = s.ClassName,
                AcademicYear = s.AcademicYear,
                AppointmentCount = s.Appointments.Count
            }).ToList();
        }

        public async Task<StudentDetailDto> GetStudentByIdAsync(int studentId, ClaimsPrincipal user)
        {
            var currentUserId = GetCurrentUserId(user);
            var isAdmin = IsAdmin(user);

            if (!isAdmin)
            {
                var currentStudent = await _repository.GetStudentWithUserByUserIdAsync(currentUserId);
                if (currentStudent == null || currentStudent.StudentId != studentId)
                {
                    throw new ForbiddenAccessException("Bạn không có quyền xem sinh viên này.");
                }
            }

            var student = await _repository.GetStudentByIdWithDetailsAsync(studentId);
            if (student == null)
            {
                throw new KeyNotFoundException("Không tìm thấy sinh viên.");
            }

            return ToDetailDto(student);
        }

        public async Task<StudentDetailDto> GetMyProfileAsync(ClaimsPrincipal user)
        {
            var currentUserId = GetCurrentUserId(user);
            var student = await _repository.GetStudentWithUserByUserIdAsync(currentUserId);
            if (student == null)
            {
                throw new KeyNotFoundException("Không tìm thấy thông tin sinh viên.");
            }

            return ToDetailDto(await _repository.GetStudentByIdWithDetailsAsync(student.StudentId) ?? student);
        }

        public async Task<StudentDetailDto> UpdateMyProfileAsync(UpdateStudentProfileDto dto, ClaimsPrincipal user)
        {
            var currentUserId = GetCurrentUserId(user);
            var student = await _context.Students
                .Include(s => s.User)
                .FirstOrDefaultAsync(s => s.UserId == currentUserId);

            if (student == null || student.User == null)
            {
                throw new KeyNotFoundException("Không tìm thấy thông tin sinh viên.");
            }

            ValidateUpdate(dto);

            if (!string.IsNullOrWhiteSpace(dto.EmailAddress))
            {
                var normalizedEmail = dto.EmailAddress.Trim();
                var emailExists = await _context.Users.AnyAsync(u =>
                    u.EmailAddress == normalizedEmail && u.UserId != currentUserId);

                if (emailExists)
                {
                    throw new ArgumentException("Email đã được sử dụng.");
                }

                student.User.EmailAddress = normalizedEmail;
            }

            if (!string.IsNullOrWhiteSpace(dto.FullName))
            {
                student.User.FullName = dto.FullName.Trim();
            }

            if (dto.PhoneNumber != null)
            {
                student.User.PhoneNumber = string.IsNullOrWhiteSpace(dto.PhoneNumber) ? null : dto.PhoneNumber.Trim();
            }

            if (dto.Major != null)
            {
                student.Major = string.IsNullOrWhiteSpace(dto.Major) ? null : dto.Major.Trim();
            }

            if (dto.ClassName != null)
            {
                student.ClassName = string.IsNullOrWhiteSpace(dto.ClassName) ? null : dto.ClassName.Trim();
            }

            if (dto.AcademicYear != null)
            {
                student.AcademicYear = string.IsNullOrWhiteSpace(dto.AcademicYear) ? null : dto.AcademicYear.Trim();
            }

            await _repository.SaveChangesAsync();

            var refreshed = await _repository.GetStudentByIdWithDetailsAsync(student.StudentId);
            return ToDetailDto(refreshed ?? student);
        }

        private static StudentDetailDto ToDetailDto(Student student)
        {
            return new StudentDetailDto
            {
                StudentId = student.StudentId,
                StudentCode = student.StudentCode,
                FullName = student.User?.FullName ?? "",
                EmailAddress = student.User?.EmailAddress ?? "",
                PhoneNumber = student.User?.PhoneNumber,
                AvatarUrl = student.User?.AvatarUrl,
                Major = student.Major,
                ClassName = student.ClassName,
                AcademicYear = student.AcademicYear,
                Appointments = student.Appointments
                    .OrderByDescending(a => a.CreatedAt)
                    .Select(ToAppointmentResponseDto)
                    .ToList()
            };
        }

        private static AppointmentResponseDto ToAppointmentResponseDto(Appointment appointment)
        {
            return new AppointmentResponseDto
            {
                AppointmentId = appointment.AppointmentId,
                StudentId = appointment.StudentId,
                StudentName = appointment.Student?.User?.FullName ?? "",
                StudentCode = appointment.Student?.StudentCode,
                LecturerId = appointment.LecturerId,
                LecturerName = appointment.Lecturer?.User?.FullName ?? "",
                Department = appointment.Lecturer?.Department,
                AvailabilitySlotId = appointment.AvailabilitySlotId,
                StartTime = appointment.AvailabilitySlot?.StartTime ?? default,
                EndTime = appointment.AvailabilitySlot?.EndTime ?? default,
                MeetingType = appointment.AvailabilitySlot?.MeetingType ?? "",
                LocationOrLink = appointment.AvailabilitySlot?.LocationOrLink,
                Topic = appointment.Topic,
                Description = appointment.Description,
                Status = appointment.Status,
                LecturerResponse = appointment.LecturerResponse,
                CancellationReason = appointment.CancellationReason,
                CreatedAt = appointment.CreatedAt,
                UpdatedAt = appointment.UpdatedAt
            };
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

        private static bool IsAdmin(ClaimsPrincipal user)
        {
            return string.Equals(user.FindFirst(ClaimTypes.Role)?.Value, "Admin", StringComparison.OrdinalIgnoreCase);
        }

        private static void ValidateUpdate(UpdateStudentProfileDto dto)
        {
            if (dto.FullName != null && string.IsNullOrWhiteSpace(dto.FullName))
            {
                throw new ArgumentException("Họ tên không được để trống.");
            }

            if (dto.EmailAddress != null && string.IsNullOrWhiteSpace(dto.EmailAddress))
            {
                throw new ArgumentException("Email không được để trống.");
            }
        }
    }
}
