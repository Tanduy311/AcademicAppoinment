using AcademicAppoinment.DTOs.Auth;
using AcademicAppoinment.Helpers;
using AcademicAppoinment.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace AcademicAppoinment.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly JwtTokenHelper _jwtTokenHelper;

        public AuthController(AppDbContext context, JwtTokenHelper jwtTokenHelper)
        {
            _context = context;
            _jwtTokenHelper = jwtTokenHelper;
        }

        /// <summary>
        /// 1. Đăng ký tài khoản Sinh viên
        /// </summary>
        [HttpPost("register-student")]
        public async Task<IActionResult> RegisterStudent([FromBody] RegisterStudentDto dto)
        {
            if (await _context.Users.AnyAsync(u => u.AccountName == dto.AccountName))
                return BadRequest("Tên tài khoản đã tồn tại.");

            if (await _context.Users.AnyAsync(u => u.EmailAddress == dto.EmailAddress))
                return BadRequest("Email đã được sử dụng.");

            if (await _context.Students.AnyAsync(s => s.StudentCode == dto.StudentCode))
                return BadRequest("Mã sinh viên đã tồn tại.");

            // 1. Tạo User
            var user = new User
            {
                AccountName = dto.AccountName,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                FullName = dto.FullName,
                EmailAddress = dto.EmailAddress,
                PhoneNumber = dto.PhoneNumber,
                RoleId = 2, // RoleId = 2 là Student
                IsActive = true,
                CreatedAt = DateTime.Now
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // 2. Tạo Student gắn với User vừa tạo
            var student = new Student
            {
                UserId = user.UserId,
                StudentCode = dto.StudentCode,
                Major = dto.Major,
                ClassName = dto.ClassName,
                AcademicYear = dto.AcademicYear
            };

            _context.Students.Add(student);
            await _context.SaveChangesAsync();

            var token = _jwtTokenHelper.GenerateToken(user, "Student", studentId: student.StudentId);

            return Ok(new AuthResponseDto
            {
                Token = token,
                UserId = user.UserId,
                AccountName = user.AccountName,
                FullName = user.FullName,
                EmailAddress = user.EmailAddress,
                RoleName = "Student",
                StudentId = student.StudentId
            });
        }

        /// <summary>
        /// 2. Đăng ký tài khoản Giảng viên
        /// </summary>
        [HttpPost("register-lecturer")]
        public async Task<IActionResult> RegisterLecturer([FromBody] RegisterLecturerDto dto)
        {
            if (await _context.Users.AnyAsync(u => u.AccountName == dto.AccountName))
                return BadRequest("Tên tài khoản đã tồn tại.");

            if (await _context.Users.AnyAsync(u => u.EmailAddress == dto.EmailAddress))
                return BadRequest("Email đã được sử dụng.");

            if (await _context.Lecturers.AnyAsync(l => l.LecturerCode == dto.LecturerCode))
                return BadRequest("Mã giảng viên đã tồn tại.");

            // 1. Tạo User
            var user = new User
            {
                AccountName = dto.AccountName,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                FullName = dto.FullName,
                EmailAddress = dto.EmailAddress,
                PhoneNumber = dto.PhoneNumber,
                RoleId = 3, // RoleId = 3 là Lecturer
                IsActive = true,
                CreatedAt = DateTime.Now
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // 2. Tạo Lecturer gắn với User vừa tạo
            var lecturer = new Lecturer
            {
                UserId = user.UserId,
                LecturerCode = dto.LecturerCode,
                Department = dto.Department,
                Specialization = dto.Specialization,
                OfficeLocation = dto.OfficeLocation,
                ConsultationDescription = dto.ConsultationDescription
            };

            _context.Lecturers.Add(lecturer);
            await _context.SaveChangesAsync();

            var token = _jwtTokenHelper.GenerateToken(user, "Lecturer", lecturerId: lecturer.LecturerId);

            return Ok(new AuthResponseDto
            {
                Token = token,
                UserId = user.UserId,
                AccountName = user.AccountName,
                FullName = user.FullName,
                EmailAddress = user.EmailAddress,
                RoleName = "Lecturer",
                LecturerId = lecturer.LecturerId
            });
        }

        /// <summary>
        /// 3. Đăng nhập hệ thống (Dùng chung cho cả Admin, Student, Lecturer)
        /// </summary>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var user = await _context.Users
                .Include(u => u.Role)
                .Include(u => u.Student)
                .Include(u => u.Lecturer)
                .FirstOrDefaultAsync(u => u.AccountName == dto.AccountName);

            if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
            {
                return Unauthorized("Tên tài khoản hoặc mật khẩu không chính xác.");
            }

            if (!user.IsActive)
            {
                return Unauthorized("Tài khoản của bạn đã bị khóa.");
            }

            string roleName = user.Role?.RoleName ?? "User";
            int? studentId = user.Student?.StudentId;
            int? lecturerId = user.Lecturer?.LecturerId;

            var token = _jwtTokenHelper.GenerateToken(user, roleName, studentId, lecturerId);

            return Ok(new AuthResponseDto
            {
                Token = token,
                UserId = user.UserId,
                AccountName = user.AccountName,
                FullName = user.FullName,
                EmailAddress = user.EmailAddress,
                RoleName = roleName,
                StudentId = studentId,
                LecturerId = lecturerId
            });
        }

        /// <summary>
        /// 4. Lấy thông tin tài khoản hiện tại (Yêu cầu phải có Token)
        /// </summary>
        [HttpGet("me")]
        [Authorize]
        public async Task<IActionResult> GetCurrentUser()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized();
            }

            var user = await _context.Users
                .Include(u => u.Role)
                .Include(u => u.Student)
                .Include(u => u.Lecturer)
                .FirstOrDefaultAsync(u => u.UserId == userId);

            if (user == null) return NotFound("Không tìm thấy người dùng.");

            return Ok(new
            {
                user.UserId,
                user.AccountName,
                user.FullName,
                user.EmailAddress,
                user.PhoneNumber,
                RoleName = user.Role?.RoleName,
                StudentInfo = user.Student,
                LecturerInfo = user.Lecturer
            });
        }
    }
}
