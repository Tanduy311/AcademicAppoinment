using AcademicAppointment.Application.DTOs.Slot;
using AcademicAppointment.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AcademicAppointment.API.Controllers
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

        /// <summary>
        /// Giảng viên tạo khung giờ rảnh mới.
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Lecturer")]
        public async Task<IActionResult> CreateSlot([FromBody] CreateSlotDto dto)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdClaim, out int userId)) return Unauthorized();

            var lecturerIdClaim = User.FindFirst("LecturerId")?.Value;
            var userNameClaim = User.FindFirst(ClaimTypes.Name)?.Value;

            try
            {
                var result = await _slotService.CreateSlotAsync(userId, lecturerIdClaim, userNameClaim, dto);
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Giảng viên xem danh sách slot rảnh của chính mình.
        /// </summary>
        [HttpGet("my-slots")]
        [Authorize(Roles = "Lecturer")]
        public async Task<IActionResult> GetMySlots()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdClaim, out int userId)) return Unauthorized();

            try
            {
                var slots = await _slotService.GetMySlotsAsync(userId);
                return Ok(slots);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        /// <summary>
        /// Lấy tất cả slot còn trống của một giảng viên.
        /// </summary>
        [HttpGet("lecturer/{lecturerId}")]
        public async Task<IActionResult> GetSlotsByLecturer(int lecturerId)
        {
            var slots = await _slotService.GetSlotsByLecturerAsync(lecturerId);
            return Ok(slots);
        }

        /// <summary>
        /// Giảng viên xóa một slot rảnh chưa có sinh viên đặt.
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Lecturer")]
        public async Task<IActionResult> DeleteSlot(int id)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdClaim, out int userId)) return Unauthorized();

            try
            {
                await _slotService.DeleteSlotAsync(userId, id);
                return Ok(new { message = "Xóa khung giờ rảnh thành công." });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}

