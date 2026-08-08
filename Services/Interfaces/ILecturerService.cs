using AcademicAppoinment.DTOs.Lecturers;

namespace AcademicAppoinment.Services.Interfaces
{
    public interface ILecturerService
    {
        Task<IReadOnlyList<LecturerListItemDto>> GetLecturersAsync();
        Task<LecturerDetailDto> GetLecturerByIdAsync(int lecturerId);
    }
}
