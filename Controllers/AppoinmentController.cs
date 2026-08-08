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

        /* Phân quyền:
         * Chỉ những user có StudentId/LectureId trùng với 
         * StudentId/LectureId mới coi đc appoinment detail đó        
        */
        //[Authorize]
        [HttpGet("{id}")]
        public IActionResult GetAppointmentById(int id)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var roleClaim = User.FindFirst(ClaimTypes.Role)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || string.IsNullOrEmpty(roleClaim))
            {
                return BadRequest("User claims not found. Provide valid JWT or pass userId and role as query for testing.");
            }
            var userId = int.Parse(userIdClaim);
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
            if (string.IsNullOrEmpty(userIdClaim)) return BadRequest("Missing user claim");
            var userId = int.Parse(userIdClaim);

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
            if (string.IsNullOrEmpty(userIdClaim)) return BadRequest("Missing user claim");
            var userId = int.Parse(userIdClaim);

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
            if (string.IsNullOrEmpty(userIdClaim)) return BadRequest("Missing user claim");
            var userId = int.Parse(userIdClaim);

            var ok = _appointmentService.StudentCancelAppointment(id, userId, request.Reason, out var error);
            if (!ok) return BadRequest(new { message = error });
            return Ok(new { message = "Appointment cancelled" });
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
