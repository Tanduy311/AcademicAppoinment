using AcademicAppoinment.Services.Lecturers;
using Microsoft.AspNetCore.Mvc;

namespace AcademicAppoinment.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LecturersController : ControllerBase
    {
        private readonly ILecturerService _lecturerService;

        public LecturersController(ILecturerService lecturerService)
        {
            _lecturerService = lecturerService;
        }

        [HttpGet]
        public IActionResult Search([FromQuery] string? name, [FromQuery] string? department, [FromQuery] string? specialization, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            var lecturers = _lecturerService.SearchLecturers(name, department, specialization, page, pageSize);
            var result = lecturers.Select(l => new {
                l.LecturerId,
                l.LecturerCode,
                FullName = l.User?.FullName,
                l.Department,
                l.Specialization,
                l.OfficeLocation
            });
            return Ok(result);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var lecturer = _lecturerService.GetLecturerById(id);
            if (lecturer == null) return NotFound();
            return Ok(new {
                lecturer.LecturerId,
                lecturer.LecturerCode,
                FullName = lecturer.User?.FullName,
                Email = lecturer.User?.EmailAddress,
                lecturer.Department,
                lecturer.Specialization,
                lecturer.ConsultationDescription,
                lecturer.OfficeLocation
            });
        }

        [HttpGet("{id}/slots")]
        public IActionResult GetSlots(int id)
        {
            var slots = _lecturerService.GetFutureAvailableSlots(id);
            var result = slots.Select(s => new {
                s.AvailabilitySlotId,
                s.StartTime,
                s.EndTime,
                s.MeetingType,
                s.LocationOrLink
            });
            return Ok(result);
        }
    }
}