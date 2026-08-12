using AcademicAppoinment.DTOs.Students;
using AcademicAppoinment.Models;
using AcademicAppoinment.Repositories;
using AcademicAppoinment.Services;
using AcademicAppoinment.Tests.TestHelpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Security.Claims;

namespace AcademicAppoinment.Tests
{
    [TestClass]
    public class StudentServiceTests
    {
        [TestMethod]
        public async Task UpdateMyProfileAsync_UpdatesStudentAndUserFields()
        {
            using var context = TestDbFactory.CreateContext(nameof(UpdateMyProfileAsync_UpdatesStudentAndUserFields));
            SeedRoles(context);
            SeedStudentGraph(context);

            var service = new StudentService(new AppRepository(context), context);
            var principal = TestDbFactory.CreatePrincipal(new Claim(ClaimTypes.NameIdentifier, "1"));

            var result = await service.UpdateMyProfileAsync(new UpdateStudentProfileDto
            {
                FullName = "Updated Student",
                EmailAddress = "updated@student.test",
                PhoneNumber = "0999999999",
                Major = "Data Science",
                ClassName = "K66",
                AcademicYear = "2022-2026"
            }, principal);

            Assert.AreEqual("Updated Student", result.FullName);
            Assert.AreEqual("updated@student.test", result.EmailAddress);
            Assert.AreEqual("Data Science", result.Major);
            Assert.AreEqual("K66", result.ClassName);
            Assert.AreEqual("2022-2026", result.AcademicYear);
        }

        [TestMethod]
        public async Task UpdateMyProfileAsync_Throws_WhenEmailAlreadyUsed()
        {
            using var context = TestDbFactory.CreateContext(nameof(UpdateMyProfileAsync_Throws_WhenEmailAlreadyUsed));
            SeedRoles(context);
            SeedStudentGraph(context);

            context.Users.Add(new User
            {
                UserId = 2,
                AccountName = "student_02",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Password123@"),
                FullName = "Student Two",
                EmailAddress = "used@test.local",
                PhoneNumber = "0900000002",
                RoleId = 2,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            });
            context.SaveChanges();

            var service = new StudentService(new AppRepository(context), context);
            var principal = TestDbFactory.CreatePrincipal(new Claim(ClaimTypes.NameIdentifier, "1"));

            await Assert.ThrowsExceptionAsync<ArgumentException>(() =>
                service.UpdateMyProfileAsync(new UpdateStudentProfileDto
                {
                    EmailAddress = "used@test.local"
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

        private static void SeedStudentGraph(AppDbContext context)
        {
            var user = new User
            {
                UserId = 1,
                AccountName = "student_01",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Password123@"),
                FullName = "Student One",
                EmailAddress = "student1@test.local",
                PhoneNumber = "0900000001",
                RoleId = 2,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            var student = new Student
            {
                StudentId = 1,
                UserId = 1,
                StudentCode = "SV001",
                Major = "IT",
                ClassName = "K65",
                AcademicYear = "2021-2025"
            };

            context.Users.Add(user);
            context.Students.Add(student);
            context.SaveChanges();
        }
    }
}
