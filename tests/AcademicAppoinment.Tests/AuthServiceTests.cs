using AcademicAppoinment.Helpers;
using AcademicAppoinment.Models;
using AcademicAppoinment.Repositories;
using AcademicAppoinment.Services;
using AcademicAppoinment.Tests.TestHelpers;
using AcademicAppoinment.DTOs.Auth;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AcademicAppoinment.Tests
{
    [TestClass]
    public class AuthServiceTests
    {
        [TestMethod]
        public async Task LoginAsync_ReturnsToken_ForValidCredentials()
        {
            using var context = TestDbFactory.CreateContext(nameof(LoginAsync_ReturnsToken_ForValidCredentials));
            SeedRoles(context);
            SeedStudentUser(context);

            var service = new AuthService(new AppRepository(context), new JwtTokenHelper(TestDbFactory.CreateJwtConfig()));

            var result = await service.LoginAsync(new LoginDto
            {
                AccountName = "student_01",
                Password = "Password123@"
            });

            Assert.IsFalse(string.IsNullOrWhiteSpace(result.Token));
            Assert.AreEqual("Student", result.RoleName);
            Assert.AreEqual(2, result.StudentId);
        }

        [TestMethod]
        public async Task LoginAsync_Throws_WhenPasswordIsWrong()
        {
            using var context = TestDbFactory.CreateContext(nameof(LoginAsync_Throws_WhenPasswordIsWrong));
            SeedRoles(context);
            SeedStudentUser(context);

            var service = new AuthService(new AppRepository(context), new JwtTokenHelper(TestDbFactory.CreateJwtConfig()));

            await Assert.ThrowsExceptionAsync<UnauthorizedAccessException>(() => service.LoginAsync(new LoginDto
            {
                AccountName = "student_01",
                Password = "wrong-password"
            }));
        }

        private static void SeedRoles(AppDbContext context)
        {
            context.Roles.AddRange(
                new Role { RoleId = 1, RoleName = "Admin" },
                new Role { RoleId = 2, RoleName = "Student" },
                new Role { RoleId = 3, RoleName = "Lecturer" });
            context.SaveChanges();
        }

        private static void SeedStudentUser(AppDbContext context)
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
                StudentId = 2,
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
