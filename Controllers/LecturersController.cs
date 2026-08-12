using AcademicAppoinment.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AcademicAppoinment.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LecturersController : ControllerBase
    {
        private readonly ILecturerService _lecturerService;

        public LecturersController(ILecturerService lecturerService)
        {
            _lecturerService = lecturerService;
        }

        [HttpGet]
        public async Task<IActionResult> GetLecturers()
        {
            var result = await _lecturerService.GetLecturersAsync();
            return Ok(result);
        }

        [HttpGet("me")]
        [Authorize(Roles = "Lecturer")]
        public async Task<IActionResult> GetMyProfile()
        {
            var result = await _lecturerService.GetMyProfileAsync(User);
            return Ok(result);
        }

        [HttpPut("me")]
        [Authorize(Roles = "Lecturer")]
        public async Task<IActionResult> UpdateMyProfile([FromBody] AcademicAppoinment.DTOs.Lecturers.UpdateLecturerProfileDto dto)
        {
            var result = await _lecturerService.UpdateMyProfileAsync(dto, User);
            return Ok(result);
        }

        [HttpGet("{lecturerId}")]
        public async Task<IActionResult> GetLecturerById(int lecturerId)
        {
            var result = await _lecturerService.GetLecturerByIdAsync(lecturerId);
            return Ok(result);
        }
    }
}
