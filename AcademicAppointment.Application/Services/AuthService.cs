using AcademicAppointment.Application.DTOs.Auth;
using AcademicAppointment.Application.Interfaces;
using AcademicAppointment.Domain.Entities;

namespace AcademicAppointment.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _users;
        private readonly IStudentRepository _students;
        private readonly ILecturerRepository _lecturers;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly IJwtTokenHelper _jwtTokenHelper;

        public AuthService(
            IUserRepository users,
            IStudentRepository students,
            ILecturerRepository lecturers,
            IUnitOfWork unitOfWork,
            IPasswordHasher passwordHasher,
            IDateTimeProvider dateTimeProvider,
            IJwtTokenHelper jwtTokenHelper)
        {
            _users = users;
            _students = students;
            _lecturers = lecturers;
            _unitOfWork = unitOfWork;
            _passwordHasher = passwordHasher;
            _dateTimeProvider = dateTimeProvider;
            _jwtTokenHelper = jwtTokenHelper;
        }

        public async Task<AuthResponseDto> RegisterStudentAsync(RegisterStudentDto dto)
        {
            if (await _users.ExistsByAccountNameAsync(dto.AccountName))
                throw new InvalidOperationException("Tên tài khoản đã tồn tại.");

            if (await _users.ExistsByEmailAddressAsync(dto.EmailAddress))
                throw new InvalidOperationException("Email đã được sử dụng.");

            if (await _students.ExistsByStudentCodeAsync(dto.StudentCode))
                throw new InvalidOperationException("Mã sinh viên đã tồn tại.");

            var user = new User
            {
                AccountName = dto.AccountName,
                PasswordHash = _passwordHasher.Hash(dto.Password),
                FullName = dto.FullName,
                EmailAddress = dto.EmailAddress,
                PhoneNumber = dto.PhoneNumber,
                RoleId = 2,
                IsActive = true,
                CreatedAt = _dateTimeProvider.UtcNow
            };

            _users.Add(user);
            await _unitOfWork.SaveChangesAsync();

            var student = new Student
            {
                UserId = user.UserId,
                StudentCode = dto.StudentCode,
                Major = dto.Major,
                ClassName = dto.ClassName,
                AcademicYear = dto.AcademicYear
            };

            _students.Add(student);
            await _unitOfWork.SaveChangesAsync();

            var token = _jwtTokenHelper.GenerateToken(user, "Student", studentId: student.StudentId);

            return new AuthResponseDto
            {
                Token = token,
                UserId = user.UserId,
                AccountName = user.AccountName,
                FullName = user.FullName,
                EmailAddress = user.EmailAddress,
                RoleName = "Student",
                StudentId = student.StudentId
            };
        }

        public async Task<AuthResponseDto> RegisterLecturerAsync(RegisterLecturerDto dto)
        {
            if (await _users.ExistsByAccountNameAsync(dto.AccountName))
                throw new InvalidOperationException("Tên tài khoản đã tồn tại.");

            if (await _users.ExistsByEmailAddressAsync(dto.EmailAddress))
                throw new InvalidOperationException("Email đã được sử dụng.");

            if (await _lecturers.ExistsByLecturerCodeAsync(dto.LecturerCode))
                throw new InvalidOperationException("Mã giảng viên đã tồn tại.");

            var user = new User
            {
                AccountName = dto.AccountName,
                PasswordHash = _passwordHasher.Hash(dto.Password),
                FullName = dto.FullName,
                EmailAddress = dto.EmailAddress,
                PhoneNumber = dto.PhoneNumber,
                RoleId = 3,
                IsActive = true,
                CreatedAt = _dateTimeProvider.UtcNow
            };

            _users.Add(user);
            await _unitOfWork.SaveChangesAsync();

            var lecturer = new Lecturer
            {
                UserId = user.UserId,
                LecturerCode = dto.LecturerCode,
                Department = dto.Department,
                Specialization = dto.Specialization,
                OfficeLocation = dto.OfficeLocation,
                ConsultationDescription = dto.ConsultationDescription
            };

            _lecturers.Add(lecturer);
            await _unitOfWork.SaveChangesAsync();

            var token = _jwtTokenHelper.GenerateToken(user, "Lecturer", lecturerId: lecturer.LecturerId);

            return new AuthResponseDto
            {
                Token = token,
                UserId = user.UserId,
                AccountName = user.AccountName,
                FullName = user.FullName,
                EmailAddress = user.EmailAddress,
                RoleName = "Lecturer",
                LecturerId = lecturer.LecturerId
            };
        }

        public async Task<AuthResponseDto> LoginAsync(LoginDto dto)
        {
            var user = await _users.GetByAccountNameWithProfileAsync(dto.AccountName);

            if (user == null || !_passwordHasher.Verify(dto.Password, user.PasswordHash))
            {
                throw new UnauthorizedAccessException("Tên tài khoản hoặc mật khẩu không chính xác.");
            }

            if (!user.IsActive)
            {
                throw new UnauthorizedAccessException("Tài khoản của bạn đã bị khóa.");
            }

            string roleName = user.Role?.RoleName ?? "User";
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
                RoleName = roleName,
                StudentId = studentId,
                LecturerId = lecturerId
            };
        }

        public async Task<CurrentUserDto?> GetCurrentUserAsync(int userId)
        {
            var user = await _users.GetByIdWithProfileAsync(userId);

            if (user == null) return null;

            return new CurrentUserDto
            {
                UserId = user.UserId,
                AccountName = user.AccountName,
                FullName = user.FullName,
                EmailAddress = user.EmailAddress,
                PhoneNumber = user.PhoneNumber,
                RoleName = user.Role?.RoleName,
                StudentInfo = user.Student == null
                    ? null
                    : new StudentProfileDto
                    {
                        StudentId = user.Student.StudentId,
                        StudentCode = user.Student.StudentCode,
                        Major = user.Student.Major,
                        ClassName = user.Student.ClassName,
                        AcademicYear = user.Student.AcademicYear
                    },
                LecturerInfo = user.Lecturer == null
                    ? null
                    : new LecturerProfileDto
                    {
                        LecturerId = user.Lecturer.LecturerId,
                        LecturerCode = user.Lecturer.LecturerCode,
                        Department = user.Lecturer.Department,
                        Specialization = user.Lecturer.Specialization,
                        OfficeLocation = user.Lecturer.OfficeLocation,
                        ConsultationDescription = user.Lecturer.ConsultationDescription
                    }
            };
        }
    }
}

