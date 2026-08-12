using AcademicAppoinment.DTOs.Appointments;
using AcademicAppoinment.Helpers.Exceptions;
using AcademicAppoinment.Models;
using AcademicAppoinment.Repositories;
using AcademicAppoinment.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Security.Claims;

namespace AcademicAppoinment.Services
{
    public class AppointmentService : IAppointmentService
    {
        private readonly AppDbContext _context;
        private readonly IAppRepository _repository;

        public AppointmentService(AppDbContext context, IAppRepository repository)
        {
            _context = context;
            _repository = repository;
        }

        public async Task<AppointmentResponseDto> CreateAppointmentAsync(CreateAppointmentDto dto, ClaimsPrincipal user)
        {
            var student = await GetCurrentStudentAsync(user);
            if (student == null)
            {
                throw new UnauthorizedAccessException("Không tìm thấy thông tin sinh viên.");
            }

            await using var transaction = _context.Database.IsRelational()
                ? await _context.Database.BeginTransactionAsync(IsolationLevel.Serializable)
                : null;

            var slot = await _repository.GetAvailabilitySlotWithLecturerAsync(dto.AvailabilitySlotId);
            if (slot == null)
            {
                throw new KeyNotFoundException("Không tìm thấy khung giờ rảnh.");
            }

            if (slot.IsDeleted)
            {
                throw new KeyNotFoundException("Khung giờ không tồn tại.");
            }

            var hasActiveAppointment = await _context.Appointments
                .AnyAsync(a => a.AvailabilitySlotId == dto.AvailabilitySlotId && (a.Status == "Pending" || a.Status == "Confirmed"));

            if (!slot.IsAvailable || hasActiveAppointment)
            {
                throw new ArgumentException("Khung giờ này đã được người khác đặt lịch. Vui lòng chọn khung giờ khác.");
            }

            if (slot.StartTime <= DateTime.Now)
            {
                throw new ArgumentException("Không thể đặt lịch cho khung giờ đã qua.");
            }

            var appointment = new Appointment
            {
                StudentId = student.StudentId,
                LecturerId = slot.LecturerId,
                AvailabilitySlotId = slot.AvailabilitySlotId,
                Topic = dto.Topic,
                Description = dto.Description,
                Status = "Pending",
                CreatedAt = DateTime.Now
            };

            slot.IsAvailable = false;
            _repository.AddAppointment(appointment);
            try
            {
                await _repository.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                throw new ArgumentException("Khung giờ này đã được đặt lịch. Vui lòng chọn khung giờ khác.", ex);
            }

            _repository.AddNotification(new Notification
            {
                UserId = slot.Lecturer!.UserId,
                StudentId = student.StudentId,
                LecturerId = slot.LecturerId,
                AppointmentId = appointment.AppointmentId,
                Title = "Có lịch hẹn tư vấn mới",
                Message = $"{student.User?.FullName ?? student.User?.AccountName ?? "Sinh viên"} đã đặt lịch tư vấn với chủ đề: {appointment.Topic}.",
                IsRead = false,
                CreatedAt = DateTime.Now
            });

            await _repository.SaveChangesAsync();
            if (transaction != null)
            {
                await transaction.CommitAsync();
            }

            var created = await _repository.GetAppointmentDetailByIdAsync(appointment.AppointmentId);
            if (created == null)
            {
                throw new KeyNotFoundException("Không thể tải lại lịch hẹn vừa tạo.");
            }

            return ToAppointmentResponseDto(created);
        }

        public async Task<AppointmentResponseDto> GetAppointmentByIdAsync(int id, ClaimsPrincipal user)
        {
            var appointment = await _repository.GetAppointmentDetailByIdAsync(id);
            if (appointment == null)
            {
                throw new KeyNotFoundException("Không tìm thấy lịch hẹn.");
            }

            if (!CanAccessAppointment(appointment, user))
            {
                throw new ForbiddenAccessException("Bạn không có quyền xem lịch hẹn này.");
            }

            return ToAppointmentResponseDto(appointment);
        }

        public async Task<IReadOnlyList<AppointmentResponseDto>> GetMyAppointmentsAsync(ClaimsPrincipal user)
        {
            var student = await GetCurrentStudentAsync(user);
            if (student == null)
            {
                throw new UnauthorizedAccessException("Không tìm thấy thông tin sinh viên.");
            }

            var appointments = await _repository.GetAppointmentsByStudentIdAsync(student.StudentId);
            return appointments.Select(ToAppointmentResponseDto).ToList();
        }

        public async Task<IReadOnlyList<AppointmentResponseDto>> GetLecturerAppointmentsAsync(ClaimsPrincipal user)
        {
            var lecturer = await GetCurrentLecturerAsync(user);
            if (lecturer == null)
            {
                throw new UnauthorizedAccessException("Không tìm thấy thông tin giảng viên.");
            }

            var appointments = await _repository.GetAppointmentsByLecturerIdAsync(lecturer.LecturerId);
            return appointments.Select(ToAppointmentResponseDto).ToList();
        }

        public async Task<AppointmentResponseDto> UpdateAppointmentStatusAsync(int id, UpdateAppointmentStatusDto dto, ClaimsPrincipal user)
        {
            var lecturer = await GetCurrentLecturerAsync(user);
            if (lecturer == null)
            {
                throw new UnauthorizedAccessException("Không tìm thấy thông tin giảng viên.");
            }

            var appointment = await _repository.GetAppointmentDetailByIdAsync(id);
            if (appointment == null)
            {
                throw new KeyNotFoundException("Không tìm thấy lịch hẹn.");
            }

            if (appointment.LecturerId != lecturer.LecturerId)
            {
                throw new ForbiddenAccessException("Bạn không có quyền cập nhật lịch hẹn này.");
            }

            if (!string.Equals(appointment.Status, "Pending", StringComparison.OrdinalIgnoreCase) &&
                !string.Equals(appointment.Status, "Confirmed", StringComparison.OrdinalIgnoreCase) &&
                !string.Equals(appointment.Status, "Approved", StringComparison.OrdinalIgnoreCase))
            {
                throw new ArgumentException("Chỉ có thể cập nhật lịch hẹn đang ở trạng thái Pending hoặc Confirmed.");
            }

            var normalizedStatus = NormalizeAppointmentStatus(dto.Status);
            if (normalizedStatus != "Confirmed" && normalizedStatus != "Rejected" && normalizedStatus != "Cancelled")
            {
                throw new ArgumentException("Status chỉ được là Confirmed, Rejected hoặc Cancelled.");
            }

            if ((normalizedStatus == "Rejected" || normalizedStatus == "Cancelled") && string.IsNullOrWhiteSpace(dto.LecturerResponse))
            {
                throw new ArgumentException("Vui lòng nhập lý do từ chối/hủy lịch hẹn.");
            }

            appointment.Status = normalizedStatus;
            appointment.LecturerResponse = dto.LecturerResponse;
            if (normalizedStatus == "Cancelled")
            {
                appointment.CancellationReason = dto.LecturerResponse;
            }
            appointment.UpdatedAt = DateTime.Now;

            if ((normalizedStatus == "Rejected" || normalizedStatus == "Cancelled") && appointment.AvailabilitySlot != null)
            {
                appointment.AvailabilitySlot.IsAvailable = true;
            }

            _repository.AddNotification(new Notification
            {
                UserId = appointment.Student!.UserId,
                StudentId = appointment.StudentId,
                LecturerId = appointment.LecturerId,
                AppointmentId = appointment.AppointmentId,
                Title = normalizedStatus == "Confirmed"
                    ? "Lịch hẹn đã được xác nhận"
                    : normalizedStatus == "Cancelled"
                    ? "Lịch hẹn đã bị hủy bởi giảng viên"
                    : "Lịch hẹn đã bị từ chối",
                Message = normalizedStatus == "Confirmed"
                    ? $"Lịch tư vấn về chủ đề '{appointment.Topic}' đã được giảng viên xác nhận."
                    : normalizedStatus == "Cancelled"
                    ? $"Lịch tư vấn về chủ đề '{appointment.Topic}' đã bị hủy bởi giảng viên. Lời nhắn: {dto.LecturerResponse}"
                    : $"Lịch tư vấn về chủ đề '{appointment.Topic}' đã bị từ chối. Phản hồi: {dto.LecturerResponse}",
                IsRead = false,
                CreatedAt = DateTime.Now
            });

            await _repository.SaveChangesAsync();
            return ToAppointmentResponseDto(appointment);
        }

        public async Task<AppointmentResponseDto> CancelAppointmentAsync(int id, CancelAppointmentDto dto, ClaimsPrincipal user)
        {
            var student = await GetCurrentStudentAsync(user);
            if (student == null)
            {
                throw new UnauthorizedAccessException("Không tìm thấy thông tin sinh viên.");
            }

            var appointment = await _repository.GetAppointmentDetailByIdAsync(id);
            if (appointment == null)
            {
                throw new KeyNotFoundException("Không tìm thấy lịch hẹn.");
            }

            if (appointment.StudentId != student.StudentId)
            {
                throw new ForbiddenAccessException("Bạn không có quyền hủy lịch hẹn này.");
            }

            if (string.Equals(appointment.Status, "Cancelled", StringComparison.OrdinalIgnoreCase))
            {
                throw new ArgumentException("Lịch hẹn này đã được hủy trước đó.");
            }

            if (string.Equals(appointment.Status, "Rejected", StringComparison.OrdinalIgnoreCase))
            {
                throw new ArgumentException("Không thể hủy lịch hẹn đã bị từ chối.");
            }

            if (appointment.AvailabilitySlot != null && appointment.AvailabilitySlot.StartTime <= DateTime.Now)
            {
                throw new ArgumentException("Không thể hủy lịch hẹn đã bắt đầu hoặc đã qua.");
            }

            appointment.Status = "Cancelled";
            appointment.CancellationReason = dto.CancellationReason;
            appointment.UpdatedAt = DateTime.Now;

            if (appointment.AvailabilitySlot != null)
            {
                appointment.AvailabilitySlot.IsAvailable = true;
            }

            if (appointment.Lecturer != null)
            {
                _repository.AddNotification(new Notification
                {
                    UserId = appointment.Lecturer.UserId,
                    StudentId = appointment.StudentId,
                    LecturerId = appointment.LecturerId,
                    AppointmentId = appointment.AppointmentId,
                    Title = "Sinh viên đã hủy lịch hẹn",
                    Message = $"{appointment.Student?.User?.FullName ?? student.User?.FullName ?? "Sinh viên"} đã hủy lịch tư vấn về chủ đề: {appointment.Topic}.",
                    IsRead = false,
                    CreatedAt = DateTime.Now
                });
            }

            await _repository.SaveChangesAsync();
            return ToAppointmentResponseDto(appointment);
        }

        private async Task<Student?> GetCurrentStudentAsync(ClaimsPrincipal user)
        {
            var studentIdClaim = user.FindFirst("StudentId")?.Value;
            if (int.TryParse(studentIdClaim, out var studentId))
            {
                return await _repository.GetStudentWithUserByIdAsync(studentId);
            }

            var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdClaim, out var userId))
            {
                return null;
            }

            return await _repository.GetStudentWithUserByUserIdAsync(userId);
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

        private static bool CanAccessAppointment(Appointment appointment, ClaimsPrincipal user)
        {
            var role = user.FindFirst(ClaimTypes.Role)?.Value;
            if (role == "Admin")
            {
                return true;
            }

            var studentIdClaim = user.FindFirst("StudentId")?.Value;
            if (int.TryParse(studentIdClaim, out var studentId) && appointment.StudentId == studentId)
            {
                return true;
            }

            var lecturerIdClaim = user.FindFirst("LecturerId")?.Value;
            if (int.TryParse(lecturerIdClaim, out var lecturerId) && appointment.LecturerId == lecturerId)
            {
                return true;
            }

            return false;
        }

        private static string NormalizeAppointmentStatus(string status)
        {
            if (status.Equals("Confirmed", StringComparison.OrdinalIgnoreCase) || status.Equals("Approved", StringComparison.OrdinalIgnoreCase))
            {
                return "Confirmed";
            }

            if (status.Equals("Rejected", StringComparison.OrdinalIgnoreCase))
            {
                return "Rejected";
            }

            if (status.Equals("Cancelled", StringComparison.OrdinalIgnoreCase))
            {
                return "Cancelled";
            }

            if (status.Equals("Pending", StringComparison.OrdinalIgnoreCase))
            {
                return "Pending";
            }

            return status;
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
    }
}
