using AcademicAppointment.Application.Interfaces;
using AcademicAppointment.Domain.Entities;
using AcademicAppointment.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AcademicAppointment.Infrastructure.Repositories
{
    public class StudentRepository : IStudentRepository
    {
        private readonly AppDbContext _context;

        public StudentRepository(AppDbContext context)
        {
            _context = context;
        }

        public Task<bool> ExistsByStudentCodeAsync(string studentCode, CancellationToken cancellationToken = default)
        {
            return _context.Students.AnyAsync(s => s.StudentCode == studentCode, cancellationToken);
        }

        public void Add(Student student)
        {
            _context.Students.Add(student);
        }
    }
}

