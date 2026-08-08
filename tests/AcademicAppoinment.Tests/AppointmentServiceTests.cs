using System;
using Xunit;
using AcademicAppoinment.Models;
using AcademicAppoinment.Services.Appoiment;
using Microsoft.EntityFrameworkCore;

namespace AcademicAppoinment.Tests
{
    public class AppointmentServiceTests
    {
        private AppDbContext CreateInMemoryDb(string dbName)
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(dbName)
                .Options;
            return new AppDbContext(options);
        }

        [Fact]
        public void UT01_CreateAppointment_Succeeds_WhenSlotAvailable()
        {
            using var ctx = CreateInMemoryDb("UT01");

            var user = new User { AccountName = "stu1", PasswordHash="x", EmailAddress="a@a.com", FullName="Stu One", RoleId=2, CreatedAt=DateTime.Now, IsActive=true };
            ctx.Users.Add(user);
            ctx.SaveChanges();
            var student = new Student { UserId = user.UserId, StudentCode = "S001" };
            ctx.Students.Add(student);

            var lecUser = new User { AccountName = "lec1", PasswordHash="x", EmailAddress="b@b.com", FullName="Lec One", RoleId=3, CreatedAt=DateTime.Now, IsActive=true };
            ctx.Users.Add(lecUser);
            ctx.SaveChanges();
            var lecturer = new Lecturer { UserId = lecUser.UserId, LecturerCode = "L001" };
            ctx.Lecturers.Add(lecturer);
            ctx.SaveChanges();

            var slot = new AvailabilitySlot { LecturerId = lecturer.LecturerId, StartTime = DateTime.Now.AddHours(1), EndTime = DateTime.Now.AddHours(2), MeetingType = "Online", IsAvailable = true };
            ctx.AvailabilitySlots.Add(slot);
            ctx.SaveChanges();

            var service = new AppointmentService(ctx);
            var req = new AcademicAppoinment.DTOs.CreateAppointmentRequest { AvailabilitySlotId = slot.AvailabilitySlotId, Topic = "Test" };
            var appt = service.CreateAppointment(user.UserId, req);

            Assert.NotNull(appt);
            var updatedSlot = ctx.AvailabilitySlots.Find(slot.AvailabilitySlotId);
            Assert.False(updatedSlot.IsAvailable);
        }

        [Fact]
        public void UT02_CreateAppointment_Throws_WhenSlotUnavailable()
        {
            using var ctx = CreateInMemoryDb("UT02");

            var user = new User { AccountName = "stu2", PasswordHash="x", EmailAddress="c@c.com", FullName="Stu Two", RoleId=2, CreatedAt=DateTime.Now, IsActive=true };
            ctx.Users.Add(user);
            ctx.SaveChanges();
            var student = new Student { UserId = user.UserId, StudentCode = "S002" };
            ctx.Students.Add(student);

            var lecUser = new User { AccountName = "lec2", PasswordHash="x", EmailAddress="d@d.com", FullName="Lec Two", RoleId=3, CreatedAt=DateTime.Now, IsActive=true };
            ctx.Users.Add(lecUser);
            ctx.SaveChanges();
            var lecturer = new Lecturer { UserId = lecUser.UserId, LecturerCode = "L002" };
            ctx.Lecturers.Add(lecturer);
            ctx.SaveChanges();

            var slot = new AvailabilitySlot { LecturerId = lecturer.LecturerId, StartTime = DateTime.Now.AddHours(1), EndTime = DateTime.Now.AddHours(2), MeetingType = "Online", IsAvailable = false };
            ctx.AvailabilitySlots.Add(slot);
            ctx.SaveChanges();

            var service = new AppointmentService(ctx);
            var req = new AcademicAppoinment.DTOs.CreateAppointmentRequest { AvailabilitySlotId = slot.AvailabilitySlotId, Topic = "Test" };

            Assert.Throws<InvalidOperationException>(() => service.CreateAppointment(user.UserId, req));
        }
    }
}
