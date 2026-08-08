using AcademicAppoinment.Services.Interfaces;
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

        [HttpGet("{lecturerId}")]
        public async Task<IActionResult> GetLecturerById(int lecturerId)
        {
            var result = await _lecturerService.GetLecturerByIdAsync(lecturerId);
            return Ok(result);
        }
    }
}
