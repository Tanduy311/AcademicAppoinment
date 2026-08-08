using AcademicAppointment.Application.Interfaces;
using AcademicAppointment.Domain.Entities;
using AcademicAppointment.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AcademicAppointment.Infrastructure.Repositories
{
    public class LecturerRepository : ILecturerRepository
    {
        private readonly AppDbContext _context;

        public LecturerRepository(AppDbContext context)
        {
            _context = context;
        }

        public Task<bool> ExistsByLecturerCodeAsync(string lecturerCode, CancellationToken cancellationToken = default)
        {
            return _context.Lecturers.AnyAsync(l => l.LecturerCode == lecturerCode, cancellationToken);
        }

        public Task<Lecturer?> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default)
        {
            return _context.Lecturers.FirstOrDefaultAsync(l => l.UserId == userId, cancellationToken);
        }

        public void Add(Lecturer lecturer)
        {
            _context.Lecturers.Add(lecturer);
        }
    }
}

