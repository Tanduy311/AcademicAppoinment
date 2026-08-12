using AcademicAppoinment.DTOs.Slot;
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
            var result = await _slotService.CreateSlotAsync(dto, User);
            return Ok(result);
        }

        [HttpGet("my-slots")]
        [Authorize(Roles = "Lecturer")]
        public async Task<IActionResult> GetMySlots()
        {
            var result = await _slotService.GetMySlotsAsync(User);
            return Ok(result);
        }

        [HttpGet("lecturer/{lecturerId}")]
        public async Task<IActionResult> GetSlotsByLecturer(int lecturerId)
        {
            var result = await _slotService.GetSlotsByLecturerAsync(lecturerId);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Lecturer")]
        public async Task<IActionResult> DeleteSlot(int id)
        {
            var message = await _slotService.DeleteSlotAsync(id, User);
            return Ok(new { message });
        }
    }
}

