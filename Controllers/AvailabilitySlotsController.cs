using AcademicAppoinment.DTOs.Slot;
using AcademicAppoinment.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace AcademicAppoinment.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AvailabilitySlotsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AvailabilitySlotsController(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// 1. Giảng viên tạo khung giờ rảnh mới (Chỉ dành cho Lecturer)
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Lecturer")]
        public async Task<IActionResult> CreateSlot([FromBody] CreateSlotDto dto)
        {
            // Lấy LecturerId từ Claim trong Token
            var lecturerIdClaim = User.FindFirst("LecturerId")?.Value;
            if (string.IsNullOrEmpty(lecturerIdClaim) || !int.TryParse(lecturerIdClaim, out int lecturerId))
            {
                // Dự phòng nếu Claim LecturerId chưa có trong token, tìm qua UserId
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (!int.TryParse(userIdClaim, out int userId)) return Unauthorized();

                var lecturer = await _context.Lecturers.FirstOrDefaultAsync(l => l.UserId == userId);
                if (lecturer == null) return BadRequest("Không tìm thấy thông tin Giảng viên.");
                lecturerId = lecturer.LecturerId;
            }

            // Kiểm tra trùng lặp khung giờ (Tránh Giảng viên tạo 2 slot đè lên nhau)
            bool isOverlap = await _context.AvailabilitySlots.AnyAsync(s =>
                s.LecturerId == lecturerId &&
                ((dto.StartTime >= s.StartTime && dto.StartTime < s.EndTime) ||
                 (dto.EndTime > s.StartTime && dto.EndTime <= s.EndTime) ||
                 (dto.StartTime <= s.StartTime && dto.EndTime >= s.EndTime)));

            if (isOverlap)
            {
                return BadRequest("Khung giờ này bị trùng trùng với một lịch rảnh khác đã tạo.");
            }

            var slot = new AvailabilitySlot
            {
                LecturerId = lecturerId,
                StartTime = dto.StartTime,
                EndTime = dto.EndTime,
                MeetingType = dto.MeetingType,
                LocationOrLink = dto.LocationOrLink,
                IsAvailable = true,
                CreatedAt = DateTime.Now
            };

            _context.AvailabilitySlots.Add(slot);
            await _context.SaveChangesAsync();

            return Ok(new SlotResponseDto
            {
                AvailabilitySlotId = slot.AvailabilitySlotId,
                LecturerId = slot.LecturerId,
                LecturerName = User.FindFirst(ClaimTypes.Name)?.Value ?? "",
                StartTime = slot.StartTime,
                EndTime = slot.EndTime,
                MeetingType = slot.MeetingType,
                LocationOrLink = slot.LocationOrLink,
                IsAvailable = slot.IsAvailable,
                CreatedAt = slot.CreatedAt
            });
        }

        /// <summary>
        /// 2. Giảng viên xem danh sách các slot rảnh của chính mình
        /// </summary>
        [HttpGet("my-slots")]
        [Authorize(Roles = "Lecturer")]
        public async Task<IActionResult> GetMySlots()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdClaim, out int userId)) return Unauthorized();

            var lecturer = await _context.Lecturers.FirstOrDefaultAsync(l => l.UserId == userId);
            if (lecturer == null) return NotFound("Không tìm thấy thông tin Giảng viên.");

            var slots = await _context.AvailabilitySlots
                .Include(s => s.Lecturer)
                .ThenInclude(l => l!.User)
                .Where(s => s.LecturerId == lecturer.LecturerId)
                .OrderByDescending(s => s.StartTime)
                .Select(s => new SlotResponseDto
                {
                    AvailabilitySlotId = s.AvailabilitySlotId,
                    LecturerId = s.LecturerId,
                    LecturerName = s.Lecturer!.User!.FullName,
                    Department = s.Lecturer.Department,
                    StartTime = s.StartTime,
                    EndTime = s.EndTime,
                    MeetingType = s.MeetingType,
                    LocationOrLink = s.LocationOrLink,
                    IsAvailable = s.IsAvailable,
                    CreatedAt = s.CreatedAt
                })
                .ToListAsync();

            return Ok(slots);
        }

        /// <summary>
        /// 3. Lấy tất cả lịch rảnh TRỐNG (IsAvailable = true) của 1 Giảng viên cụ thể (Công khai cho Sinh viên chọn)
        /// </summary>
        [HttpGet("lecturer/{lecturerId}")]
        public async Task<IActionResult> GetSlotsByLecturer(int lecturerId)
        {
            var slots = await _context.AvailabilitySlots
                .Include(s => s.Lecturer)
                .ThenInclude(l => l!.User)
                .Where(s => s.LecturerId == lecturerId && s.IsAvailable && s.StartTime > DateTime.Now)
                .OrderBy(s => s.StartTime)
                .Select(s => new SlotResponseDto
                {
                    AvailabilitySlotId = s.AvailabilitySlotId,
                    LecturerId = s.LecturerId,
                    LecturerName = s.Lecturer!.User!.FullName,
                    Department = s.Lecturer.Department,
                    StartTime = s.StartTime,
                    EndTime = s.EndTime,
                    MeetingType = s.MeetingType,
                    LocationOrLink = s.LocationOrLink,
                    IsAvailable = s.IsAvailable,
                    CreatedAt = s.CreatedAt
                })
                .ToListAsync();

            return Ok(slots);
        }

        /// <summary>
        /// 4. Giảng viên xóa 1 slot rảnh (Chỉ xóa được slot chưa ai đặt)
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Lecturer")]
        public async Task<IActionResult> DeleteSlot(int id)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdClaim, out int userId)) return Unauthorized();

            var lecturer = await _context.Lecturers.FirstOrDefaultAsync(l => l.UserId == userId);
            if (lecturer == null) return Unauthorized();

            var slot = await _context.AvailabilitySlots.FindAsync(id);
            if (slot == null) return NotFound("Không tìm thấy khung giờ này.");

            if (slot.LecturerId != lecturer.LecturerId)
            {
                return Forbid("Bạn không có quyền xóa khung giờ của giảng viên khác.");
            }

            if (!slot.IsAvailable)
            {
                return BadRequest("Không thể xóa khung giờ này vì đã có Sinh viên đặt lịch.");
            }

            _context.AvailabilitySlots.Remove(slot);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Xóa khung giờ rảnh thành công." });
        }
    }
}
