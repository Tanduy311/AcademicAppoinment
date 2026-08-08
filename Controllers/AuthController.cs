using AcademicAppoinment.DTOs.Auth;
using AcademicAppoinment.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AcademicAppoinment.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register-student")]
        public async Task<IActionResult> RegisterStudent([FromBody] RegisterStudentDto dto)
        {
            return await HandleAsync(() => _authService.RegisterStudentAsync(dto));
        }

        [HttpPost("register-lecturer")]
        public async Task<IActionResult> RegisterLecturer([FromBody] RegisterLecturerDto dto)
        {
            return await HandleAsync(() => _authService.RegisterLecturerAsync(dto));
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            return await HandleAsync(() => _authService.LoginAsync(dto));
        }

        [HttpGet("me")]
        [Authorize]
        public async Task<IActionResult> GetCurrentUser()
        {
            return await HandleAsync(() => _authService.GetCurrentUserAsync(User));
        }

        private async Task<IActionResult> HandleAsync<T>(Func<Task<T>> action)
        {
            try
            {
                return Ok(await action());
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(string.IsNullOrWhiteSpace(ex.Message) ? null : ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch
            {
                return StatusCode(500, "Đã xảy ra lỗi không xác định.");
            }
        }
    }
}
