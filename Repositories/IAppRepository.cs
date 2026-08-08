using AcademicAppoinment.Models;

namespace AcademicAppoinment.Repositories
{
    public interface IAppRepository
    {
        Task<bool> UserAccountExistsAsync(string accountName);
        Task<bool> UserEmailExistsAsync(string emailAddress);
        Task<bool> StudentCodeExistsAsync(string studentCode);
        Task<bool> LecturerCodeExistsAsync(string lecturerCode);

        Task<User?> GetUserByAccountNameAsync(string accountName);
        Task<User?> GetUserWithDetailsByIdAsync(int userId);
        Task<User?> GetUserByIdWithDetailsAsync(int userId);
        Task<List<User>> GetUsersAsync();
        Task<List<Role>> GetRolesAsync();
        Task<Role?> GetRoleByIdAsync(int roleId);

        Task<Student?> GetStudentWithUserByIdAsync(int studentId);
        Task<Student?> GetStudentWithUserByUserIdAsync(int userId);
        Task<List<Student>> GetStudentsAsync();
        Task<Student?> GetStudentByIdWithDetailsAsync(int studentId);

        Task<Lecturer?> GetLecturerWithUserByIdAsync(int lecturerId);
        Task<Lecturer?> GetLecturerWithUserByUserIdAsync(int userId);
        Task<List<Lecturer>> GetLecturersAsync();
        Task<Lecturer?> GetLecturerByIdWithDetailsAsync(int lecturerId);

        Task<AvailabilitySlot?> GetAvailabilitySlotWithLecturerAsync(int slotId);
        Task<bool> HasSlotOverlapAsync(int lecturerId, DateTime startTime, DateTime endTime);
        Task<List<AvailabilitySlot>> GetSlotsByLecturerIdAsync(int lecturerId);

        Task<Appointment?> GetAppointmentDetailByIdAsync(int appointmentId);
        Task<List<Appointment>> GetAppointmentsByStudentIdAsync(int studentId);
        Task<List<Appointment>> GetAppointmentsByLecturerIdAsync(int lecturerId);

        Task<List<Notification>> GetNotificationsByUserIdAsync(int userId);
        Task<Notification?> GetNotificationByIdAsync(int notificationId);
        Task<int> GetUnreadNotificationCountByUserIdAsync(int userId);


        void AddUser(User user);
        void AddStudent(Student student);
        void AddLecturer(Lecturer lecturer);
        void AddAppointment(Appointment appointment);
        void AddNotification(Notification notification);
        void AddSlot(AvailabilitySlot slot);
        void RemoveSlot(AvailabilitySlot slot);

        Task<int> SaveChangesAsync();
    }
}
