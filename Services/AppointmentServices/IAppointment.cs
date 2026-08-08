using AcademicAppoinment.Models;

namespace AcademicAppoinment.Services.Appoiment
{
    public interface IAppointmentService
    {
        Appointment? GetAppointmentByIdForUser(int appointmentId, int userId, string role
        );
    }
}
