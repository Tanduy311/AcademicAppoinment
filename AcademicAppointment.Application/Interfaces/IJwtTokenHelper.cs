using AcademicAppointment.Domain.Entities;

namespace AcademicAppointment.Application.Interfaces
{
    public interface IJwtTokenHelper
    {
        string GenerateToken(User user, string roleName, int? studentId = null, int? lecturerId = null);
    }
}

