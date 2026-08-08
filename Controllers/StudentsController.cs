using AcademicAppoinment.Models;
using Microsoft.AspNetCore.Mvc;

namespace AcademicAppoinment.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public StudentsController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        [HttpPost("upload-progress")]
        public IActionResult UploadProgress([FromForm] IFormFile file)
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim)) return BadRequest("Missing user claim");
            var userId = int.Parse(userIdClaim);

            var student = _context.Students.FirstOrDefault(s => s.UserId == userId);
            if (student == null) return BadRequest("Student not found");

            if (file == null || file.Length == 0) return BadRequest("No file uploaded");

            var uploads = Path.Combine(_env.ContentRootPath, "Uploads", "StudentProgress");
            Directory.CreateDirectory(uploads);

            var fileName = Path.GetFileName(file.FileName);
            var savePath = Path.Combine(uploads, $"{Guid.NewGuid()}_{fileName}");

            using (var stream = System.IO.File.Create(savePath))
            {
                file.CopyTo(stream);
            }

            var record = new StudentProgress
            {
                StudentId = student.StudentId,
                FileName = fileName,
                FilePath = savePath,
                ContentType = file.ContentType
            };

            _context.StudentProgresses.Add(record);
            _context.SaveChanges();

            return Ok(new { message = "File uploaded", id = record.StudentProgressId });
        }
    }
}