using AcademicAppointment.Application.DTOs.Slot;

namespace AcademicAppointment.Application.Interfaces
{
    public interface IAvailabilitySlotService
    {
        Task<SlotResponseDto> CreateSlotAsync(int userId, string? lecturerClaimId, string? userNameClaim, CreateSlotDto dto);
        Task<List<SlotResponseDto>> GetMySlotsAsync(int userId);
        Task<List<SlotResponseDto>> GetSlotsByLecturerAsync(int lecturerId);
        Task DeleteSlotAsync(int userId, int slotId);
    }
}

