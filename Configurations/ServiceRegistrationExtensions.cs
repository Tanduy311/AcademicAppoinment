using AcademicAppoinment.Helpers;
using AcademicAppoinment.Models;
using AcademicAppoinment.Repositories;
using AcademicAppoinment.Services;
using AcademicAppoinment.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

namespace AcademicAppoinment.Configurations
{
    public static class ServiceRegistrationExtensions
    {
        public static IServiceCollection AddAcademicAppointmentServices(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            var jwtSettings = configuration.GetSection("JwtSettings");
            var key = jwtSettings["SecretKey"]
                ?? throw new InvalidOperationException("JwtSettings:SecretKey is not configured.");
            var issuer = jwtSettings["Issuer"]
                ?? throw new InvalidOperationException("JwtSettings:Issuer is not configured.");
            var audience = jwtSettings["Audience"]
                ?? throw new InvalidOperationException("JwtSettings:Audience is not configured.");

            services.AddScoped<JwtTokenHelper>();
            services.AddScoped<IAppRepository, AppRepository>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IAppointmentService, AppointmentService>();
            services.AddScoped<IAvailabilitySlotService, AvailabilitySlotService>();
            services.AddScoped<INotificationService, NotificationService>();
            services.AddScoped<ILecturerService, LecturerService>();
            services.AddScoped<IStudentService, StudentService>();
            services.AddScoped<IAdminService, AdminService>();
            services.AddScoped<SupabaseStorageService>();
            services.AddScoped<IAvatarStorageService>(provider => provider.GetRequiredService<SupabaseStorageService>());
            services.AddScoped<ISupabaseStorageService>(provider => provider.GetRequiredService<SupabaseStorageService>());
            services.AddHttpClient();


            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DBConnection")));

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = issuer,
                        ValidAudience = audience,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key))
                    };
                });

            services.AddAuthorization();

            services.AddSwaggerGen(options =>
            {
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "Enter the JWT bearer token.",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT"
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });
            });

            return services;
        }
    }
}
