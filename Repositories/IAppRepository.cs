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

        Task<Student?> GetStudentWithUserByIdAsync(int studentId);
        Task<Student?> GetStudentWithUserByUserIdAsync(int userId);

        Task<Lecturer?> GetLecturerWithUserByIdAsync(int lecturerId);
        Task<Lecturer?> GetLecturerWithUserByUserIdAsync(int userId);

        Task<AvailabilitySlot?> GetAvailabilitySlotWithLecturerAsync(int slotId);
        Task<bool> HasSlotOverlapAsync(int lecturerId, DateTime startTime, DateTime endTime);
        Task<List<AvailabilitySlot>> GetSlotsByLecturerIdAsync(int lecturerId);

        Task<Appointment?> GetAppointmentDetailByIdAsync(int appointmentId);
        Task<List<Appointment>> GetAppointmentsByStudentIdAsync(int studentId);
        Task<List<Appointment>> GetAppointmentsByLecturerIdAsync(int lecturerId);

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
