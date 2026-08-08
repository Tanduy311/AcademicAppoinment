using AcademicAppoinment.Models;
using Microsoft.EntityFrameworkCore;

namespace AcademicAppoinment.Services.Lecturers
{
    public class LecturerService : ILecturerService
    {
        private readonly AppDbContext _context;

        public LecturerService(AppDbContext context)
        {
            _context = context;
        }

        public IEnumerable<Lecturer> SearchLecturers(string? name, string? department, string? specialization, int page = 1, int pageSize = 20)
        {
            var query = _context.Lecturers.Include(l => l.User).AsQueryable();

            if (!string.IsNullOrWhiteSpace(name))
            {
                query = query.Where(l => l.User.FullName.Contains(name));
            }
            if (!string.IsNullOrWhiteSpace(department))
            {
                query = query.Where(l => l.Department != null && l.Department.Contains(department));
            }
            if (!string.IsNullOrWhiteSpace(specialization))
            {
                query = query.Where(l => l.Specialization != null && l.Specialization.Contains(specialization));
            }

            return query.Skip((page - 1) * pageSize).Take(pageSize).ToList();
        }

        public Lecturer? GetLecturerById(int id)
        {
            return _context.Lecturers.Include(l => l.User).FirstOrDefault(l => l.LecturerId == id);
        }

        public IEnumerable<AvailabilitySlot> GetFutureAvailableSlots(int lecturerId)
        {
            var now = DateTime.Now;
            return _context.AvailabilitySlots
                .Where(s => s.LecturerId == lecturerId && s.IsAvailable && s.StartTime > now)
                .OrderBy(s => s.StartTime)
                .ToList();
        }
    }
}