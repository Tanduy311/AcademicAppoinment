IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260807113314_InitialCreate'
)
BEGIN
    CREATE TABLE [Roles] (
        [RoleId] int NOT NULL IDENTITY,
        [RoleName] nvarchar(50) NOT NULL,
        CONSTRAINT [PK_Roles] PRIMARY KEY ([RoleId])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260807113314_InitialCreate'
)
BEGIN
    CREATE TABLE [Users] (
        [UserId] int NOT NULL IDENTITY,
        [AccountName] nvarchar(100) NOT NULL,
        [PasswordHash] nvarchar(max) NOT NULL,
        [PhoneNumber] nvarchar(20) NULL,
        [EmailAddress] nvarchar(450) NOT NULL,
        [FullName] nvarchar(50) NOT NULL,
        [RoleId] int NOT NULL,
        [IsActive] bit NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        CONSTRAINT [PK_Users] PRIMARY KEY ([UserId]),
        CONSTRAINT [FK_Users_Roles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [Roles] ([RoleId]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260807113314_InitialCreate'
)
BEGIN
    CREATE TABLE [Lecturers] (
        [LecturerId] int NOT NULL IDENTITY,
        [UserId] int NOT NULL,
        [LecturerCode] nvarchar(50) NOT NULL,
        [Department] nvarchar(150) NULL,
        [Specialization] nvarchar(150) NULL,
        [OfficeLocation] nvarchar(200) NULL,
        [ConsultationDescription] nvarchar(max) NULL,
        CONSTRAINT [PK_Lecturers] PRIMARY KEY ([LecturerId]),
        CONSTRAINT [FK_Lecturers_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([UserId]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260807113314_InitialCreate'
)
BEGIN
    CREATE TABLE [Students] (
        [StudentId] int NOT NULL IDENTITY,
        [UserId] int NOT NULL,
        [StudentCode] nvarchar(50) NOT NULL,
        [Major] nvarchar(150) NULL,
        [ClassName] nvarchar(50) NULL,
        [AcademicYear] nvarchar(20) NULL,
        CONSTRAINT [PK_Students] PRIMARY KEY ([StudentId]),
        CONSTRAINT [FK_Students_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([UserId]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260807113314_InitialCreate'
)
BEGIN
    CREATE TABLE [AvailabilitySlots] (
        [AvailabilitySlotId] int NOT NULL IDENTITY,
        [LecturerId] int NOT NULL,
        [StartTime] datetime2 NOT NULL,
        [EndTime] datetime2 NOT NULL,
        [MeetingType] nvarchar(50) NOT NULL,
        [LocationOrLink] nvarchar(500) NULL,
        [IsAvailable] bit NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        CONSTRAINT [PK_AvailabilitySlots] PRIMARY KEY ([AvailabilitySlotId]),
        CONSTRAINT [FK_AvailabilitySlots_Lecturers_LecturerId] FOREIGN KEY ([LecturerId]) REFERENCES [Lecturers] ([LecturerId]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260807113314_InitialCreate'
)
BEGIN
    CREATE TABLE [Appointments] (
        [AppointmentId] int NOT NULL IDENTITY,
        [StudentId] int NOT NULL,
        [LecturerId] int NOT NULL,
        [AvailabilitySlotId] int NOT NULL,
        [Topic] nvarchar(200) NOT NULL,
        [Description] nvarchar(max) NULL,
        [Status] nvarchar(50) NOT NULL,
        [LecturerResponse] nvarchar(max) NULL,
        [CancellationReason] nvarchar(max) NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NULL,
        CONSTRAINT [PK_Appointments] PRIMARY KEY ([AppointmentId]),
        CONSTRAINT [FK_Appointments_AvailabilitySlots_AvailabilitySlotId] FOREIGN KEY ([AvailabilitySlotId]) REFERENCES [AvailabilitySlots] ([AvailabilitySlotId]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Appointments_Lecturers_LecturerId] FOREIGN KEY ([LecturerId]) REFERENCES [Lecturers] ([LecturerId]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Appointments_Students_StudentId] FOREIGN KEY ([StudentId]) REFERENCES [Students] ([StudentId]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260807113314_InitialCreate'
)
BEGIN
    CREATE TABLE [Notifications] (
        [NotificationId] int NOT NULL IDENTITY,
        [UserId] int NOT NULL,
        [StudentId] int NULL,
        [LecturerId] int NULL,
        [AppointmentId] int NULL,
        [Title] nvarchar(200) NOT NULL,
        [Message] nvarchar(max) NOT NULL,
        [IsRead] bit NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        CONSTRAINT [PK_Notifications] PRIMARY KEY ([NotificationId]),
        CONSTRAINT [FK_Notifications_Appointments_AppointmentId] FOREIGN KEY ([AppointmentId]) REFERENCES [Appointments] ([AppointmentId]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Notifications_Lecturers_LecturerId] FOREIGN KEY ([LecturerId]) REFERENCES [Lecturers] ([LecturerId]),
        CONSTRAINT [FK_Notifications_Students_StudentId] FOREIGN KEY ([StudentId]) REFERENCES [Students] ([StudentId]),
        CONSTRAINT [FK_Notifications_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([UserId]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260807113314_InitialCreate'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'RoleId', N'RoleName') AND [object_id] = OBJECT_ID(N'[Roles]'))
        SET IDENTITY_INSERT [Roles] ON;
    EXEC(N'INSERT INTO [Roles] ([RoleId], [RoleName])
    VALUES (1, N''Admin''),
    (2, N''Student''),
    (3, N''Lecturer'')');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'RoleId', N'RoleName') AND [object_id] = OBJECT_ID(N'[Roles]'))
        SET IDENTITY_INSERT [Roles] OFF;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260807113314_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Appointments_AvailabilitySlotId] ON [Appointments] ([AvailabilitySlotId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260807113314_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Appointments_LecturerId] ON [Appointments] ([LecturerId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260807113314_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Appointments_StudentId] ON [Appointments] ([StudentId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260807113314_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_AvailabilitySlots_LecturerId] ON [AvailabilitySlots] ([LecturerId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260807113314_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Lecturers_LecturerCode] ON [Lecturers] ([LecturerCode]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260807113314_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Lecturers_UserId] ON [Lecturers] ([UserId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260807113314_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Notifications_AppointmentId] ON [Notifications] ([AppointmentId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260807113314_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Notifications_LecturerId] ON [Notifications] ([LecturerId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260807113314_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Notifications_StudentId] ON [Notifications] ([StudentId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260807113314_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Notifications_UserId] ON [Notifications] ([UserId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260807113314_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Students_StudentCode] ON [Students] ([StudentCode]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260807113314_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Students_UserId] ON [Students] ([UserId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260807113314_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Users_AccountName] ON [Users] ([AccountName]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260807113314_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Users_EmailAddress] ON [Users] ([EmailAddress]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260807113314_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Users_RoleId] ON [Users] ([RoleId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260807113314_InitialCreate'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260807113314_InitialCreate', N'8.0.29');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260808075250_Member2_AddStudentProgress'
)
BEGIN
    CREATE TABLE [StudentProgresses] (
        [StudentProgressId] int NOT NULL IDENTITY,
        [StudentId] int NOT NULL,
        [FileName] nvarchar(260) NOT NULL,
        [FilePath] nvarchar(1024) NOT NULL,
        [ContentType] nvarchar(100) NULL,
        [CreatedAt] datetime2 NOT NULL,
        CONSTRAINT [PK_StudentProgresses] PRIMARY KEY ([StudentProgressId]),
        CONSTRAINT [FK_StudentProgresses_Students_StudentId] FOREIGN KEY ([StudentId]) REFERENCES [Students] ([StudentId]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260808075250_Member2_AddStudentProgress'
)
BEGIN
    CREATE INDEX [IX_StudentProgresses_StudentId] ON [StudentProgresses] ([StudentId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260808075250_Member2_AddStudentProgress'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260808075250_Member2_AddStudentProgress', N'8.0.29');
END;
GO

COMMIT;
GO

