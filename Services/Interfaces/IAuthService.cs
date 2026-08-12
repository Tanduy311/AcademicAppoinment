using AcademicAppoinment.DTOs.Auth;
using System.Security.Claims;

namespace AcademicAppoinment.Services.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponseDto> RegisterStudentAsync(RegisterStudentDto dto);
        Task<AuthResponseDto> RegisterLecturerAsync(RegisterLecturerDto dto);
        Task<AuthResponseDto> LoginAsync(LoginDto dto);
        Task<CurrentUserResponseDto> GetCurrentUserAsync(ClaimsPrincipal user);
        Task<CurrentUserResponseDto> UpdateMyAvatarAsync(IFormFile avatar, ClaimsPrincipal user);
    }
}
