using AcademicAppointment.Application.DTOs.Auth;

namespace AcademicAppointment.Application.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponseDto> RegisterStudentAsync(RegisterStudentDto dto);
        Task<AuthResponseDto> RegisterLecturerAsync(RegisterLecturerDto dto);
        Task<AuthResponseDto> LoginAsync(LoginDto dto);
        Task<CurrentUserDto?> GetCurrentUserAsync(int userId);
    }
}

