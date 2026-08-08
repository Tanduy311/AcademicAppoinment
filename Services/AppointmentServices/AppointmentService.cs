using Microsoft.EntityFrameworkCore;
using AcademicAppoinment.Models;

namespace AcademicAppoinment.Services.Appoiment
{
    public class AppointmentService : IAppointmentService
    {
        private readonly AppDbContext _context;

        public AppointmentService(AppDbContext context)
        {
            _context = context;
        }

        public Appointment? GetAppointmentByIdForUser(
            int appointmentId,
            int userId,
            string role)
        {
            if (role == "Student")
            {
                var student = _context.Students
                    .FirstOrDefault(s => s.UserId == userId);

                if (student == null)
                {
                    return null;
                }

                return _context.Appointments
                    .Include(a => a.Student)
                        .ThenInclude(s => s.User)
                    .Include(a => a.Lecturer)
                        .ThenInclude(l => l.User)
                    .Include(a => a.AvailabilitySlot)
                    .FirstOrDefault(a =>
                        a.AppointmentId == appointmentId &&
                        a.StudentId == student.StudentId
                    );
            }

            if (role == "Lecturer")
            {
                var lecturer = _context.Lecturers
                    .FirstOrDefault(l => l.UserId == userId);

                if (lecturer == null)
                {
                    return null;
                }

                return _context.Appointments
                    .Include(a => a.Student)
                        .ThenInclude(s => s.User)
                    .Include(a => a.Lecturer)
                        .ThenInclude(l => l.User)
                    .Include(a => a.AvailabilitySlot)
                    .FirstOrDefault(a =>
                        a.AppointmentId == appointmentId &&
                        a.LecturerId == lecturer.LecturerId
                    );
            }

            return null;
        }
    }
}
