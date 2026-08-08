using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AcademicAppoinment.DTOs;
using AcademicAppoinment.Services.Lecturers;
using System.Security.Claims;

namespace AcademicAppoinment.Controllers
{
    [ApiController]
    [Route("api/availability-slots")]
    [Authorize(Policy = "LecturerOnly")]
    public class AvailabilitySlotController : ControllerBase
    {
        private readonly IAvailabilitySlotService _availabilitySlotService;

        public AvailabilitySlotController(IAvailabilitySlotService availabilitySlotService)
        {
            _availabilitySlotService = availabilitySlotService;
        }

        private int GetUserIdFromClaims()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            {
                throw new UnauthorizedAccessException("User ID not found in token.");
            }
            return userId;
        }

        /// <summary>
        /// Create a new availability slot for consultation
        /// </summary>
        [HttpPost]
        public IActionResult CreateAvailabilitySlot([FromBody] CreateAvailabilitySlotRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                int userId = GetUserIdFromClaims();
                var slot = _availabilitySlotService.CreateSlot(userId, request, out string error);

                if (slot == null)
                {
                    return BadRequest(new { message = error });
                }

                var response = new AvailabilitySlotResponse
                {
                    AvailabilitySlotId = slot.AvailabilitySlotId,
                    LecturerId = slot.LecturerId,
                    StartTime = slot.StartTime,
                    EndTime = slot.EndTime,
                    MeetingType = slot.MeetingType,
                    LocationOrLink = slot.LocationOrLink,
                    IsAvailable = slot.IsAvailable,
                    CreatedAt = slot.CreatedAt
                };

                return CreatedAtAction(nameof(GetMySlots), new { slotId = slot.AvailabilitySlotId }, response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An unexpected error occurred.", details = ex.Message });
            }
        }

        /// <summary>
        /// Get all availability slots managed by current lecturer
        /// </summary>
        [HttpGet("my-slots")]
        public IActionResult GetMySlots()
        {
            try
            {
                int userId = GetUserIdFromClaims();
                var slots = _availabilitySlotService.GetMySlots(userId, out string error);

                if (!string.IsNullOrEmpty(error))
                {
                    return BadRequest(new { message = error });
                }

                return Ok(new { data = slots, count = slots.Count() });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An unexpected error occurred.", details = ex.Message });
            }
        }

        /// <summary>
        /// Update an availability slot
        /// </summary>
        [HttpPut("{id}")]
        public IActionResult UpdateAvailabilitySlot(int id, [FromBody] UpdateAvailabilitySlotRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                int userId = GetUserIdFromClaims();
                var updatedSlot = _availabilitySlotService.UpdateSlot(userId, id, request, out string error);

                if (updatedSlot == null)
                {
                    if (error.Contains("only update your own"))
                    {
                        return Forbid();
                    }
                    return BadRequest(new { message = error });
                }

                var response = new AvailabilitySlotResponse
                {
                    AvailabilitySlotId = updatedSlot.AvailabilitySlotId,
                    LecturerId = updatedSlot.LecturerId,
                    StartTime = updatedSlot.StartTime,
                    EndTime = updatedSlot.EndTime,
                    MeetingType = updatedSlot.MeetingType,
                    LocationOrLink = updatedSlot.LocationOrLink,
                    IsAvailable = updatedSlot.IsAvailable,
                    CreatedAt = updatedSlot.CreatedAt
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An unexpected error occurred.", details = ex.Message });
            }
        }

        /// <summary>
        /// Delete an availability slot (only if no active appointments exist)
        /// </summary>
        [HttpDelete("{id}")]
        public IActionResult DeleteAvailabilitySlot(int id)
        {
            try
            {
                int userId = GetUserIdFromClaims();
                bool success = _availabilitySlotService.DeleteSlot(userId, id, out string error);

                if (!success)
                {
                    if (error.Contains("only delete your own"))
                    {
                        return Forbid();
                    }
                    return BadRequest(new { message = error });
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An unexpected error occurred.", details = ex.Message });
            }
        }
    }
}
