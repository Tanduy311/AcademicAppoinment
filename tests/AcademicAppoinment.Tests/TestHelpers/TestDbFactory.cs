using AcademicAppoinment.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Security.Claims;

namespace AcademicAppoinment.Tests.TestHelpers
{
    public static class TestDbFactory
    {
        public static AppDbContext CreateContext(string dbName)
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(dbName)
                .EnableSensitiveDataLogging()
                .Options;

            return new AppDbContext(options);
        }

        public static ClaimsPrincipal CreatePrincipal(params Claim[] claims)
        {
            var identity = new ClaimsIdentity(claims, "TestAuth");
            return new ClaimsPrincipal(identity);
        }

        public static IConfiguration CreateJwtConfig()
        {
            return new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["JwtSettings:SecretKey"] = "AcademicAppointment_Test_Secret_Key_1234567890",
                    ["JwtSettings:Issuer"] = "AcademicAppointment.Tests",
                    ["JwtSettings:Audience"] = "AcademicAppointment.Tests",
                    ["JwtSettings:ExpiryMinutes"] = "60"
                })
                .Build();
        }
    }
}
