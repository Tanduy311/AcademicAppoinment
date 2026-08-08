using System;
using Xunit;
using AcademicAppoinment.Models;
using AcademicAppoinment.Services.Lecturers;
using AcademicAppoinment.Services.Appoiment;
using AcademicAppoinment.DTOs;
using Microsoft.EntityFrameworkCore;

namespace AcademicAppoinment.Tests
{
    public class AvailabilitySlotServiceTests
    {
        private AppDbContext CreateInMemoryDb(string dbName)
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(dbName)
                .Options;
            return new AppDbContext(options);
        }

        private (User lecturerUser, Lecturer lecturer) SetupLecturer(AppDbContext ctx, string name)
        {
            var user = new User
            {
                AccountName = name,
                PasswordHash = "hash",
                EmailAddress = $"{name}@test.com",
                FullName = $"Lecturer {name}",
                RoleId = 3,
                CreatedAt = DateTime.Now,
                IsActive = true
            };
            ctx.Users.Add(user);
            ctx.SaveChanges();

            var lecturer = new Lecturer { UserId = user.UserId, LecturerCode = $"L{name}" };
            ctx.Lecturers.Add(lecturer);
            ctx.SaveChanges();

            return (user, lecturer);
        }

        [Fact]
        public void UT03_RejectOverlappingSlots_WhenCreatingConflictingSlot()
        {
            using var ctx = CreateInMemoryDb("UT03_Overlapping");

            var (lecUser, lecturer) = SetupLecturer(ctx, "lec1");

            // Create first slot
            var slot1 = new AvailabilitySlot
            {
                LecturerId = lecturer.LecturerId,
                StartTime = DateTime.Now.AddHours(2),
                EndTime = DateTime.Now.AddHours(3),
                MeetingType = "Online",
                IsAvailable = true
            };
            ctx.AvailabilitySlots.Add(slot1);
            ctx.SaveChanges();

            // Try to create overlapping slot
            var service = new AvailabilitySlotService(ctx);
            var overlappingRequest = new CreateAvailabilitySlotRequest
            {
                StartTime = DateTime.Now.AddHours(2).AddMinutes(30),
                EndTime = DateTime.Now.AddHours(3).AddMinutes(30),
                MeetingType = "Online"
            };

            var result = service.CreateSlot(lecUser.UserId, overlappingRequest, out string error);

            Assert.Null(result);
            Assert.Equal("This time slot overlaps with an existing slot.", error);
        }

        [Fact]
        public void UT04_ApproveAppointment_SucceedsForResponsibleLecturer()
        {
            using var ctx = CreateInMemoryDb("UT04_Approve");

            var (lecUser, lecturer) = SetupLecturer(ctx, "lec1");

            var stuUser = new User
            {
                AccountName = "stu1",
                PasswordHash = "hash",
                EmailAddress = "stu1@test.com",
                FullName = "Student 1",
                RoleId = 2,
                CreatedAt = DateTime.Now,
                IsActive = true
            };
            ctx.Users.Add(stuUser);
            ctx.SaveChanges();

            var student = new Student { UserId = stuUser.UserId, StudentCode = "S001" };
            ctx.Students.Add(student);
            ctx.SaveChanges();

            var slot = new AvailabilitySlot
            {
                LecturerId = lecturer.LecturerId,
                StartTime = DateTime.Now.AddHours(1),
                EndTime = DateTime.Now.AddHours(2),
                MeetingType = "Online",
                IsAvailable = true
            };
            ctx.AvailabilitySlots.Add(slot);
            ctx.SaveChanges();

            var appointment = new Appointment
            {
                StudentId = student.StudentId,
                LecturerId = lecturer.LecturerId,
                AvailabilitySlotId = slot.AvailabilitySlotId,
                Topic = "Test Appointment",
                Status = "Pending",
                CreatedAt = DateTime.Now
            };
            ctx.Appointments.Add(appointment);
            ctx.SaveChanges();

            var service = new AppointmentService(ctx);
            bool success = service.ApproveAppointment(appointment.AppointmentId, lecUser.UserId, out string error);

            Assert.True(success);
            var updated = ctx.Appointments.Find(appointment.AppointmentId);
            Assert.Equal("Approved", updated.Status);
        }

        [Fact]
        public void UT05_ApproveAppointment_ReturnsForbiddenForDifferentLecturer()
        {
            using var ctx = CreateInMemoryDb("UT05_Forbidden");

            var (lecUser1, lecturer1) = SetupLecturer(ctx, "lec1");
            var (lecUser2, lecturer2) = SetupLecturer(ctx, "lec2");

            var stuUser = new User
            {
                AccountName = "stu1",
                PasswordHash = "hash",
                EmailAddress = "stu1@test.com",
                FullName = "Student 1",
                RoleId = 2,
                CreatedAt = DateTime.Now,
                IsActive = true
            };
            ctx.Users.Add(stuUser);
            ctx.SaveChanges();

            var student = new Student { UserId = stuUser.UserId, StudentCode = "S001" };
            ctx.Students.Add(student);
            ctx.SaveChanges();

            var slot = new AvailabilitySlot
            {
                LecturerId = lecturer1.LecturerId,
                StartTime = DateTime.Now.AddHours(1),
                EndTime = DateTime.Now.AddHours(2),
                MeetingType = "Online",
                IsAvailable = true
            };
            ctx.AvailabilitySlots.Add(slot);
            ctx.SaveChanges();

            var appointment = new Appointment
            {
                StudentId = student.StudentId,
                LecturerId = lecturer1.LecturerId,
                AvailabilitySlotId = slot.AvailabilitySlotId,
                Topic = "Test Appointment",
                Status = "Pending",
                CreatedAt = DateTime.Now
            };
            ctx.Appointments.Add(appointment);
            ctx.SaveChanges();

            var service = new AppointmentService(ctx);
            bool success = service.ApproveAppointment(appointment.AppointmentId, lecUser2.UserId, out string error);

            Assert.False(success);
            Assert.Contains("can only approve", error);
        }

        [Fact]
        public void UT06_RejectAppointment_FailsWithoutReason()
        {
            using var ctx = CreateInMemoryDb("UT06_RejectNoReason");

            var (lecUser, lecturer) = SetupLecturer(ctx, "lec1");

            var stuUser = new User
            {
                AccountName = "stu1",
                PasswordHash = "hash",
                EmailAddress = "stu1@test.com",
                FullName = "Student 1",
                RoleId = 2,
                CreatedAt = DateTime.Now,
                IsActive = true
            };
            ctx.Users.Add(stuUser);
            ctx.SaveChanges();

            var student = new Student { UserId = stuUser.UserId, StudentCode = "S001" };
            ctx.Students.Add(student);
            ctx.SaveChanges();

            var slot = new AvailabilitySlot
            {
                LecturerId = lecturer.LecturerId,
                StartTime = DateTime.Now.AddHours(1),
                EndTime = DateTime.Now.AddHours(2),
                MeetingType = "Online",
                IsAvailable = true
            };
            ctx.AvailabilitySlots.Add(slot);
            ctx.SaveChanges();

            var appointment = new Appointment
            {
                StudentId = student.StudentId,
                LecturerId = lecturer.LecturerId,
                AvailabilitySlotId = slot.AvailabilitySlotId,
                Topic = "Test Appointment",
                Status = "Pending",
                CreatedAt = DateTime.Now
            };
            ctx.Appointments.Add(appointment);
            ctx.SaveChanges();

            var service = new AppointmentService(ctx);
            bool success = service.RejectAppointment(appointment.AppointmentId, lecUser.UserId, "", out string error);

            Assert.False(success);
            Assert.Equal("Rejection reason is required.", error);
        }

        [Fact]
        public void UT06_RejectAppointment_SucceedsWithValidReason()
        {
            using var ctx = CreateInMemoryDb("UT06_RejectWithReason");

            var (lecUser, lecturer) = SetupLecturer(ctx, "lec1");

            var stuUser = new User
            {
                AccountName = "stu1",
                PasswordHash = "hash",
                EmailAddress = "stu1@test.com",
                FullName = "Student 1",
                RoleId = 2,
                CreatedAt = DateTime.Now,
                IsActive = true
            };
            ctx.Users.Add(stuUser);
            ctx.SaveChanges();

            var student = new Student { UserId = stuUser.UserId, StudentCode = "S001" };
            ctx.Students.Add(student);
            ctx.SaveChanges();

            var slot = new AvailabilitySlot
            {
                LecturerId = lecturer.LecturerId,
                StartTime = DateTime.Now.AddHours(1),
                EndTime = DateTime.Now.AddHours(2),
                MeetingType = "Online",
                IsAvailable = true
            };
            ctx.AvailabilitySlots.Add(slot);
            ctx.SaveChanges();

            var appointment = new Appointment
            {
                StudentId = student.StudentId,
                LecturerId = lecturer.LecturerId,
                AvailabilitySlotId = slot.AvailabilitySlotId,
                Topic = "Test Appointment",
                Status = "Pending",
                CreatedAt = DateTime.Now
            };
            ctx.Appointments.Add(appointment);
            ctx.SaveChanges();

            var service = new AppointmentService(ctx);
            bool success = service.RejectAppointment(appointment.AppointmentId, lecUser.UserId, "Not available at this time", out string error);

            Assert.True(success);
            var updated = ctx.Appointments.Find(appointment.AppointmentId);
            Assert.Equal("Rejected", updated.Status);
            Assert.Equal("Not available at this time", updated.LecturerResponse);
        }
    }
}
