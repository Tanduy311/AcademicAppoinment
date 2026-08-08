using AcademicAppointment.Application.Interfaces;
using AcademicAppointment.Domain.Entities;
using AcademicAppointment.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AcademicAppointment.Infrastructure.Repositories
{
    public class AvailabilitySlotRepository : IAvailabilitySlotRepository
    {
        private readonly AppDbContext _context;

        public AvailabilitySlotRepository(AppDbContext context)
        {
            _context = context;
        }

        public Task<bool> ExistsOverlappingAsync(
            int lecturerId,
            DateTime startTime,
            DateTime endTime,
            CancellationToken cancellationToken = default)
        {
            return _context.AvailabilitySlots.AnyAsync(s =>
                s.LecturerId == lecturerId &&
                ((startTime >= s.StartTime && startTime < s.EndTime) ||
                 (endTime > s.StartTime && endTime <= s.EndTime) ||
                 (startTime <= s.StartTime && endTime >= s.EndTime)),
                cancellationToken);
        }

        public async Task<IReadOnlyList<AvailabilitySlot>> GetByLecturerIdAsync(
            int lecturerId,
            CancellationToken cancellationToken = default)
        {
            return await _context.AvailabilitySlots
                .Include(s => s.Lecturer)
                .ThenInclude(l => l!.User)
                .Where(s => s.LecturerId == lecturerId)
                .OrderByDescending(s => s.StartTime)
                .ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<AvailabilitySlot>> GetAvailableByLecturerIdAsync(
            int lecturerId,
            DateTime after,
            CancellationToken cancellationToken = default)
        {
            return await _context.AvailabilitySlots
                .Include(s => s.Lecturer)
                .ThenInclude(l => l!.User)
                .Where(s => s.LecturerId == lecturerId && s.IsAvailable && s.StartTime > after)
                .OrderBy(s => s.StartTime)
                .ToListAsync(cancellationToken);
        }

        public Task<AvailabilitySlot?> GetByIdAsync(int slotId, CancellationToken cancellationToken = default)
        {
            return _context.AvailabilitySlots.FirstOrDefaultAsync(s => s.AvailabilitySlotId == slotId, cancellationToken);
        }

        public void Add(AvailabilitySlot slot)
        {
            _context.AvailabilitySlots.Add(slot);
        }

        public void Remove(AvailabilitySlot slot)
        {
            _context.AvailabilitySlots.Remove(slot);
        }
    }
}

