using System;
using System.IO;
using System.Linq;
using AcademicAppoinment.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

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
        [Consumes("multipart/form-data")]
        public IActionResult UploadProgress([FromForm] AcademicAppoinment.DTOs.UploadProgressRequest req)
        {
            var file = req?.File;
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdClaim, out var userId)) return BadRequest("Missing or invalid user claim");

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
                ContentType = file.ContentType,
                CreatedAt = DateTime.UtcNow
            };

            _context.StudentProgresses.Add(record);
            _context.SaveChanges();

            return Ok(new { message = "File uploaded", id = record.StudentProgressId });
        }
    }
}