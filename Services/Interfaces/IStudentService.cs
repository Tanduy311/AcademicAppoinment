using AcademicAppoinment.DTOs.Students;
using System.Security.Claims;

namespace AcademicAppoinment.Services.Interfaces
{
    public interface IStudentService
    {
        Task<IReadOnlyList<StudentListItemDto>> GetStudentsAsync();
        Task<StudentDetailDto> GetStudentByIdAsync(int studentId, ClaimsPrincipal user);
        Task<StudentDetailDto> GetMyProfileAsync(ClaimsPrincipal user);
    }
}
