using Microsoft.EntityFrameworkCore;

namespace AcademicAppoinment.Models
{

    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<Lecturer> Lecturers { get; set; }
        public DbSet<AvailabilitySlot> AvailabilitySlots { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<Notification> Notifications { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // AccountName không được trùng
            modelBuilder.Entity<User>()
                .HasIndex(u => u.AccountName)
                .IsUnique();

            // EmailAddress không được trùng
            modelBuilder.Entity<User>()
                .HasIndex(u => u.EmailAddress)
                .IsUnique();

            // StudentCode không được trùng
            modelBuilder.Entity<Student>()
                .HasIndex(s => s.StudentCode)
                .IsUnique();

            // LecturerCode không được trùng
            modelBuilder.Entity<Lecturer>()
                .HasIndex(l => l.LecturerCode)
                .IsUnique();

            // Role 1 - N User
            modelBuilder.Entity<User>()
                .HasOne(u => u.Role)
                .WithMany(r => r.Users)
                .HasForeignKey(u => u.RoleId)
                .OnDelete(DeleteBehavior.Restrict);

            // User 1 - 1 Student
            modelBuilder.Entity<Student>()
                .HasOne(s => s.User)
                .WithOne(u => u.Student)
                .HasForeignKey<Student>(s => s.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // User 1 - 1 Lecturer
            modelBuilder.Entity<Lecturer>()
                .HasOne(l => l.User)
                .WithOne(u => u.Lecturer)
                .HasForeignKey<Lecturer>(l => l.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Lecturer 1 - N AvailabilitySlot
            modelBuilder.Entity<AvailabilitySlot>()
                .HasOne(a => a.Lecturer)
                .WithMany(l => l.AvailabilitySlots)
                .HasForeignKey(a => a.LecturerId)
                .OnDelete(DeleteBehavior.Restrict);

            // Student 1 - N Appointment
            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.Student)
                .WithMany(s => s.Appointments)
                .HasForeignKey(a => a.StudentId)
                .OnDelete(DeleteBehavior.Restrict);

            // Lecturer 1 - N Appointment
            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.Lecturer)
                .WithMany(l => l.Appointments)
                .HasForeignKey(a => a.LecturerId)
                .OnDelete(DeleteBehavior.Restrict);

            // AvailabilitySlot 1 - N Appointment
            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.AvailabilitySlot)
                .WithMany(s => s.Appointments)
                .HasForeignKey(a => a.AvailabilitySlotId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Appointment>()
                .HasIndex(a => a.AvailabilitySlotId)
                .IsUnique()
                .HasFilter("[Status] IN ('Pending', 'Confirmed')");

            // User 1 - N Notification
            modelBuilder.Entity<Notification>()
                .HasOne(n => n.User)
                .WithMany(u => u.Notifications)
                .HasForeignKey(n => n.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Appointment 1 - N Notification
            modelBuilder.Entity<Notification>()
                .HasOne(n => n.Appointment)
                .WithMany(a => a.Notifications)
                .HasForeignKey(n => n.AppointmentId)
                .OnDelete(DeleteBehavior.Restrict);

            // Dữ liệu Role ban đầu
            modelBuilder.Entity<Role>().HasData(
                new Role
                {
                    RoleId = 1,
                    RoleName = "Admin"
                },
                new Role
                {
                    RoleId = 2,
                    RoleName = "Student"
                },
                new Role
                {
                    RoleId = 3,
                    RoleName = "Lecturer"
                }
            );
        }
    }
}
