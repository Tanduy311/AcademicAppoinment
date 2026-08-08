using AcademicAppoinment.Models;

namespace AcademicAppoinment.Services.Appoiment
{
using AcademicAppoinment.DTOs;
using System.Collections.Generic;

    public interface IAppointmentService
    {
        Appointment? GetAppointmentByIdForUser(
            int appointmentId,
            int userId,
            string role
        );

        Appointment CreateAppointment(int userId, CreateAppointmentRequest request);

        IEnumerable<Appointment> GetAppointmentsForStudent(int userId, string? status = null, int page = 1, int pageSize = 20);

        bool StudentCancelAppointment(int appointmentId, int userId, string reason, out string error);

        // Lecturer operations
        IEnumerable<Appointment> GetPendingAppointmentsForLecturer(int userId, int page = 1, int pageSize = 20);

        bool ApproveAppointment(int appointmentId, int userId, out string error);

        bool RejectAppointment(int appointmentId, int userId, string reason, out string error);

        bool LecturerCancelAppointment(int appointmentId, int userId, string reason, out string error);

        bool CompleteAppointment(int appointmentId, int userId, out string error);
    }
}
