using AcademicAppoinment.DTOs.Appointments;
using AcademicAppoinment.Helpers.Exceptions;
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
            return await HandleActionAsync(async () =>
            {
                var created = await _appointmentService.CreateAppointmentAsync(dto, User);
                return CreatedAtAction(nameof(GetAppointmentById), new { id = created.AppointmentId }, created);
            });
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetAppointmentById(int id)
        {
            return await HandleOkAsync(() => _appointmentService.GetAppointmentByIdAsync(id, User));
        }

        [HttpGet("my-appointments")]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> GetMyAppointments()
        {
            return await HandleOkAsync(() => _appointmentService.GetMyAppointmentsAsync(User));
        }

        [HttpGet("lecturer-appointments")]
        [Authorize(Roles = "Lecturer")]
        public async Task<IActionResult> GetLecturerAppointments()
        {
            return await HandleOkAsync(() => _appointmentService.GetLecturerAppointmentsAsync(User));
        }

        [HttpPut("{id}/status")]
        [Authorize(Roles = "Lecturer")]
        public async Task<IActionResult> UpdateAppointmentStatus(int id, [FromBody] UpdateAppointmentStatusDto dto)
        {
            return await HandleOkAsync(() => _appointmentService.UpdateAppointmentStatusAsync(id, dto, User));
        }

        [HttpPut("{id}/cancel")]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> CancelAppointment(int id, [FromBody] CancelAppointmentDto dto)
        {
            return await HandleOkAsync(() => _appointmentService.CancelAppointmentAsync(id, dto, User));
        }

        private async Task<IActionResult> HandleOkAsync<T>(Func<Task<T>> action)
        {
            try
            {
                return Ok(await action());
            }
            catch (ForbiddenAccessException ex)
            {
                return StatusCode(403, ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(string.IsNullOrWhiteSpace(ex.Message) ? null : ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch
            {
                return StatusCode(500, "Đã xảy ra lỗi không xác định.");
            }
        }

        private async Task<IActionResult> HandleActionAsync(Func<Task<IActionResult>> action)
        {
            try
            {
                return await action();
            }
            catch (ForbiddenAccessException ex)
            {
                return StatusCode(403, ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(string.IsNullOrWhiteSpace(ex.Message) ? null : ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch
            {
                return StatusCode(500, "Đã xảy ra lỗi không xác định.");
            }
        }
    }
}
