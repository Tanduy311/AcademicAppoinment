using AcademicAppoinment.DTOs.Appointments;
using AcademicAppoinment.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AcademicAppoinment.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AppointmentsController : ControllerBase
    {
        private readonly IAppointmentService _appointmentService;

        public AppointmentsController(IAppointmentService appointmentService)
        {
            _appointmentService = appointmentService;
        }

        [HttpPost]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> CreateAppointment([FromBody] CreateAppointmentDto dto)
        {
            var created = await _appointmentService.CreateAppointmentAsync(dto, User);
            return CreatedAtAction(nameof(GetAppointmentById), new { id = created.AppointmentId }, created);
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetAppointmentById(int id)
        {
            var result = await _appointmentService.GetAppointmentByIdAsync(id, User);
            return Ok(result);
        }

        [HttpGet("my-appointments")]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> GetMyAppointments()
        {
            var result = await _appointmentService.GetMyAppointmentsAsync(User);
            return Ok(result);
        }

        [HttpGet("lecturer-appointments")]
        [Authorize(Roles = "Lecturer")]
        public async Task<IActionResult> GetLecturerAppointments()
        {
            var result = await _appointmentService.GetLecturerAppointmentsAsync(User);
            return Ok(result);
        }

        [HttpPut("{id}/status")]
        [Authorize(Roles = "Lecturer")]
        public async Task<IActionResult> UpdateAppointmentStatus(int id, [FromBody] UpdateAppointmentStatusDto dto)
        {
            var result = await _appointmentService.UpdateAppointmentStatusAsync(id, dto, User);
            return Ok(result);
        }

        [HttpPut("{id}/cancel")]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> CancelAppointment(int id, [FromBody] CancelAppointmentDto dto)
        {
            var result = await _appointmentService.CancelAppointmentAsync(id, dto, User);
            return Ok(result);
        }
    }
}

