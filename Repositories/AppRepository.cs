using AcademicAppoinment.Models;
using Microsoft.EntityFrameworkCore;

namespace AcademicAppoinment.Repositories
{
    public class AppRepository : IAppRepository
    {
        private readonly AppDbContext _context;

        public AppRepository(AppDbContext context)
        {
            _context = context;
        }

        public Task<bool> UserAccountExistsAsync(string accountName) =>
            _context.Users.AnyAsync(u => u.AccountName == accountName);

        public Task<bool> UserEmailExistsAsync(string emailAddress) =>
            _context.Users.AnyAsync(u => u.EmailAddress == emailAddress);

        public Task<bool> StudentCodeExistsAsync(string studentCode) =>
            _context.Students.AnyAsync(s => s.StudentCode == studentCode);

        public Task<bool> LecturerCodeExistsAsync(string lecturerCode) =>
            _context.Lecturers.AnyAsync(l => l.LecturerCode == lecturerCode);

        public Task<User?> GetUserByAccountNameAsync(string accountName) =>
            _context.Users
                .Include(u => u.Role)
                .Include(u => u.Student)
                .Include(u => u.Lecturer)
                .FirstOrDefaultAsync(u => u.AccountName == accountName);

        public Task<User?> GetUserWithDetailsByIdAsync(int userId) =>
            _context.Users
                .Include(u => u.Role)
                .Include(u => u.Student)
                .Include(u => u.Lecturer)
                .FirstOrDefaultAsync(u => u.UserId == userId);

        public Task<User?> GetUserByIdWithDetailsAsync(int userId) =>
            _context.Users
                .Include(u => u.Role)
                .Include(u => u.Student)
                .Include(u => u.Lecturer)
                .FirstOrDefaultAsync(u => u.UserId == userId);

        public Task<List<User>> GetUsersAsync() =>
            _context.Users
                .Include(u => u.Role)
                .Include(u => u.Student)
                .Include(u => u.Lecturer)
                .OrderByDescending(u => u.CreatedAt)
                .ToListAsync();

        public Task<List<Role>> GetRolesAsync() =>
            _context.Roles
                .OrderBy(r => r.RoleId)
                .ToListAsync();

        public Task<Role?> GetRoleByIdAsync(int roleId) =>
            _context.Roles.FirstOrDefaultAsync(r => r.RoleId == roleId);

        public Task<Student?> GetStudentWithUserByIdAsync(int studentId) =>
            _context.Students
                .Include(s => s.User)
                .FirstOrDefaultAsync(s => s.StudentId == studentId);

        public Task<Student?> GetStudentWithUserByUserIdAsync(int userId) =>
            _context.Students
                .Include(s => s.User)
                .FirstOrDefaultAsync(s => s.UserId == userId);

        public Task<List<Student>> GetStudentsAsync() =>
            _context.Students
                .Include(s => s.User)
                .Include(s => s.Appointments)
                .ToListAsync();

        public Task<Student?> GetStudentByIdWithDetailsAsync(int studentId) =>
            _context.Students
                .Include(s => s.User)
                .Include(s => s.Appointments)
                    .ThenInclude(a => a.Lecturer)
                        .ThenInclude(l => l!.User)
                .Include(s => s.Appointments)
                    .ThenInclude(a => a.AvailabilitySlot)
                .FirstOrDefaultAsync(s => s.StudentId == studentId);

        public Task<Lecturer?> GetLecturerWithUserByIdAsync(int lecturerId) =>
            _context.Lecturers
                .Include(l => l.User)
                .FirstOrDefaultAsync(l => l.LecturerId == lecturerId);

        public Task<Lecturer?> GetLecturerWithUserByUserIdAsync(int userId) =>
            _context.Lecturers
                .Include(l => l.User)
                .FirstOrDefaultAsync(l => l.UserId == userId);

        public Task<List<Lecturer>> GetLecturersAsync() =>
            _context.Lecturers
                .Include(l => l.User)
                .Include(l => l.AvailabilitySlots)
                .ToListAsync();

        public Task<Lecturer?> GetLecturerByIdWithDetailsAsync(int lecturerId) =>
            _context.Lecturers
                .Include(l => l.User)
                .Include(l => l.AvailabilitySlots)
                .FirstOrDefaultAsync(l => l.LecturerId == lecturerId);

        public Task<AvailabilitySlot?> GetAvailabilitySlotWithLecturerAsync(int slotId) =>
            _context.AvailabilitySlots
                .Include(s => s.Lecturer)
                .ThenInclude(l => l!.User)
                .FirstOrDefaultAsync(s => s.AvailabilitySlotId == slotId);

        public Task<bool> HasSlotOverlapAsync(int lecturerId, DateTime startTime, DateTime endTime) =>
            _context.AvailabilitySlots.AnyAsync(s =>
                s.LecturerId == lecturerId &&
                !s.IsDeleted &&
                ((startTime >= s.StartTime && startTime < s.EndTime) ||
                 (endTime > s.StartTime && endTime <= s.EndTime) ||
                 (startTime <= s.StartTime && endTime >= s.EndTime)));

        public Task<List<AvailabilitySlot>> GetSlotsByLecturerIdAsync(int lecturerId) =>
            _context.AvailabilitySlots
                .Include(s => s.Lecturer)
                .ThenInclude(l => l!.User)
                .Include(s => s.Appointments)
                .Where(s => s.LecturerId == lecturerId && !s.IsDeleted)
                .OrderByDescending(s => s.StartTime)
                .ToListAsync();

        public Task<Appointment?> GetAppointmentDetailByIdAsync(int appointmentId) =>
            _context.Appointments
                .Include(a => a.Student)
                    .ThenInclude(s => s!.User)
                .Include(a => a.Lecturer)
                    .ThenInclude(l => l!.User)
                .Include(a => a.AvailabilitySlot)
                .FirstOrDefaultAsync(a => a.AppointmentId == appointmentId);

        public Task<List<Appointment>> GetAppointmentsByStudentIdAsync(int studentId) =>
            _context.Appointments
                .Include(a => a.Student)
                    .ThenInclude(s => s!.User)
                .Include(a => a.Lecturer)
                    .ThenInclude(l => l!.User)
                .Include(a => a.AvailabilitySlot)
                .Where(a => a.StudentId == studentId)
                .OrderByDescending(a => a.CreatedAt)
                .ToListAsync();

        public Task<List<Appointment>> GetAppointmentsByLecturerIdAsync(int lecturerId) =>
            _context.Appointments
                .Include(a => a.Student)
                    .ThenInclude(s => s!.User)
                .Include(a => a.Lecturer)
                    .ThenInclude(l => l!.User)
                .Include(a => a.AvailabilitySlot)
                .Where(a => a.LecturerId == lecturerId)
                .OrderByDescending(a => a.CreatedAt)
                .ToListAsync();

        public Task<List<Notification>> GetNotificationsByUserIdAsync(int userId) =>
            _context.Notifications
                .Where(n => n.UserId == userId)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();

        public Task<Notification?> GetNotificationByIdAsync(int notificationId) =>
            _context.Notifications.FirstOrDefaultAsync(n => n.NotificationId == notificationId);

        public Task<int> GetUnreadNotificationCountByUserIdAsync(int userId) =>
            _context.Notifications.CountAsync(n => n.UserId == userId && !n.IsRead);


        public void AddUser(User user) => _context.Users.Add(user);

        public void AddStudent(Student student) => _context.Students.Add(student);

        public void AddLecturer(Lecturer lecturer) => _context.Lecturers.Add(lecturer);

        public void AddAppointment(Appointment appointment) => _context.Appointments.Add(appointment);

        public void AddNotification(Notification notification) => _context.Notifications.Add(notification);

        public void AddSlot(AvailabilitySlot slot) => _context.AvailabilitySlots.Add(slot);

        public void RemoveSlot(AvailabilitySlot slot) => _context.AvailabilitySlots.Remove(slot);

        public Task<int> SaveChangesAsync() => _context.SaveChangesAsync();
    }
}
