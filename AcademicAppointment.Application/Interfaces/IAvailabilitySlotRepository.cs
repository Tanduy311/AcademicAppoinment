using AcademicAppointment.Domain.Entities;

namespace AcademicAppointment.Application.Interfaces
{
    public interface IAvailabilitySlotRepository
    {
        Task<bool> ExistsOverlappingAsync(
            int lecturerId,
            DateTime startTime,
            DateTime endTime,
            CancellationToken cancellationToken = default);

        Task<IReadOnlyList<AvailabilitySlot>> GetByLecturerIdAsync(
            int lecturerId,
            CancellationToken cancellationToken = default);

        Task<IReadOnlyList<AvailabilitySlot>> GetAvailableByLecturerIdAsync(
            int lecturerId,
            DateTime after,
            CancellationToken cancellationToken = default);

        Task<AvailabilitySlot?> GetByIdAsync(int slotId, CancellationToken cancellationToken = default);
        void Add(AvailabilitySlot slot);
        void Remove(AvailabilitySlot slot);
    }
}

