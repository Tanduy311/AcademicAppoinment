using AcademicAppoinment.DTOs.Auth;
using AcademicAppoinment.Helpers;
using AcademicAppoinment.Models;
using AcademicAppoinment.Repositories;
using AcademicAppoinment.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace AcademicAppoinment.Services
{
    public class AuthService : IAuthService
    {
        private readonly IAppRepository _repository;
        private readonly JwtTokenHelper _jwtTokenHelper;
        private readonly AppDbContext _context;
        private readonly IAvatarStorageService? _avatarStorageService;

        public AuthService(
            IAppRepository repository,
            JwtTokenHelper jwtTokenHelper,
            AppDbContext context,
            IAvatarStorageService? avatarStorageService = null)
        {
            _repository = repository;
            _jwtTokenHelper = jwtTokenHelper;
            _context = context;
            _avatarStorageService = avatarStorageService;
        }

        public async Task<AuthResponseDto> RegisterStudentAsync(RegisterStudentDto dto)
        {
            if (await _repository.UserAccountExistsAsync(dto.AccountName))
                throw new ArgumentException("Tên tài khoản đã tồn tại.");

            if (await _repository.UserEmailExistsAsync(dto.EmailAddress))
                throw new ArgumentException("Email đã được sử dụng.");

            if (await _repository.StudentCodeExistsAsync(dto.StudentCode))
                throw new ArgumentException("Mã sinh viên đã tồn tại.");

            await using var transaction = _context.Database.IsRelational()
                ? await _context.Database.BeginTransactionAsync()
                : null;

            var user = new User
            {
                AccountName = dto.AccountName,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                FullName = dto.FullName,
                EmailAddress = dto.EmailAddress,
                PhoneNumber = dto.PhoneNumber,
                RoleId = 2,
                IsActive = true,
                CreatedAt = DateTime.Now
            };

            _repository.AddUser(user);
            await _repository.SaveChangesAsync();

            var student = new Student
            {
                UserId = user.UserId,
                StudentCode = dto.StudentCode,
                Major = dto.Major,
                ClassName = dto.ClassName,
                AcademicYear = dto.AcademicYear
            };

            _repository.AddStudent(student);
            await _repository.SaveChangesAsync();

            if (transaction != null)
            {
                await transaction.CommitAsync();
            }

            var token = _jwtTokenHelper.GenerateToken(user, "Student", studentId: student.StudentId);

            return new AuthResponseDto
            {
                Token = token,
                UserId = user.UserId,
                AccountName = user.AccountName,
                FullName = user.FullName,
                EmailAddress = user.EmailAddress,
                AvatarUrl = user.AvatarUrl,
                RoleName = "Student",
                StudentId = student.StudentId
            };
        }

        public async Task<AuthResponseDto> RegisterLecturerAsync(RegisterLecturerDto dto)
        {
            if (await _repository.UserAccountExistsAsync(dto.AccountName))
                throw new ArgumentException("Tên tài khoản đã tồn tại.");

            if (await _repository.UserEmailExistsAsync(dto.EmailAddress))
                throw new ArgumentException("Email đã được sử dụng.");

            if (await _repository.LecturerCodeExistsAsync(dto.LecturerCode))
                throw new ArgumentException("Mã giảng viên đã tồn tại.");

            await using var transaction = _context.Database.IsRelational()
                ? await _context.Database.BeginTransactionAsync()
                : null;

            var user = new User
            {
                AccountName = dto.AccountName,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                FullName = dto.FullName,
                EmailAddress = dto.EmailAddress,
                PhoneNumber = dto.PhoneNumber,
                RoleId = 3,
                IsActive = true,
                CreatedAt = DateTime.Now
            };

            _repository.AddUser(user);
            await _repository.SaveChangesAsync();

            var lecturer = new Lecturer
            {
                UserId = user.UserId,
                LecturerCode = dto.LecturerCode,
                Department = dto.Department,
                Specialization = dto.Specialization,
                OfficeLocation = dto.OfficeLocation,
                ConsultationDescription = dto.ConsultationDescription
            };

            _repository.AddLecturer(lecturer);
            await _repository.SaveChangesAsync();

            if (transaction != null)
            {
                await transaction.CommitAsync();
            }

            var token = _jwtTokenHelper.GenerateToken(user, "Lecturer", lecturerId: lecturer.LecturerId);

            return new AuthResponseDto
            {
                Token = token,
                UserId = user.UserId,
                AccountName = user.AccountName,
                FullName = user.FullName,
                EmailAddress = user.EmailAddress,
                AvatarUrl = user.AvatarUrl,
                RoleName = "Lecturer",
                LecturerId = lecturer.LecturerId
            };
        }

        public async Task<AuthResponseDto> LoginAsync(LoginDto dto)
        {
            var user = await _repository.GetUserByAccountNameAsync(dto.AccountName);
            if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
            {
                throw new UnauthorizedAccessException("Tên tài khoản hoặc mật khẩu không chính xác.");
            }

            if (!user.IsActive)
            {
                throw new UnauthorizedAccessException("Tài khoản của bạn đã bị khóa.");
            }

            var roleName = user.Role?.RoleName ?? "User";
            int? studentId = user.Student?.StudentId;
            int? lecturerId = user.Lecturer?.LecturerId;

            var token = _jwtTokenHelper.GenerateToken(user, roleName, studentId, lecturerId);

            return new AuthResponseDto
            {
                Token = token,
                UserId = user.UserId,
                AccountName = user.AccountName,
                FullName = user.FullName,
                EmailAddress = user.EmailAddress,
                AvatarUrl = user.AvatarUrl,
                RoleName = roleName,
                StudentId = studentId,
                LecturerId = lecturerId
            };
        }

        public async Task<CurrentUserResponseDto> GetCurrentUserAsync(ClaimsPrincipal user)
        {
            var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
            {
                throw new UnauthorizedAccessException();
            }

            var currentUser = await _repository.GetUserWithDetailsByIdAsync(userId);
            if (currentUser == null)
            {
                throw new KeyNotFoundException("Không tìm thấy người dùng.");
            }

            return new CurrentUserResponseDto
            {
                UserId = currentUser.UserId,
                AccountName = currentUser.AccountName,
                FullName = currentUser.FullName,
                EmailAddress = currentUser.EmailAddress,
                PhoneNumber = currentUser.PhoneNumber,
                AvatarUrl = currentUser.AvatarUrl,
                RoleName = currentUser.Role?.RoleName,
                StudentInfo = currentUser.Student,
                LecturerInfo = currentUser.Lecturer
            };
        }

        public async Task<CurrentUserResponseDto> UpdateMyAvatarAsync(IFormFile avatar, ClaimsPrincipal user)
        {
            if (_avatarStorageService == null)
            {
                throw new InvalidOperationException("Avatar storage service is not configured.");
            }

            var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdClaim, out var userId))
            {
                throw new UnauthorizedAccessException();
            }

            var currentUser = await _context.Users.FirstOrDefaultAsync(u => u.UserId == userId);
            if (currentUser == null)
            {
                throw new KeyNotFoundException("KhÃ´ng tÃ¬m tháº¥y ngÆ°á»i dÃ¹ng.");
            }

            var oldBlobName = currentUser.AvatarBlobName;
            var upload = await _avatarStorageService.UploadAvatarAsync(avatar);

            currentUser.AvatarUrl = upload.AvatarUrl;
            currentUser.AvatarBlobName = upload.AvatarBlobName;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch
            {
                await SafeDeleteAvatarAsync(upload.AvatarBlobName);
                throw;
            }

            if (!string.IsNullOrWhiteSpace(oldBlobName) && !string.Equals(oldBlobName, upload.AvatarBlobName, StringComparison.OrdinalIgnoreCase))
            {
                await SafeDeleteAvatarAsync(oldBlobName);
            }

            var refreshed = await _repository.GetUserWithDetailsByIdAsync(userId);
            if (refreshed == null)
            {
                throw new KeyNotFoundException("KhÃ´ng tÃ¬m tháº¥y ngÆ°á»i dÃ¹ng.");
            }

            return new CurrentUserResponseDto
            {
                UserId = refreshed.UserId,
                AccountName = refreshed.AccountName,
                FullName = refreshed.FullName,
                EmailAddress = refreshed.EmailAddress,
                PhoneNumber = refreshed.PhoneNumber,
                AvatarUrl = refreshed.AvatarUrl,
                RoleName = refreshed.Role?.RoleName,
                StudentInfo = refreshed.Student,
                LecturerInfo = refreshed.Lecturer
            };
        }

        private async Task SafeDeleteAvatarAsync(string? blobName)
        {
            if (_avatarStorageService == null || string.IsNullOrWhiteSpace(blobName))
            {
                return;
            }

            try
            {
                await _avatarStorageService.DeleteAvatarAsync(blobName);
            }
            catch
            {
                // Best effort cleanup.
            }
        }
    }
}
