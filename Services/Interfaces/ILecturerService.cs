using AcademicAppoinment.DTOs.Lecturers;
using System.Security.Claims;

namespace AcademicAppoinment.Services.Interfaces
{
    public interface ILecturerService
    {
        Task<IReadOnlyList<LecturerListItemDto>> GetLecturersAsync();
        Task<LecturerDetailDto> GetLecturerByIdAsync(int lecturerId);
        Task<LecturerDetailDto> GetMyProfileAsync(ClaimsPrincipal user);
        Task<LecturerDetailDto> UpdateMyProfileAsync(UpdateLecturerProfileDto dto, ClaimsPrincipal user);
    }
}
