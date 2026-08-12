using AcademicAppoinment.DTOs.Admin;
using AcademicAppoinment.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AcademicAppoinment.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;

        public AdminController(IAdminService adminService)
        {
            _adminService = adminService;
        }

        [HttpGet("users")]
        public async Task<IActionResult> GetUsers()
        {
            var result = await _adminService.GetUsersAsync();
            return Ok(result);
        }

        [HttpGet("users/{userId}")]
        public async Task<IActionResult> GetUserById(int userId)
        {
            var result = await _adminService.GetUserByIdAsync(userId);
            return Ok(result);
        }

        [HttpGet("roles")]
        public async Task<IActionResult> GetRoles()
        {
            var result = await _adminService.GetRolesAsync();
            return Ok(result);
        }

        [HttpPut("users/{userId}/status")]
        public async Task<IActionResult> UpdateStatus(int userId, [FromBody] UpdateUserStatusDto dto)
        {
            await _adminService.SetUserActiveAsync(userId, dto.IsActive, User);
            return Ok(new { message = "Cập nhật trạng thái tài khoản thành công." });
        }

        [HttpPut("users/{userId}/role")]
        public async Task<IActionResult> UpdateRole(int userId, [FromBody] UpdateUserRoleDto dto)
        {
            await _adminService.SetUserRoleAsync(userId, dto.RoleId, User);
            return Ok(new { message = "Cập nhật role thành công." });
        }
    }
}
