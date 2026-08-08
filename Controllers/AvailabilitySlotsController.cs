using AcademicAppoinment.DTOs.Slot;
using AcademicAppoinment.Helpers.Exceptions;
using AcademicAppoinment.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AcademicAppoinment.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AvailabilitySlotsController : ControllerBase
    {
        private readonly IAvailabilitySlotService _slotService;

        public AvailabilitySlotsController(IAvailabilitySlotService slotService)
        {
            _slotService = slotService;
        }

        [HttpPost]
        [Authorize(Roles = "Lecturer")]
        public async Task<IActionResult> CreateSlot([FromBody] CreateSlotDto dto)
        {
            return await HandleOkAsync(() => _slotService.CreateSlotAsync(dto, User));
        }

        [HttpGet("my-slots")]
        [Authorize(Roles = "Lecturer")]
        public async Task<IActionResult> GetMySlots()
        {
            return await HandleOkAsync(() => _slotService.GetMySlotsAsync(User));
        }

        [HttpGet("lecturer/{lecturerId}")]
        public async Task<IActionResult> GetSlotsByLecturer(int lecturerId)
        {
            return await HandleOkAsync(() => _slotService.GetSlotsByLecturerAsync(lecturerId));
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Lecturer")]
        public async Task<IActionResult> DeleteSlot(int id)
        {
            return await HandleActionAsync(async () => Ok(new { message = await _slotService.DeleteSlotAsync(id, User) }));
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
