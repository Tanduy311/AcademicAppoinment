using AcademicAppoinment.DTOs.Appointments;
using AcademicAppoinment.DTOs.Slot;
using AcademicAppoinment.Models;
using AcademicAppoinment.Repositories;
using AcademicAppoinment.Services;
using AcademicAppoinment.Tests.TestHelpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Security.Claims;

namespace AcademicAppoinment.Tests
{
    [TestClass]
    public class AppointmentServiceTests
    {
        [TestMethod]
        public async Task CancelAppointmentAsync_ReopensSlot()
        {
            using var context = TestDbFactory.CreateContext(nameof(CancelAppointmentAsync_ReopensSlot));
            SeedRoles(context);
            SeedAppointmentGraph(context);

            var service = new AppointmentService(context, new AppRepository(context));
            var principal = TestDbFactory.CreatePrincipal(new Claim("StudentId", "2"));

            var result = await service.CancelAppointmentAsync(1, new CancelAppointmentDto
            {
                CancellationReason = "Need to reschedule"
            }, principal);

            var slot = await context.AvailabilitySlots.FirstAsync();

            Assert.AreEqual("Cancelled", result.Status);
            Assert.IsTrue(slot.IsAvailable);
        }

        [TestMethod]
        public async Task UpdateAppointmentStatusAsync_Rejected_ReopensSlot()
        {
            using var context = TestDbFactory.CreateContext(nameof(UpdateAppointmentStatusAsync_Rejected_ReopensSlot));
            SeedRoles(context);
            SeedAppointmentGraph(context);

            var service = new AppointmentService(context, new AppRepository(context));
            var principal = TestDbFactory.CreatePrincipal(new Claim("LecturerId", "3"));

            var result = await service.UpdateAppointmentStatusAsync(1, new UpdateAppointmentStatusDto
            {
                Status = "Rejected",
                LecturerResponse = "Not available"
            }, principal);

            var slot = await context.AvailabilitySlots.FirstAsync();

            Assert.AreEqual("Rejected", result.Status);
            Assert.IsTrue(slot.IsAvailable);
        }

        [TestMethod]
        public async Task DeleteSlotAsync_SoftDeletesSlot_AndKeepsHistory()
        {
            using var context = TestDbFactory.CreateContext(nameof(DeleteSlotAsync_SoftDeletesSlot_AndKeepsHistory));
            SeedRoles(context);
            SeedAppointmentGraph(context, "Cancelled");
            context.Notifications.Add(new Notification
            {
                NotificationId = 10,
                UserId = 1,
                StudentId = 2,
                LecturerId = 3,
                AppointmentId = 1,
                Title = "History",
                Message = "Keep appointment history",
                CreatedAt = DateTime.UtcNow
            });
            context.SaveChanges();

            var service = new AvailabilitySlotService(context, new AppRepository(context));
            var principal = TestDbFactory.CreatePrincipal(new Claim("LecturerId", "3"));

            await service.DeleteSlotAsync(4, principal);

            var slot = await context.AvailabilitySlots.FindAsync(4);

            Assert.IsNotNull(slot);
            Assert.IsTrue(slot!.IsDeleted);
            Assert.IsFalse(slot.IsAvailable);
            Assert.IsNotNull(await context.Appointments.FindAsync(1));
            Assert.IsNotNull(await context.Notifications.FindAsync(10));
        }

        [TestMethod]
        public async Task CreateSlotAsync_Throws_WhenMeetingTypeIsInvalid()
        {
            using var context = TestDbFactory.CreateContext(nameof(CreateSlotAsync_Throws_WhenMeetingTypeIsInvalid));
            SeedRoles(context);
            SeedLecturerOnly(context);

            var service = new AvailabilitySlotService(context, new AppRepository(context));
            var principal = TestDbFactory.CreatePrincipal(new Claim("LecturerId", "3"));

            await Assert.ThrowsExceptionAsync<ArgumentException>(() =>
                service.CreateSlotAsync(new CreateSlotDto
                {
                    StartTime = DateTime.Now.AddDays(1),
                    EndTime = DateTime.Now.AddDays(1).AddHours(1),
                    MeetingType = "Phone",
                    LocationOrLink = "Room A"
                }, principal));
        }

        private static void SeedRoles(AppDbContext context)
        {
            context.Roles.AddRange(
                new Role { RoleId = 1, RoleName = "Admin" },
                new Role { RoleId = 2, RoleName = "Student" },
                new Role { RoleId = 3, RoleName = "Lecturer" });
            context.SaveChanges();
        }

        private static void SeedAppointmentGraph(AppDbContext context, string appointmentStatus = "Pending")
        {
            var lecturerUser = new User
            {
                UserId = 1,
                AccountName = "lecturer_01",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Password123@"),
                FullName = "Lecturer One",
                EmailAddress = "lecturer@test.local",
                PhoneNumber = "0900000002",
                RoleId = 3,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            var studentUser = new User
            {
                UserId = 2,
                AccountName = "student_01",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Password123@"),
                FullName = "Student One",
                EmailAddress = "student@test.local",
                PhoneNumber = "0900000003",
                RoleId = 2,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            var lecturer = new Lecturer
            {
                LecturerId = 3,
                UserId = 1,
                LecturerCode = "GV001",
                Department = "IT"
            };

            var student = new Student
            {
                StudentId = 2,
                UserId = 2,
                StudentCode = "SV001",
                Major = "IT"
            };

            var slot = new AvailabilitySlot
            {
                AvailabilitySlotId = 4,
                LecturerId = 3,
                StartTime = DateTime.UtcNow.AddDays(1),
                EndTime = DateTime.UtcNow.AddDays(1).AddHours(1),
                MeetingType = "Online",
                IsAvailable = false,
                CreatedAt = DateTime.UtcNow
            };

            var appointment = new Appointment
            {
                AppointmentId = 1,
                StudentId = 2,
                LecturerId = 3,
                AvailabilitySlotId = 4,
                Topic = "Test appointment",
                Status = appointmentStatus,
                CreatedAt = DateTime.UtcNow
            };

            context.Users.AddRange(lecturerUser, studentUser);
            context.Lecturers.Add(lecturer);
            context.Students.Add(student);
            context.AvailabilitySlots.Add(slot);
            context.Appointments.Add(appointment);
            context.SaveChanges();
        }

        private static void SeedLecturerOnly(AppDbContext context)
        {
            var lecturerUser = new User
            {
                UserId = 1,
                AccountName = "lecturer_01",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Password123@"),
                FullName = "Lecturer One",
                EmailAddress = "lecturer@test.local",
                PhoneNumber = "0900000002",
                RoleId = 3,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            var lecturer = new Lecturer
            {
                LecturerId = 3,
                UserId = 1,
                LecturerCode = "GV001",
                Department = "IT"
            };

            context.Users.Add(lecturerUser);
            context.Lecturers.Add(lecturer);
            context.SaveChanges();
        }
    }
}
