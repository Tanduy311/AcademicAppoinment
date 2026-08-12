using AcademicAppoinment.DTOs.Slot;
using System.Security.Claims;

namespace AcademicAppoinment.Services.Interfaces
{
    public interface IAvailabilitySlotService
    {
        Task<SlotResponseDto> CreateSlotAsync(CreateSlotDto dto, ClaimsPrincipal user);
        Task<IReadOnlyList<SlotResponseDto>> GetMySlotsAsync(ClaimsPrincipal user);
        Task<IReadOnlyList<SlotResponseDto>> GetSlotsByLecturerAsync(int lecturerId);
        Task<string> DeleteSlotAsync(int id, ClaimsPrincipal user);
    }
}
