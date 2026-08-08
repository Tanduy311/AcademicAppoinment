-- ===================================================================
-- FILE SQL CHẠY DỮ LIỆU MẪU (SEED DATA) CHUẨN XÁC 100% CÓ PASSWORD HASH THẬT
-- Dự án: AcademicAppointment (EF Core Migration: InitialCreate)
-- Database: AcademicConsultationDB
--
-- TÀI KHOẢN & MẬT KHẨU ĐĂNG NHẬP:
-- 1. Tài khoản Admin (accountName: admin)         -> Mật khẩu: Admin@123
-- 2. Tất cả các tài khoản khác (lecturer_a, student_01,...) -> Mật khẩu: Password123@
-- ===================================================================

USE [AcademicConsultationDB];
GO

BEGIN TRANSACTION;

BEGIN TRY

    -- 1. CHÈN DỮ LIỆU BẢNG Roles (Nếu chưa có)
    IF NOT EXISTS (SELECT 1 FROM [Roles] WHERE [RoleId] = 1)
        INSERT INTO [Roles] ([RoleId], [RoleName]) VALUES (1, N'Admin');

    IF NOT EXISTS (SELECT 1 FROM [Roles] WHERE [RoleId] = 2)
        INSERT INTO [Roles] ([RoleId], [RoleName]) VALUES (2, N'Student');

    IF NOT EXISTS (SELECT 1 FROM [Roles] WHERE [RoleId] = 3)
        INSERT INTO [Roles] ([RoleId], [RoleName]) VALUES (3, N'Lecturer');

    -- 2. CHÈN / CẬP NHẬT DỮ LIỆU BẢNG Users
    -- Hash chuẩn từ BCrypt.Net cho Admin@123:    $2a$11$gFOJAI7NipASI/iCahj5MuVhAm0LIGzZK8HvcUKsZz4CTX2wMY4e6
    -- Hash chuẩn từ BCrypt.Net cho Password123@: $2a$11$sCqWnZT3YQ7I9nM0RQDpH.0rOjm1T0ZlBlfrdWoF8J7udMTqfglEG

    -- User Admin (RoleId = 1) - Mật khẩu: Admin@123
    IF NOT EXISTS (SELECT 1 FROM [Users] WHERE [AccountName] = 'admin')
    BEGIN
        INSERT INTO [Users] ([AccountName], [PasswordHash], [FullName], [EmailAddress], [PhoneNumber], [RoleId], [IsActive], [CreatedAt])
        VALUES ('admin', '$2a$11$gFOJAI7NipASI/iCahj5MuVhAm0LIGzZK8HvcUKsZz4CTX2wMY4e6', N'Hệ Thống Quản Trị', 'admin@test.local', '0901111111', 1, 1, GETDATE());
    END
    ELSE
    BEGIN
        UPDATE [Users] 
        SET [PasswordHash] = '$2a$11$gFOJAI7NipASI/iCahj5MuVhAm0LIGzZK8HvcUKsZz4CTX2wMY4e6'
        WHERE [AccountName] = 'admin';
    END

    -- User Giảng viên A (RoleId = 3) - Mật khẩu: Password123@
    IF NOT EXISTS (SELECT 1 FROM [Users] WHERE [AccountName] = 'lecturer_a')
    BEGIN
        INSERT INTO [Users] ([AccountName], [PasswordHash], [FullName], [EmailAddress], [PhoneNumber], [RoleId], [IsActive], [CreatedAt])
        VALUES ('lecturer_a', '$2a$11$sCqWnZT3YQ7I9nM0RQDpH.0rOjm1T0ZlBlfrdWoF8J7udMTqfglEG', N'TS. Nguyễn Văn An', 'an.nguyen@university.edu.vn', '0902222221', 3, 1, GETDATE());
    END
    ELSE
    BEGIN
        UPDATE [Users] 
        SET [PasswordHash] = '$2a$11$sCqWnZT3YQ7I9nM0RQDpH.0rOjm1T0ZlBlfrdWoF8J7udMTqfglEG'
        WHERE [AccountName] = 'lecturer_a';
    END

    -- User Giảng viên B (RoleId = 3) - Mật khẩu: Password123@
    IF NOT EXISTS (SELECT 1 FROM [Users] WHERE [AccountName] = 'lecturer_b')
    BEGIN
        INSERT INTO [Users] ([AccountName], [PasswordHash], [FullName], [EmailAddress], [PhoneNumber], [RoleId], [IsActive], [CreatedAt])
        VALUES ('lecturer_b', '$2a$11$sCqWnZT3YQ7I9nM0RQDpH.0rOjm1T0ZlBlfrdWoF8J7udMTqfglEG', N'PGS.TS. Trần Thị Bình', 'binh.tran@university.edu.vn', '0902222222', 3, 1, GETDATE());
    END
    ELSE
    BEGIN
        UPDATE [Users] 
        SET [PasswordHash] = '$2a$11$sCqWnZT3YQ7I9nM0RQDpH.0rOjm1T0ZlBlfrdWoF8J7udMTqfglEG'
        WHERE [AccountName] = 'lecturer_b';
    END

    -- User Giảng viên C (RoleId = 3) - Mật khẩu: Password123@
    IF NOT EXISTS (SELECT 1 FROM [Users] WHERE [AccountName] = 'lecturer_c')
    BEGIN
        INSERT INTO [Users] ([AccountName], [PasswordHash], [FullName], [EmailAddress], [PhoneNumber], [RoleId], [IsActive], [CreatedAt])
        VALUES ('lecturer_c', '$2a$11$sCqWnZT3YQ7I9nM0RQDpH.0rOjm1T0ZlBlfrdWoF8J7udMTqfglEG', N'ThS. Lê Hoàng Cường', 'cuong.le@university.edu.vn', '0902222223', 3, 1, GETDATE());
    END
    ELSE
    BEGIN
        UPDATE [Users] 
        SET [PasswordHash] = '$2a$11$sCqWnZT3YQ7I9nM0RQDpH.0rOjm1T0ZlBlfrdWoF8J7udMTqfglEG'
        WHERE [AccountName] = 'lecturer_c';
    END

    -- User Sinh viên 1 (RoleId = 2) - Mật khẩu: Password123@
    IF NOT EXISTS (SELECT 1 FROM [Users] WHERE [AccountName] = 'student_01')
    BEGIN
        INSERT INTO [Users] ([AccountName], [PasswordHash], [FullName], [EmailAddress], [PhoneNumber], [RoleId], [IsActive], [CreatedAt])
        VALUES ('student_01', '$2a$11$sCqWnZT3YQ7I9nM0RQDpH.0rOjm1T0ZlBlfrdWoF8J7udMTqfglEG', N'Nguyễn Văn Sinh', 'sinh.nv101@student.edu.vn', '0903333331', 2, 1, GETDATE());
    END
    ELSE
    BEGIN
        UPDATE [Users] 
        SET [PasswordHash] = '$2a$11$sCqWnZT3YQ7I9nM0RQDpH.0rOjm1T0ZlBlfrdWoF8J7udMTqfglEG'
        WHERE [AccountName] = 'student_01';
    END

    -- User Sinh viên 2 (RoleId = 2) - Mật khẩu: Password123@
    IF NOT EXISTS (SELECT 1 FROM [Users] WHERE [AccountName] = 'student_02')
    BEGIN
        INSERT INTO [Users] ([AccountName], [PasswordHash], [FullName], [EmailAddress], [PhoneNumber], [RoleId], [IsActive], [CreatedAt])
        VALUES ('student_02', '$2a$11$sCqWnZT3YQ7I9nM0RQDpH.0rOjm1T0ZlBlfrdWoF8J7udMTqfglEG', N'Phạm Thị Học', 'hoc.pt102@student.edu.vn', '0903333332', 2, 1, GETDATE());
    END
    ELSE
    BEGIN
        UPDATE [Users] 
        SET [PasswordHash] = '$2a$11$sCqWnZT3YQ7I9nM0RQDpH.0rOjm1T0ZlBlfrdWoF8J7udMTqfglEG'
        WHERE [AccountName] = 'student_02';
    END

    -- User Sinh viên 3 (RoleId = 2) - Mật khẩu: Password123@
    IF NOT EXISTS (SELECT 1 FROM [Users] WHERE [AccountName] = 'student_03')
    BEGIN
        INSERT INTO [Users] ([AccountName], [PasswordHash], [FullName], [EmailAddress], [PhoneNumber], [RoleId], [IsActive], [CreatedAt])
        VALUES ('student_03', '$2a$11$sCqWnZT3YQ7I9nM0RQDpH.0rOjm1T0ZlBlfrdWoF8J7udMTqfglEG', N'Trần Văn Viên', 'vien.tv103@student.edu.vn', '0903333333', 2, 1, GETDATE());
    END
    ELSE
    BEGIN
        UPDATE [Users] 
        SET [PasswordHash] = '$2a$11$sCqWnZT3YQ7I9nM0RQDpH.0rOjm1T0ZlBlfrdWoF8J7udMTqfglEG'
        WHERE [AccountName] = 'student_03';
    END

    -- 3. CHÈN DỮ LIỆU BẢNG Lecturers
    DECLARE @UserId_LecA INT = (SELECT [UserId] FROM [Users] WHERE [AccountName] = 'lecturer_a');
    DECLARE @UserId_LecB INT = (SELECT [UserId] FROM [Users] WHERE [AccountName] = 'lecturer_b');
    DECLARE @UserId_LecC INT = (SELECT [UserId] FROM [Users] WHERE [AccountName] = 'lecturer_c');

    IF @UserId_LecA IS NOT NULL AND NOT EXISTS (SELECT 1 FROM [Lecturers] WHERE [LecturerCode] = 'GV001')
        INSERT INTO [Lecturers] ([UserId], [LecturerCode], [Department], [Specialization], [OfficeLocation], [ConsultationDescription])
        VALUES (@UserId_LecA, 'GV001', N'Khoa Công Nghệ Thông Tin', N'Lập trình Web & Kiến trúc Phần mềm', N'Phòng 302-A1', N'Nhận tư vấn đồ án chuyên ngành và hướng dẫn nghiên cứu sinh.');

    IF @UserId_LecB IS NOT NULL AND NOT EXISTS (SELECT 1 FROM [Lecturers] WHERE [LecturerCode] = 'GV002')
        INSERT INTO [Lecturers] ([UserId], [LecturerCode], [Department], [Specialization], [OfficeLocation], [ConsultationDescription])
        VALUES (@UserId_LecB, 'GV002', N'Khoa Khoa Học Máy Tính', N'Trí Tuệ Nhân Tạo & Học Máy', N'Phòng 405-B2', N'Tư vấn định hướng nghiên cứu AI/ML và đăng ký môn học.');

    IF @UserId_LecC IS NOT NULL AND NOT EXISTS (SELECT 1 FROM [Lecturers] WHERE [LecturerCode] = 'GV003')
        INSERT INTO [Lecturers] ([UserId], [LecturerCode], [Department], [Specialization], [OfficeLocation], [ConsultationDescription])
        VALUES (@UserId_LecC, 'GV003', N'Khoa Hệ Thống Thông Tin', N'Phân tích & Cơ sở dữ liệu SQL', N'Phòng 101-C3', N'Hướng dẫn đồ án tốt nghiệp và hệ thống quản trị.');

    -- 4. CHÈN DỮ LIỆU BẢNG Students
    DECLARE @UserId_Stu1 INT = (SELECT [UserId] FROM [Users] WHERE [AccountName] = 'student_01');
    DECLARE @UserId_Stu2 INT = (SELECT [UserId] FROM [Users] WHERE [AccountName] = 'student_02');
    DECLARE @UserId_Stu3 INT = (SELECT [UserId] FROM [Users] WHERE [AccountName] = 'student_03');

    IF @UserId_Stu1 IS NOT NULL AND NOT EXISTS (SELECT 1 FROM [Students] WHERE [StudentCode] = 'SV001')
        INSERT INTO [Students] ([UserId], [StudentCode], [Major], [ClassName], [AcademicYear])
        VALUES (@UserId_Stu1, 'SV001', N'Công Nghệ Thông Tin', N'K65-CNTT1', '2021-2025');

    IF @UserId_Stu2 IS NOT NULL AND NOT EXISTS (SELECT 1 FROM [Students] WHERE [StudentCode] = 'SV002')
        INSERT INTO [Students] ([UserId], [StudentCode], [Major], [ClassName], [AcademicYear])
        VALUES (@UserId_Stu2, 'SV002', N'Khoa Học Máy Tính', N'K66-KHMT2', '2022-2026');

    IF @UserId_Stu3 IS NOT NULL AND NOT EXISTS (SELECT 1 FROM [Students] WHERE [StudentCode] = 'SV003')
        INSERT INTO [Students] ([UserId], [StudentCode], [Major], [ClassName], [AcademicYear])
        VALUES (@UserId_Stu3, 'SV003', N'Hệ Thống Thông Tin', N'K65-HTTT', '2021-2025');

    -- 5. CHÈN DỮ LIỆU BẢNG AvailabilitySlots
    DECLARE @LecId_A INT = (SELECT [LecturerId] FROM [Lecturers] WHERE [LecturerCode] = 'GV001');
    DECLARE @LecId_B INT = (SELECT [LecturerId] FROM [Lecturers] WHERE [LecturerCode] = 'GV002');

    IF @LecId_A IS NOT NULL AND NOT EXISTS (SELECT 1 FROM [AvailabilitySlots] WHERE [LecturerId] = @LecId_A AND [LocationOrLink] = N'Phòng 302-A1')
    BEGIN
        INSERT INTO [AvailabilitySlots] ([LecturerId], [StartTime], [EndTime], [MeetingType], [LocationOrLink], [IsAvailable], [CreatedAt])
        VALUES (@LecId_A, DATEADD(DAY, 1, DATEADD(HOUR, 8, CAST(CAST(GETDATE() AS DATE) AS DATETIME))), DATEADD(DAY, 1, DATEADD(HOUR, 10, CAST(CAST(GETDATE() AS DATE) AS DATETIME))), N'Offline', N'Phòng 302-A1', 0, GETDATE());
    END

    IF @LecId_A IS NOT NULL AND NOT EXISTS (SELECT 1 FROM [AvailabilitySlots] WHERE [LecturerId] = @LecId_A AND [LocationOrLink] LIKE '%meet.google%')
    BEGIN
        INSERT INTO [AvailabilitySlots] ([LecturerId], [StartTime], [EndTime], [MeetingType], [LocationOrLink], [IsAvailable], [CreatedAt])
        VALUES (@LecId_A, DATEADD(DAY, 2, DATEADD(HOUR, 14, CAST(CAST(GETDATE() AS DATE) AS DATETIME))), DATEADD(DAY, 2, DATEADD(HOUR, 16, CAST(CAST(GETDATE() AS DATE) AS DATETIME))), N'Online', N'https://meet.google.com/abc-defg-hij', 1, GETDATE());
    END

    IF @LecId_B IS NOT NULL AND NOT EXISTS (SELECT 1 FROM [AvailabilitySlots] WHERE [LecturerId] = @LecId_B AND [LocationOrLink] = N'Phòng 405-B2')
    BEGIN
        INSERT INTO [AvailabilitySlots] ([LecturerId], [StartTime], [EndTime], [MeetingType], [LocationOrLink], [IsAvailable], [CreatedAt])
        VALUES (@LecId_B, DATEADD(DAY, 1, DATEADD(HOUR, 9, CAST(CAST(GETDATE() AS DATE) AS DATETIME))), DATEADD(DAY, 1, DATEADD(HOUR, 11, CAST(CAST(GETDATE() AS DATE) AS DATETIME))), N'Offline', N'Phòng 405-B2', 1, GETDATE());
    END

    -- 6. CHÈN DỮ LIỆU BẢNG Appointments
    DECLARE @StuId_1 INT = (SELECT [StudentId] FROM [Students] WHERE [StudentCode] = 'SV001');
    DECLARE @SlotId_1 INT = (SELECT TOP 1 [AvailabilitySlotId] FROM [AvailabilitySlots] WHERE [LecturerId] = @LecId_A AND [IsAvailable] = 0);

    IF @StuId_1 IS NOT NULL AND @LecId_A IS NOT NULL AND @SlotId_1 IS NOT NULL 
       AND NOT EXISTS (SELECT 1 FROM [Appointments] WHERE [AvailabilitySlotId] = @SlotId_1)
    BEGIN
        INSERT INTO [Appointments] ([StudentId], [LecturerId], [AvailabilitySlotId], [Topic], [Description], [Status], [LecturerResponse], [CreatedAt], [UpdatedAt])
        VALUES (@StuId_1, @LecId_A, @SlotId_1, N'Tư vấn Đồ án Học kỳ 2', N'Em muốn nhờ Thầy hướng dẫn định hướng đề tài C# ASP.NET Core.', N'Confirmed', N'Thầy đồng ý, em đến đúng giờ nhé.', GETDATE(), GETDATE());
    END

    -- 7. CHÈN DỮ LIỆU BẢNG Notifications
    DECLARE @User_Stu1 INT = (SELECT [UserId] FROM [Users] WHERE [AccountName] = 'student_01');
    DECLARE @User_LecA INT = (SELECT [UserId] FROM [Users] WHERE [AccountName] = 'lecturer_a');
    DECLARE @ApptId_1 INT = (SELECT TOP 1 [AppointmentId] FROM [Appointments]);

    IF @User_LecA IS NOT NULL AND NOT EXISTS (SELECT 1 FROM [Notifications] WHERE [UserId] = @User_LecA AND [Title] = N'Có lịch hẹn mới')
    BEGIN
        INSERT INTO [Notifications] ([UserId], [StudentId], [LecturerId], [AppointmentId], [Title], [Message], [IsRead], [CreatedAt])
        VALUES (@User_LecA, @StuId_1, @LecId_A, @ApptId_1, N'Có lịch hẹn mới', N'Sinh viên Nguyễn Văn Sinh đã đăng ký lịch hẹn với bạn.', 1, GETDATE());
    END

    IF @User_Stu1 IS NOT NULL AND NOT EXISTS (SELECT 1 FROM [Notifications] WHERE [UserId] = @User_Stu1 AND [Title] = N'Lịch hẹn đã được phê duyệt')
    BEGIN
        INSERT INTO [Notifications] ([UserId], [StudentId], [LecturerId], [AppointmentId], [Title], [Message], [IsRead], [CreatedAt])
        VALUES (@User_Stu1, @StuId_1, @LecId_A, @ApptId_1, N'Lịch hẹn đã được phê duyệt', N'Giảng viên TS. Nguyễn Văn An đã phê duyệt lịch hẹn của bạn.', 0, GETDATE());
    END

    COMMIT TRANSACTION;
    PRINT N'=====================================================';
    PRINT N'ĐÃ CẬP NHẬT CSDL THÀNH CÔNG KHỚP VỚI BCRYPT HASH THẬT!';
    PRINT N'Tài khoản admin          -> Mật khẩu: Admin@123';
    PRINT N'Các tài khoản khác        -> Mật khẩu: Password123@';
    PRINT N'=====================================================';

END TRY
BEGIN CATCH
    ROLLBACK TRANSACTION;
    PRINT N'CÓ LỖI XẢY RA TRONG QUÁ TRÌNH CẬP NHẬT:';
    PRINT ERROR_MESSAGE();
END CATCH;
GO
