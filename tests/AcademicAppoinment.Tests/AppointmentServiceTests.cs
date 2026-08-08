using AcademicAppoinment.DTOs.Appointments;
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

        private static void SeedRoles(AppDbContext context)
        {
            context.Roles.AddRange(
                new Role { RoleId = 1, RoleName = "Admin" },
                new Role { RoleId = 2, RoleName = "Student" },
                new Role { RoleId = 3, RoleName = "Lecturer" });
            context.SaveChanges();
        }

        private static void SeedAppointmentGraph(AppDbContext context)
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
                Status = "Pending",
                CreatedAt = DateTime.UtcNow
            };

            context.Users.AddRange(lecturerUser, studentUser);
            context.Lecturers.Add(lecturer);
            context.Students.Add(student);
            context.AvailabilitySlots.Add(slot);
            context.Appointments.Add(appointment);
            context.SaveChanges();
        }
    }
}
