using AcademicAppoinment.DTOs;
using AcademicAppoinment.Models;

namespace AcademicAppoinment.Services.Lecturers
{
    public interface IAvailabilitySlotService
    {
        AvailabilitySlot CreateSlot(int userId, CreateAvailabilitySlotRequest request, out string error);
        IEnumerable<AvailabilitySlotResponse> GetMySlots(int userId, out string error);
        AvailabilitySlot? UpdateSlot(int userId, int slotId, UpdateAvailabilitySlotRequest request, out string error);
        bool DeleteSlot(int userId, int slotId, out string error);
    }
}
