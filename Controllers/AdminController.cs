using System;
using System.Linq;
using AcademicAppoinment.Models;
using Microsoft.AspNetCore.Mvc;

namespace AcademicAppoinment.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly AppDbContext _context;
        public AdminController(AppDbContext context)
        {
            _context = context;
        }

        // Temporary endpoint to seed minimal test data for local development.
        // WARNING: This is intended for local/dev only and should be removed after testing.
        [HttpPost("seed-test-data")]
        public IActionResult SeedTestData()
        {
            // Ensure roles exist
            var lecturerRole = _context.Roles.FirstOrDefault(r => r.RoleName == "Lecturer");
            var studentRole = _context.Roles.FirstOrDefault(r => r.RoleName == "Student");

            // minimal guard
            if (lecturerRole == null || studentRole == null)
            {
                return StatusCode(500, "Required roles not present in DB.");
            }

            // Create lecturer user if missing
            var lecEmail = "lecturer.test@local";
            var lecUser = _context.Users.FirstOrDefault(u => u.EmailAddress == lecEmail);
            if (lecUser == null)
            {
                lecUser = new User
                {
                    AccountName = "lecturer.test",
                    PasswordHash = "testhash",
                    EmailAddress = lecEmail,
                    FullName = "Lecturer Test",
                    RoleId = lecturerRole.RoleId,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                };
                _context.Users.Add(lecUser);
                _context.SaveChanges();
            }

            // Create lecturer entry if missing
            var lecCode = "L-TEST-2";
            var lecturer = _context.Lecturers.FirstOrDefault(l => l.UserId == lecUser.UserId);
            if (lecturer == null)
            {
                lecturer = new Lecturer
                {
                    UserId = lecUser.UserId,
                    LecturerCode = lecCode,
                    Department = "Testing",
                    Specialization = "Integration",
                    OfficeLocation = "Room Test",
                    ConsultationDescription = "Seeded test lecturer"
                };
                _context.Lecturers.Add(lecturer);
                _context.SaveChanges();
            }

            // Create availability slot
            var slotExists = _context.AvailabilitySlots.Any(s => s.LecturerId == lecturer.LecturerId && s.StartTime > DateTime.UtcNow);
            if (!slotExists)
            {
                var slot = new AvailabilitySlot
                {
                    LecturerId = lecturer.LecturerId,
                    StartTime = DateTime.UtcNow.AddDays(1).AddHours(9),
                    EndTime = DateTime.UtcNow.AddDays(1).AddHours(10),
                    MeetingType = "InPerson",
                    LocationOrLink = "Room Test",
                    IsAvailable = true,
                    CreatedAt = DateTime.UtcNow
                };
                _context.AvailabilitySlots.Add(slot);
                _context.SaveChanges();
            }

            // Create a test student
            var stuEmail = "student.test@local";
            var stuUser = _context.Users.FirstOrDefault(u => u.EmailAddress == stuEmail);
            if (stuUser == null)
            {
                stuUser = new User
                {
                    AccountName = "student.test",
                    PasswordHash = "testhash",
                    EmailAddress = stuEmail,
                    FullName = "Student Test",
                    RoleId = studentRole.RoleId,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                };
                _context.Users.Add(stuUser);
                _context.SaveChanges();
            }

            var student = _context.Students.FirstOrDefault(s => s.UserId == stuUser.UserId);
            if (student == null)
            {
                student = new Student
                {
                    UserId = stuUser.UserId,
                    StudentCode = "S-TEST-1",
                    Major = "Testing",
                    ClassName = "TST101",
                    AcademicYear = "2026"
                };
                _context.Students.Add(student);
                _context.SaveChanges();
            }

            return Ok(new
            {
                message = "Seeded test lecturer, slot and student",
                lecturerId = lecturer.LecturerId,
                studentId = student.StudentId
            });
        }
    }
}
