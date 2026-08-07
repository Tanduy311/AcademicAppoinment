using AcademicAppoinment.DTOs;
using AcademicAppoinment.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AcademicAppoinment.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthController(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }



        [HttpPost("StudentRegister")]
        public async Task<IActionResult> StudentRegister([FromBody] RegisterRequest register)
        {
            return await ProcessRegister(register, 2); // RoleId 2 for Student
        }

        [HttpPost("LecturerRegister")]
        public async Task<IActionResult> LecturerRegister([FromBody] RegisterRequest register)
        {
            return await ProcessRegister(register, 3); // RoleId 3 for Lecturer
        }

        [HttpPost("ProcessRegister")]
        public async Task<IActionResult> ProcessRegister(RegisterRequest register, int role)

        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (_context.Users.Any(u => u.AccountName == register.AccountName))
            {
                return BadRequest("Account name already exists.");
            }

            if (_context.Users.Any(u => u.EmailAddress == register.Email))
            {
                return BadRequest("Email address already used.");
            }

            var hashPassword = BCrypt.Net.BCrypt.HashPassword(register.Password);

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var user = new User
                {
                    AccountName = register.AccountName,
                    PasswordHash = hashPassword,
                    EmailAddress = register.Email,
                    FullName = register.FullName,
                    RoleId = role
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                if (role == 2)
                {
                    var student = new Student
                    {
                        UserId = user.UserId
                    };
                    _context.Students.Add(student);
                }
               
                else if (role == 3)
                {
                    var lecturer = new Lecturer
                    {
                        UserId = user.UserId
                    };
                    _context.Lecturers.Add(lecturer);
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return Ok(new { message = "registed successfully" });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        private string GenerateJwtToken(User user)
        {
            var roleName = _context.Users
                .Include(u => u.Role)
                .FirstOrDefault(u => u.UserId == user.UserId)?.Role?.RoleName;

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Name, user.AccountName),
                new Claim(ClaimTypes.Email, user.EmailAddress),
                new Claim(ClaimTypes.Role, roleName)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSettings:SecretKey"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["JwtSettings:Issuer"],
                audience: _configuration["JwtSettings:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(60),
                signingCredentials: creds
            );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

         
    }
}
