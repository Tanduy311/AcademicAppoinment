using AcademicAppointment.Domain.Entities;

namespace AcademicAppointment.Application.Interfaces
{
    public interface IStudentRepository
    {
        Task<bool> ExistsByStudentCodeAsync(string studentCode, CancellationToken cancellationToken = default);
        void Add(Student student);
    }
}

