using AcademicAppointment.Domain.Entities;

namespace AcademicAppointment.Application.Interfaces
{
    public interface ILecturerRepository
    {
        Task<bool> ExistsByLecturerCodeAsync(string lecturerCode, CancellationToken cancellationToken = default);
        Task<Lecturer?> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default);
        void Add(Lecturer lecturer);
    }
}

