using AcademicAppoinment.DTOs;
using AcademicAppoinment.Models;
using AcademicAppoinment.Services.Appoiment;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace AcademicAppoinment.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AppoinmentController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IAppointmentService _appointmentService;

        public AppoinmentController(AppDbContext context, IAppointmentService appointmentService)
        {
            _context = context;
            _appointmentService = appointmentService;
        }

        [HttpGet("{id}")]
        public IActionResult GetAppointmentById(int id)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var roleClaim = User.FindFirst(ClaimTypes.Role)?.Value;
            if (!int.TryParse(userIdClaim, out var userId) || string.IsNullOrEmpty(roleClaim))
            {
                return BadRequest("Missing or invalid user claims");
            }
            var role = roleClaim;

            var appointment = _appointmentService.GetAppointmentByIdForUser(id, userId, role);
            
            if (appointment == null)
            {
                return NotFound("Appointment was not found");
            }

            return Ok(MapToDetailResponse(appointment));
        }

        [HttpPost]
        public IActionResult CreateAppointment([FromBody] DTOs.CreateAppointmentRequest request)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdClaim, out var userId)) return BadRequest("Missing or invalid user claim");

            try
            {
                var appointment = _appointmentService.CreateAppointment(userId, request);
                return Ok(new { appointmentId = appointment.AppointmentId, message = "Appointment created" });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "An error occurred while creating appointment." });
            }
        }

        [HttpGet("/api/appointments/student")]
        public IActionResult GetStudentAppointments([FromQuery] string? status, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdClaim, out var userId)) return BadRequest("Missing or invalid user claim");

            var items = _appointmentService.GetAppointmentsForStudent(userId, status, page, pageSize);
            var result = items.Select(a => new {
                a.AppointmentId,
                a.Topic,
                a.Description,
                a.Status,
                a.CreatedAt,
                Lecturer = new { a.Lecturer.LecturerId, FullName = a.Lecturer.User?.FullName },
                Slot = new { a.AvailabilitySlot.AvailabilitySlotId, a.AvailabilitySlot.StartTime, a.AvailabilitySlot.EndTime }
            });

            return Ok(result);
        }

        [HttpPost("{id}/student-cancel")]
        public IActionResult StudentCancel(int id, [FromBody] DTOs.CancelAppointmentRequest request)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdClaim, out var userId)) return BadRequest("Missing or invalid user claim");

            var ok = _appointmentService.StudentCancelAppointment(id, userId, request.Reason, out var error);
            if (!ok) return BadRequest(new { message = error });
            return Ok(new { message = "Appointment cancelled" });
        }

        /// <summary>
        /// Get pending appointments for lecturer
        /// </summary>
        [HttpGet("/api/appointments/lecturer/pending")]
        [Authorize(Policy = "LecturerOnly")]
        public IActionResult GetLecturerPendingAppointments([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdClaim, out var userId)) return BadRequest("Missing or invalid user claim");

            var appointments = _appointmentService.GetPendingAppointmentsForLecturer(userId, page, pageSize);
            var result = appointments.Select(a => new
            {
                a.AppointmentId,
                a.Topic,
                a.Description,
                a.Status,
                a.CreatedAt,
                Student = new { a.Student.StudentId, FullName = a.Student.User?.FullName, a.Student.StudentCode },
                Slot = new { a.AvailabilitySlot.AvailabilitySlotId, a.AvailabilitySlot.StartTime, a.AvailabilitySlot.EndTime, a.AvailabilitySlot.MeetingType }
            });

            return Ok(result);
        }

        /// <summary>
        /// Approve an appointment (lecturer only)
        /// </summary>
        [HttpPost("{id}/approve")]
        [Authorize(Policy = "LecturerOnly")]
        public IActionResult ApproveAppointment(int id)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdClaim, out var userId)) return BadRequest("Missing or invalid user claim");

            bool ok = _appointmentService.ApproveAppointment(id, userId, out var error);
            if (!ok)
            {
                if (error.Contains("can only approve"))
                {
                    return Forbid();
                }
                return BadRequest(new { message = error });
            }

            return Ok(new { message = "Appointment approved" });
        }

        /// <summary>
        /// Reject an appointment with required reason (lecturer only)
        /// </summary>
        [HttpPost("{id}/reject")]
        [Authorize(Policy = "LecturerOnly")]
        public IActionResult RejectAppointment(int id, [FromBody] RejectAppointmentRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdClaim, out var userId)) return BadRequest("Missing or invalid user claim");

            bool ok = _appointmentService.RejectAppointment(id, userId, request.Reason, out var error);
            if (!ok)
            {
                if (error.Contains("can only reject"))
                {
                    return Forbid();
                }
                return BadRequest(new { message = error });
            }

            return Ok(new { message = "Appointment rejected" });
        }

        /// <summary>
        /// Cancel an appointment as lecturer (lecturer only)
        /// </summary>
        [HttpPost("{id}/lecturer-cancel")]
        [Authorize(Policy = "LecturerOnly")]
        public IActionResult LecturerCancelAppointment(int id, [FromBody] CancelAppointmentByLecturerRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdClaim, out var userId)) return BadRequest("Missing or invalid user claim");

            bool ok = _appointmentService.LecturerCancelAppointment(id, userId, request.Reason, out var error);
            if (!ok)
            {
                if (error.Contains("can only cancel"))
                {
                    return Forbid();
                }
                return BadRequest(new { message = error });
            }

            return Ok(new { message = "Appointment cancelled" });
        }

        /// <summary>
        /// Mark an appointment as completed (lecturer only)
        /// </summary>
        [HttpPost("{id}/complete")]
        [Authorize(Policy = "LecturerOnly")]
        public IActionResult CompleteAppointment(int id)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdClaim, out var userId)) return BadRequest("Missing or invalid user claim");

            bool ok = _appointmentService.CompleteAppointment(id, userId, out var error);
            if (!ok)
            {
                if (error.Contains("can only complete"))
                {
                    return Forbid();
                }
                return BadRequest(new { message = error });
            }

            return Ok(new { message = "Appointment completed" });
        }

        private AppointmentDetailResponse MapToDetailResponse(Appointment appointment)
        {
            return new AppointmentDetailResponse
            {
                AppointmentId = appointment.AppointmentId,
                Topic = appointment.Topic,
                Description = appointment.Description,
                Status = appointment.Status,
                LecturerResponse = appointment.LecturerResponse,
                CancellationReason = appointment.CancellationReason,
                CreatedAt = appointment.CreatedAt,
                UpdatedAt = appointment.UpdatedAt,

                Student = new StudentResponse
                {
                    StudentId = appointment.Student.StudentId,
                    StudentCode = appointment.Student.StudentCode,
                    FullName = appointment.Student.User.FullName,
                    EmailAddress = appointment.Student.User.EmailAddress,
                    Major = appointment.Student.Major,
                    ClassName = appointment.Student.ClassName,
                    AcademicYear = appointment.Student.AcademicYear
                },

                Lecturer = new LecturerResponseDto
                {
                    LecturerId = appointment.Lecturer.LecturerId,
                    LecturerCode = appointment.Lecturer.LecturerCode,
                    FullName = appointment.Lecturer.User.FullName,
                    EmailAddress = appointment.Lecturer.User.EmailAddress,
                    Department = appointment.Lecturer.Department,
                    Specialization = appointment.Lecturer.Specialization,
                    OfficeLocation = appointment.Lecturer.OfficeLocation
                },

                Slot = new SlotResponse
                {
                    AvailabilitySlotId = appointment.AvailabilitySlot.AvailabilitySlotId,
                    StartTime = appointment.AvailabilitySlot.StartTime,
                    EndTime = appointment.AvailabilitySlot.EndTime,
                    MeetingType = appointment.AvailabilitySlot.MeetingType,
                    LocationOrLink = appointment.AvailabilitySlot.LocationOrLink
                }
            };
        }
    }
}
