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

        public Appointment CreateAppointment(int userId, DTOs.CreateAppointmentRequest request)
        {
            // find student
            var student = _context.Students.FirstOrDefault(s => s.UserId == userId)
                ?? throw new InvalidOperationException("Student not found for current user.");

            var slot = _context.AvailabilitySlots.Include(s => s.Lecturer)
                .FirstOrDefault(s => s.AvailabilitySlotId == request.AvailabilitySlotId)
                ?? throw new InvalidOperationException("Selected slot not found.");

            if (!slot.IsAvailable || slot.StartTime <= DateTime.Now)
            {
                throw new InvalidOperationException("Selected slot is not available.");
            }

            // conflict check: any overlapping appointment for this student with active statuses
            var overlapping = _context.Appointments
                .Include(a => a.AvailabilitySlot)
                .Where(a => a.StudentId == student.StudentId)
                .Where(a => a.Status != "Cancelled" && a.Status != "Rejected" && a.Status != "Completed")
                .Any(a => a.AvailabilitySlot.StartTime < slot.EndTime && a.AvailabilitySlot.EndTime > slot.StartTime);

            if (overlapping)
            {
                throw new InvalidOperationException("Student has conflicting appointment.");
            }

            var appointment = new Appointment
            {
                StudentId = student.StudentId,
                LecturerId = slot.LecturerId,
                AvailabilitySlotId = slot.AvailabilitySlotId,
                Topic = request.Topic,
                Description = request.Description,
                Status = "Pending",
                CreatedAt = DateTime.Now
            };

            slot.IsAvailable = false;

            _context.Appointments.Add(appointment);
            _context.SaveChanges();

            return appointment;
        }

        public IEnumerable<Appointment> GetAppointmentsForStudent(int userId, string? status = null, int page = 1, int pageSize = 20)
        {
            var student = _context.Students.FirstOrDefault(s => s.UserId == userId)
                ?? throw new InvalidOperationException("Student not found for current user.");

            var query = _context.Appointments
                .Include(a => a.AvailabilitySlot)
                .Include(a => a.Lecturer).ThenInclude(l => l.User)
                .Include(a => a.Student).ThenInclude(s => s.User)
                .Where(a => a.StudentId == student.StudentId)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(status))
            {
                query = query.Where(a => a.Status == status);
            }

            return query.OrderByDescending(a => a.CreatedAt)
                        .Skip((page - 1) * pageSize)
                        .Take(pageSize)
                        .ToList();
        }

        public bool StudentCancelAppointment(int appointmentId, int userId, string reason, out string error)
        {
            error = string.Empty;

            var student = _context.Students.FirstOrDefault(s => s.UserId == userId);
            if (student == null)
            {
                error = "Student not found.";
                return false;
            }

            var appointment = _context.Appointments.Include(a => a.AvailabilitySlot).FirstOrDefault(a => a.AppointmentId == appointmentId && a.StudentId == student.StudentId);
            if (appointment == null)
            {
                error = "Appointment not found.";
                return false;
            }

            if (appointment.Status == "Completed")
            {
                error = "Cannot cancel a completed appointment.";
                return false;
            }

            appointment.CancellationReason = reason;
            appointment.Status = "Cancelled";
            appointment.UpdatedAt = DateTime.Now;

            if (appointment.AvailabilitySlot != null)
            {
                appointment.AvailabilitySlot.IsAvailable = true;
            }

            _context.SaveChanges();
            return true;
        }

        // BR-10: Lecturer operations - must be the lecturer responsible for the slot
        public IEnumerable<Appointment> GetPendingAppointmentsForLecturer(int userId, int page = 1, int pageSize = 20)
        {
            var lecturer = _context.Lecturers.FirstOrDefault(l => l.UserId == userId);
            if (lecturer == null)
            {
                return Enumerable.Empty<Appointment>();
            }

            return _context.Appointments
                .Include(a => a.AvailabilitySlot)
                .Include(a => a.Lecturer).ThenInclude(l => l.User)
                .Include(a => a.Student).ThenInclude(s => s.User)
                .Where(a => a.LecturerId == lecturer.LecturerId && a.Status == "Pending")
                .OrderByDescending(a => a.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();
        }

        public bool ApproveAppointment(int appointmentId, int userId, out string error)
        {
            error = string.Empty;

            var lecturer = _context.Lecturers.FirstOrDefault(l => l.UserId == userId);
            if (lecturer == null)
            {
                error = "Lecturer not found.";
                return false;
            }

            var appointment = _context.Appointments.FirstOrDefault(a => a.AppointmentId == appointmentId);
            if (appointment == null)
            {
                error = "Appointment not found.";
                return false;
            }

            // BR-10: Only the responsible lecturer can approve
            if (appointment.LecturerId != lecturer.LecturerId)
            {
                error = "You can only approve appointments assigned to you.";
                return false;
            }

            if (appointment.Status != "Pending")
            {
                error = $"Cannot approve appointment with status '{appointment.Status}'.";
                return false;
            }

            appointment.Status = "Approved";
            appointment.UpdatedAt = DateTime.Now;
            _context.SaveChanges();
            return true;
        }

        public bool RejectAppointment(int appointmentId, int userId, string reason, out string error)
        {
            error = string.Empty;

            if (string.IsNullOrWhiteSpace(reason))
            {
                error = "Rejection reason is required.";
                return false;
            }

            var lecturer = _context.Lecturers.FirstOrDefault(l => l.UserId == userId);
            if (lecturer == null)
            {
                error = "Lecturer not found.";
                return false;
            }

            var appointment = _context.Appointments.Include(a => a.AvailabilitySlot).FirstOrDefault(a => a.AppointmentId == appointmentId);
            if (appointment == null)
            {
                error = "Appointment not found.";
                return false;
            }

            // BR-10: Only the responsible lecturer can reject
            if (appointment.LecturerId != lecturer.LecturerId)
            {
                error = "You can only reject appointments assigned to you.";
                return false;
            }

            if (appointment.Status != "Pending")
            {
                error = $"Cannot reject appointment with status '{appointment.Status}'.";
                return false;
            }

            appointment.Status = "Rejected";
            appointment.LecturerResponse = reason;
            appointment.UpdatedAt = DateTime.Now;

            if (appointment.AvailabilitySlot != null)
            {
                appointment.AvailabilitySlot.IsAvailable = true;
            }

            _context.SaveChanges();
            return true;
        }

        public bool LecturerCancelAppointment(int appointmentId, int userId, string reason, out string error)
        {
            error = string.Empty;

            if (string.IsNullOrWhiteSpace(reason))
            {
                error = "Cancellation reason is required.";
                return false;
            }

            var lecturer = _context.Lecturers.FirstOrDefault(l => l.UserId == userId);
            if (lecturer == null)
            {
                error = "Lecturer not found.";
                return false;
            }

            var appointment = _context.Appointments.Include(a => a.AvailabilitySlot).FirstOrDefault(a => a.AppointmentId == appointmentId);
            if (appointment == null)
            {
                error = "Appointment not found.";
                return false;
            }

            // BR-10: Only the responsible lecturer can cancel
            if (appointment.LecturerId != lecturer.LecturerId)
            {
                error = "You can only cancel appointments assigned to you.";
                return false;
            }

            if (appointment.Status == "Completed")
            {
                error = "Cannot cancel a completed appointment.";
                return false;
            }

            appointment.CancellationReason = reason;
            appointment.Status = "Cancelled";
            appointment.UpdatedAt = DateTime.Now;

            if (appointment.AvailabilitySlot != null)
            {
                appointment.AvailabilitySlot.IsAvailable = true;
            }

            _context.SaveChanges();
            return true;
        }

        public bool CompleteAppointment(int appointmentId, int userId, out string error)
        {
            error = string.Empty;

            var lecturer = _context.Lecturers.FirstOrDefault(l => l.UserId == userId);
            if (lecturer == null)
            {
                error = "Lecturer not found.";
                return false;
            }

            var appointment = _context.Appointments.FirstOrDefault(a => a.AppointmentId == appointmentId);
            if (appointment == null)
            {
                error = "Appointment not found.";
                return false;
            }

            // BR-10: Only the responsible lecturer can complete
            if (appointment.LecturerId != lecturer.LecturerId)
            {
                error = "You can only complete appointments assigned to you.";
                return false;
            }

            if (appointment.Status != "Approved")
            {
                error = $"Cannot complete appointment with status '{appointment.Status}'. Only approved appointments can be completed.";
                return false;
            }

            appointment.Status = "Completed";
            appointment.UpdatedAt = DateTime.Now;
            _context.SaveChanges();
            return true;
        }
    }
}
