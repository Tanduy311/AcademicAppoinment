using AcademicAppoinment.Configurations;
using AcademicAppoinment.Models;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
    {
        policy.WithOrigins(
                "http://localhost:5173",
                "http://localhost:4173",
                "http://localhost:3000")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});
builder.Services.AddAcademicAppointmentServices(builder.Configuration);
// Configure the HTTP request pipeline.
var app = builder.Build();

app.UseMiddleware<AcademicAppoinment.Middlewares.GlobalExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    var adminExists = context.Users.Any(u => u.AccountName == "admin");
    if (!adminExists)
    {
        context.Users.Add(new User
        {
            AccountName = "admin",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
            FullName = "System Admin",
            EmailAddress = "admin@test.local",
            PhoneNumber = "0000000000",
            RoleId = 1,
            IsActive = true,
            CreatedAt = DateTime.Now
        });

        context.SaveChanges();
    }

    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("Frontend");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
