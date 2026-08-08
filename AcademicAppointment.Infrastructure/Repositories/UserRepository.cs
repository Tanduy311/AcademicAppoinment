using AcademicAppointment.Application.Interfaces;
using AcademicAppointment.Domain.Entities;
using AcademicAppointment.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AcademicAppointment.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;

        public UserRepository(AppDbContext context)
        {
            _context = context;
        }

        public Task<bool> ExistsByAccountNameAsync(string accountName, CancellationToken cancellationToken = default)
        {
            return _context.Users.AnyAsync(u => u.AccountName == accountName, cancellationToken);
        }

        public Task<bool> ExistsByEmailAddressAsync(string emailAddress, CancellationToken cancellationToken = default)
        {
            return _context.Users.AnyAsync(u => u.EmailAddress == emailAddress, cancellationToken);
        }

        public Task<User?> GetByAccountNameWithProfileAsync(string accountName, CancellationToken cancellationToken = default)
        {
            return _context.Users
                .Include(u => u.Role)
                .Include(u => u.Student)
                .Include(u => u.Lecturer)
                .FirstOrDefaultAsync(u => u.AccountName == accountName, cancellationToken);
        }

        public Task<User?> GetByIdWithProfileAsync(int userId, CancellationToken cancellationToken = default)
        {
            return _context.Users
                .Include(u => u.Role)
                .Include(u => u.Student)
                .Include(u => u.Lecturer)
                .FirstOrDefaultAsync(u => u.UserId == userId, cancellationToken);
        }

        public void Add(User user)
        {
            _context.Users.Add(user);
        }
    }
}

