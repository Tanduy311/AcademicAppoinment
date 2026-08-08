using AcademicAppoinment.DTOs.Appointments;
using AcademicAppoinment.DTOs.Students;
using AcademicAppoinment.Helpers.Exceptions;
using AcademicAppoinment.Models;
using AcademicAppoinment.Repositories;
using AcademicAppoinment.Services.Interfaces;
using System.Security.Claims;

namespace AcademicAppoinment.Services
{
    public class StudentService : IStudentService
    {
        private readonly IAppRepository _repository;

        public StudentService(IAppRepository repository)
        {
            _repository = repository;
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

        private static StudentDetailDto ToDetailDto(Student student)
        {
            return new StudentDetailDto
            {
                StudentId = student.StudentId,
                StudentCode = student.StudentCode,
                FullName = student.User?.FullName ?? "",
                EmailAddress = student.User?.EmailAddress ?? "",
                PhoneNumber = student.User?.PhoneNumber,
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
    }
}
