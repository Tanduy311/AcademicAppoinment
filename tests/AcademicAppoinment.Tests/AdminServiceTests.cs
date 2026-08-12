using AcademicAppoinment.Helpers.Exceptions;
using AcademicAppoinment.Models;
using AcademicAppoinment.Repositories;
using AcademicAppoinment.Services;
using AcademicAppoinment.Tests.TestHelpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Security.Claims;

namespace AcademicAppoinment.Tests
{
    [TestClass]
    public class AdminServiceTests
    {
        [TestMethod]
        public async Task SetUserActiveAsync_Throws_WhenAdminTargetsSelf()
        {
            using var context = TestDbFactory.CreateContext(nameof(SetUserActiveAsync_Throws_WhenAdminTargetsSelf));
            SeedRoles(context);
            SeedAdminUser(context);

            var service = new AdminService(new AppRepository(context));
            var principal = TestDbFactory.CreatePrincipal(new Claim(ClaimTypes.NameIdentifier, "1"));

            await Assert.ThrowsExceptionAsync<ForbiddenAccessException>(() =>
                service.SetUserActiveAsync(1, false, principal));
        }

        [TestMethod]
        public async Task SetUserRoleAsync_Throws_WhenTargetHasNoLecturerProfile()
        {
            using var context = TestDbFactory.CreateContext(nameof(SetUserRoleAsync_Throws_WhenTargetHasNoLecturerProfile));
            SeedRoles(context);
            SeedAdminUser(context);
            context.Users.Add(new User
            {
                UserId = 2,
                AccountName = "plain_user",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("User@123"),
                FullName = "Plain User",
                EmailAddress = "plain@test.local",
                RoleId = 1,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            });
            context.SaveChanges();

            var service = new AdminService(new AppRepository(context));
            var principal = TestDbFactory.CreatePrincipal(new Claim(ClaimTypes.NameIdentifier, "1"));

            await Assert.ThrowsExceptionAsync<ArgumentException>(() =>
                service.SetUserRoleAsync(2, 3, principal));
        }

        private static void SeedRoles(AppDbContext context)
        {
            context.Roles.AddRange(
                new Role { RoleId = 1, RoleName = "Admin" },
                new Role { RoleId = 2, RoleName = "Student" },
                new Role { RoleId = 3, RoleName = "Lecturer" });
            context.SaveChanges();
        }

        private static void SeedAdminUser(AppDbContext context)
        {
            context.Users.Add(new User
            {
                UserId = 1,
                AccountName = "admin",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
                FullName = "System Admin",
                EmailAddress = "admin@test.local",
                PhoneNumber = "0900000000",
                RoleId = 1,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            });
            context.SaveChanges();
        }
    }
}
