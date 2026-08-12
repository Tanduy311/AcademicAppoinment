using AcademicAppoinment.Helpers;
using AcademicAppoinment.Models;
using AcademicAppoinment.Repositories;
using AcademicAppoinment.Services;
using AcademicAppoinment.Services.Interfaces;
using AcademicAppoinment.Tests.TestHelpers;
using AcademicAppoinment.DTOs.Auth;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

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

            var service = new AuthService(new AppRepository(context), new JwtTokenHelper(TestDbFactory.CreateJwtConfig()), context);

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

            var service = new AuthService(new AppRepository(context), new JwtTokenHelper(TestDbFactory.CreateJwtConfig()), context);

            await Assert.ThrowsExceptionAsync<UnauthorizedAccessException>(() => service.LoginAsync(new LoginDto
            {
                AccountName = "student_01",
                Password = "wrong-password"
            }));
        }

        [TestMethod]
        public async Task UpdateMyAvatarAsync_UpdatesAvatarAndDeletesOldBlob()
        {
            using var context = TestDbFactory.CreateContext(nameof(UpdateMyAvatarAsync_UpdatesAvatarAndDeletesOldBlob));
            SeedRoles(context);
            SeedStudentUser(context);

            var storage = new FakeAvatarStorageService();
            var service = new AuthService(new AppRepository(context), new JwtTokenHelper(TestDbFactory.CreateJwtConfig()), context, storage);
            var principal = TestDbFactory.CreatePrincipal(new Claim(ClaimTypes.NameIdentifier, "1"));
            var file = CreateAvatarFile("avatar.png", "image/png");

            var result = await service.UpdateMyAvatarAsync(file, principal);

            Assert.AreEqual("https://cdn.test/new-avatar.png", result.AvatarUrl);
            Assert.AreEqual("avatar-old.png", storage.DeletedBlobName);

            var user = await context.Users.FirstAsync(u => u.UserId == 1);
            Assert.AreEqual("https://cdn.test/new-avatar.png", user.AvatarUrl);
            Assert.AreEqual("avatar-new.png", user.AvatarBlobName);
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
                AvatarUrl = "https://cdn.test/old-avatar.png",
                AvatarBlobName = "avatar-old.png",
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

        private static IFormFile CreateAvatarFile(string fileName, string contentType)
        {
            var bytes = new byte[] { 1, 2, 3, 4 };
            var stream = new MemoryStream(bytes);
            return new FormFile(stream, 0, bytes.Length, "avatar", fileName)
            {
                Headers = new HeaderDictionary(),
                ContentType = contentType
            };
        }

        private sealed class FakeAvatarStorageService : IAvatarStorageService
        {
            public string? DeletedBlobName { get; private set; }

            public Task DeleteAvatarAsync(string? blobName, CancellationToken cancellationToken = default)
            {
                DeletedBlobName = blobName;
                return Task.CompletedTask;
            }

            public Task<AvatarUploadResult> UploadAvatarAsync(IFormFile file, CancellationToken cancellationToken = default)
            {
                return Task.FromResult(new AvatarUploadResult("https://cdn.test/new-avatar.png", "avatar-new.png"));
            }
        }
    }
}
