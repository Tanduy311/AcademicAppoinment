using AcademicAppoinment.DTOs.Appointments;
using AcademicAppoinment.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace AcademicAppoinment.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AppointmentsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AppointmentsController(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Sinh viên đặt lịch tư vấn từ một slot còn trống.
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> CreateAppointment([FromBody] CreateAppointmentDto dto)
        {
            var student = await GetCurrentStudentAsync();
            if (student == null)
            {
                return Unauthorized("Không tìm thấy thông tin sinh viên.");
            }

            var slot = await _context.AvailabilitySlots
                .Include(s => s.Lecturer)
                .ThenInclude(l => l!.User)
                .FirstOrDefaultAsync(s => s.AvailabilitySlotId == dto.AvailabilitySlotId);

            if (slot == null)
            {
                return NotFound("Không tìm thấy khung giờ rảnh.");
            }

            if (!slot.IsAvailable)
            {
                return BadRequest("Khung giờ này đã được đặt.");
            }

            if (slot.StartTime <= DateTime.Now)
            {
                return BadRequest("Không thể đặt lịch cho khung giờ đã qua.");
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

            await using var transaction = await _context.Database.BeginTransactionAsync();

            slot.IsAvailable = false;
            _context.Appointments.Add(appointment);
            await _context.SaveChangesAsync();

            _context.Notifications.Add(new Notification
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

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            var created = await AppointmentDetailQuery()
                .FirstAsync(a => a.AppointmentId == appointment.AppointmentId);

            return CreatedAtAction(
                nameof(GetAppointmentById),
                new { id = appointment.AppointmentId },
                ToAppointmentResponseDto(created));
        }

        /// <summary>
        /// Xem chi tiết một lịch hẹn.
        /// </summary>
        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetAppointmentById(int id)
        {
            var appointment = await AppointmentDetailQuery()
                .FirstOrDefaultAsync(a => a.AppointmentId == id);

            if (appointment == null)
            {
                return NotFound("Không tìm thấy lịch hẹn.");
            }

            if (!CanAccessAppointment(appointment))
            {
                return StatusCode(403, "Bạn không có quyền xem lịch hẹn này.");
            }

            return Ok(ToAppointmentResponseDto(appointment));
        }

        /// <summary>
        /// Sinh viên xem danh sách lịch hẹn của chính mình.
        /// </summary>
        [HttpGet("my-appointments")]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> GetMyAppointments()
        {
            var student = await GetCurrentStudentAsync();
            if (student == null)
            {
                return Unauthorized("Không tìm thấy thông tin sinh viên.");
            }

            var appointments = await AppointmentDetailQuery()
                .Where(a => a.StudentId == student.StudentId)
                .OrderByDescending(a => a.CreatedAt)
                .ToListAsync();

            return Ok(appointments.Select(ToAppointmentResponseDto));
        }

        /// <summary>
        /// Giảng viên xem danh sách lịch hẹn của mình.
        /// </summary>
        [HttpGet("lecturer-appointments")]
        [Authorize(Roles = "Lecturer")]
        public async Task<IActionResult> GetLecturerAppointments()
        {
            var lecturer = await GetCurrentLecturerAsync();
            if (lecturer == null)
            {
                return Unauthorized("Không tìm thấy thông tin giảng viên.");
            }

            var appointments = await AppointmentDetailQuery()
                .Where(a => a.LecturerId == lecturer.LecturerId)
                .OrderByDescending(a => a.CreatedAt)
                .ToListAsync();

            return Ok(appointments.Select(ToAppointmentResponseDto));
        }

        /// <summary>
        /// Giảng viên duyệt hoặc từ chối lịch hẹn.
        /// </summary>
        [HttpPut("{id}/status")]
        [Authorize(Roles = "Lecturer")]
        public async Task<IActionResult> UpdateAppointmentStatus(int id, [FromBody] UpdateAppointmentStatusDto dto)
        {
            var lecturer = await GetCurrentLecturerAsync();
            if (lecturer == null)
            {
                return Unauthorized("Không tìm thấy thông tin giảng viên.");
            }

            var appointment = await AppointmentDetailQuery()
                .FirstOrDefaultAsync(a => a.AppointmentId == id);

            if (appointment == null)
            {
                return NotFound("Không tìm thấy lịch hẹn.");
            }

            if (appointment.LecturerId != lecturer.LecturerId)
            {
                return StatusCode(403, "Bạn không có quyền cập nhật lịch hẹn này.");
            }

            if (!string.Equals(appointment.Status, "Pending", StringComparison.OrdinalIgnoreCase))
            {
                return BadRequest("Chỉ có thể cập nhật lịch hẹn đang ở trạng thái Pending.");
            }

            var normalizedStatus = NormalizeAppointmentStatus(dto.Status);
            if (normalizedStatus != "Confirmed" && normalizedStatus != "Rejected")
            {
                return BadRequest("Status chỉ được là Confirmed hoặc Rejected.");
            }

            if (normalizedStatus == "Rejected" && string.IsNullOrWhiteSpace(dto.LecturerResponse))
            {
                return BadRequest("Vui lòng nhập lý do từ chối lịch hẹn.");
            }

            appointment.Status = normalizedStatus;
            appointment.LecturerResponse = dto.LecturerResponse;
            appointment.UpdatedAt = DateTime.Now;

            _context.Notifications.Add(new Notification
            {
                UserId = appointment.Student!.UserId,
                StudentId = appointment.StudentId,
                LecturerId = appointment.LecturerId,
                AppointmentId = appointment.AppointmentId,
                Title = normalizedStatus == "Confirmed"
                    ? "Lịch hẹn đã được xác nhận"
                    : "Lịch hẹn đã bị từ chối",
                Message = normalizedStatus == "Confirmed"
                    ? $"Lịch tư vấn về chủ đề '{appointment.Topic}' đã được giảng viên xác nhận."
                    : $"Lịch tư vấn về chủ đề '{appointment.Topic}' đã bị từ chối. Phản hồi: {dto.LecturerResponse}",
                IsRead = false,
                CreatedAt = DateTime.Now
            });

            await _context.SaveChangesAsync();

            return Ok(ToAppointmentResponseDto(appointment));
        }

        /// <summary>
        /// Sinh viên hủy lịch hẹn của chính mình.
        /// </summary>
        [HttpPut("{id}/cancel")]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> CancelAppointment(int id, [FromBody] CancelAppointmentDto dto)
        {
            var student = await GetCurrentStudentAsync();
            if (student == null)
            {
                return Unauthorized("Không tìm thấy thông tin sinh viên.");
            }

            var appointment = await AppointmentDetailQuery()
                .FirstOrDefaultAsync(a => a.AppointmentId == id);

            if (appointment == null)
            {
                return NotFound("Không tìm thấy lịch hẹn.");
            }

            if (appointment.StudentId != student.StudentId)
            {
                return StatusCode(403, "Bạn không có quyền hủy lịch hẹn này.");
            }

            if (string.Equals(appointment.Status, "Cancelled", StringComparison.OrdinalIgnoreCase))
            {
                return BadRequest("Lịch hẹn này đã được hủy trước đó.");
            }

            if (string.Equals(appointment.Status, "Rejected", StringComparison.OrdinalIgnoreCase))
            {
                return BadRequest("Không thể hủy lịch hẹn đã bị từ chối.");
            }

            if (appointment.AvailabilitySlot != null && appointment.AvailabilitySlot.StartTime <= DateTime.Now)
            {
                return BadRequest("Không thể hủy lịch hẹn đã bắt đầu hoặc đã qua.");
            }

            appointment.Status = "Cancelled";
            appointment.CancellationReason = dto.CancellationReason;
            appointment.UpdatedAt = DateTime.Now;

            if (appointment.Lecturer != null)
            {
                _context.Notifications.Add(new Notification
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

            await _context.SaveChangesAsync();

            return Ok(ToAppointmentResponseDto(appointment));
        }

        private IQueryable<Appointment> AppointmentDetailQuery()
        {
            return _context.Appointments
                .Include(a => a.Student)
                    .ThenInclude(s => s!.User)
                .Include(a => a.Lecturer)
                    .ThenInclude(l => l!.User)
                .Include(a => a.AvailabilitySlot);
        }

        private async Task<Student?> GetCurrentStudentAsync()
        {
            var studentIdClaim = User.FindFirst("StudentId")?.Value;
            if (int.TryParse(studentIdClaim, out int studentId))
            {
                return await _context.Students
                    .Include(s => s.User)
                    .FirstOrDefaultAsync(s => s.StudentId == studentId);
            }

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdClaim, out int userId))
            {
                return null;
            }

            return await _context.Students
                .Include(s => s.User)
                .FirstOrDefaultAsync(s => s.UserId == userId);
        }

        private async Task<Lecturer?> GetCurrentLecturerAsync()
        {
            var lecturerIdClaim = User.FindFirst("LecturerId")?.Value;
            if (int.TryParse(lecturerIdClaim, out int lecturerId))
            {
                return await _context.Lecturers
                    .Include(l => l.User)
                    .FirstOrDefaultAsync(l => l.LecturerId == lecturerId);
            }

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdClaim, out int userId))
            {
                return null;
            }

            return await _context.Lecturers
                .Include(l => l.User)
                .FirstOrDefaultAsync(l => l.UserId == userId);
        }

        private bool CanAccessAppointment(Appointment appointment)
        {
            var role = User.FindFirst(ClaimTypes.Role)?.Value;
            if (role == "Admin")
            {
                return true;
            }

            var studentIdClaim = User.FindFirst("StudentId")?.Value;
            if (int.TryParse(studentIdClaim, out int studentId) && appointment.StudentId == studentId)
            {
                return true;
            }

            var lecturerIdClaim = User.FindFirst("LecturerId")?.Value;
            if (int.TryParse(lecturerIdClaim, out int lecturerId) && appointment.LecturerId == lecturerId)
            {
                return true;
            }

            return false;
        }

        private static string NormalizeAppointmentStatus(string status)
        {
            if (status.Equals("Confirmed", StringComparison.OrdinalIgnoreCase))
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
