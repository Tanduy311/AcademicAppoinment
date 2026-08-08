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
            //Lấy claim UserId trong JWT
           var userId = int.Parse(
               User.FindFirst(ClaimTypes.NameIdentifier)!.Value
           );

            //Lấy role trong JWT
            var role = User.FindFirst(ClaimTypes.Role)?.Value;
            var appointment = _appointmentService.GetAppointmentByIdForUser(id, userId, role);
            
            if (appointment == null)
            {
                return NotFound("Appointment was not found");
            }

            return Ok(MapToDetailResponse(appointment));
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
