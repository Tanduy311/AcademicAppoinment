using AcademicAppointment.Domain.Entities;

namespace AcademicAppointment.Application.Interfaces
{
    public interface IUserRepository
    {
        Task<bool> ExistsByAccountNameAsync(string accountName, CancellationToken cancellationToken = default);
        Task<bool> ExistsByEmailAddressAsync(string emailAddress, CancellationToken cancellationToken = default);
        Task<User?> GetByAccountNameWithProfileAsync(string accountName, CancellationToken cancellationToken = default);
        Task<User?> GetByIdWithProfileAsync(int userId, CancellationToken cancellationToken = default);
        void Add(User user);
    }
}

