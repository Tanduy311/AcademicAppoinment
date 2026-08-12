export type RoleName = 'Admin' | 'Student' | 'Lecturer' | string;

export interface AuthResponseDto {
  token: string;
  userId: number;
  accountName: string;
  fullName: string;
  emailAddress: string;
  avatarUrl?: string | null;
  roleName: string;
  studentId?: number | null;
  lecturerId?: number | null;
}

export interface CurrentUserResponseDto {
  userId: number;
  accountName: string;
  fullName: string;
  emailAddress: string;
  phoneNumber?: string | null;
  avatarUrl?: string | null;
  roleName?: string | null;
  studentInfo?: StudentInfo | null;
  lecturerInfo?: LecturerInfo | null;
}

export interface StudentInfo {
  studentId: number;
  studentCode: string;
  major?: string | null;
  className?: string | null;
  academicYear?: string | null;
}

export interface LecturerInfo {
  lecturerId: number;
  lecturerCode: string;
  department?: string | null;
  specialization?: string | null;
  officeLocation?: string | null;
  consultationDescription?: string | null;
}

export interface LecturerListItemDto {
  lecturerId: number;
  lecturerCode: string;
  fullName: string;
  emailAddress: string;
  phoneNumber?: string | null;
  department?: string | null;
  specialization?: string | null;
  officeLocation?: string | null;
  consultationDescription?: string | null;
  upcomingSlotCount: number;
}

export interface SlotResponseDto {
  availabilitySlotId: number;
  lecturerId: number;
  lecturerName: string;
  department?: string | null;
  startTime: string;
  endTime: string;
  meetingType: string;
  locationOrLink?: string | null;
  isAvailable: boolean;
  createdAt: string;
}

export interface LecturerDetailDto {
  lecturerId: number;
  lecturerCode: string;
  fullName: string;
  emailAddress: string;
  phoneNumber?: string | null;
  department?: string | null;
  specialization?: string | null;
  officeLocation?: string | null;
  consultationDescription?: string | null;
  upcomingSlots: SlotResponseDto[];
}

export interface AppointmentResponseDto {
  appointmentId: number;
  studentId: number;
  studentName: string;
  studentCode?: string | null;
  lecturerId: number;
  lecturerName: string;
  department?: string | null;
  availabilitySlotId: number;
  startTime: string;
  endTime: string;
  meetingType: string;
  locationOrLink?: string | null;
  topic: string;
  description?: string | null;
  status: string;
  lecturerResponse?: string | null;
  cancellationReason?: string | null;
  createdAt: string;
  updatedAt?: string | null;
}

export interface StudentListItemDto {
  studentId: number;
  studentCode: string;
  fullName: string;
  emailAddress: string;
  phoneNumber?: string | null;
  avatarUrl?: string | null;
  major?: string | null;
  className?: string | null;
  academicYear?: string | null;
  appointmentCount: number;
}

export interface StudentDetailDto extends StudentListItemDto {
  appointments: AppointmentResponseDto[];
}

export interface AdminUserListItemDto {
  userId: number;
  accountName: string;
  fullName: string;
  emailAddress: string;
  phoneNumber?: string | null;
  roleName: string;
  isActive: boolean;
  studentId?: number | null;
  lecturerId?: number | null;
  createdAt: string;
}

export interface AdminUserDetailDto extends AdminUserListItemDto {
  studentInfo?: {
    studentId: number;
    studentCode: string;
    major?: string | null;
    className?: string | null;
    academicYear?: string | null;
  } | null;
  lecturerInfo?: {
    lecturerId: number;
    lecturerCode: string;
    department?: string | null;
    specialization?: string | null;
    officeLocation?: string | null;
    consultationDescription?: string | null;
  } | null;
}

export interface RoleDto {
  roleId: number;
  roleName: string;
}

export interface NotificationResponseDto {
  notificationId: number;
  userId: number;
  studentId?: number | null;
  lecturerId?: number | null;
  appointmentId?: number | null;
  title: string;
  message: string;
  isRead: boolean;
  createdAt: string;
}

export interface LoginDto {
  accountName: string;
  password: string;
}

export interface RegisterStudentDto {
  accountName: string;
  password: string;
  fullName: string;
  emailAddress: string;
  phoneNumber?: string;
  studentCode: string;
  major?: string;
  className?: string;
  academicYear?: string;
}

export interface RegisterLecturerDto {
  accountName: string;
  password: string;
  fullName: string;
  emailAddress: string;
  phoneNumber?: string;
  lecturerCode: string;
  department?: string;
  specialization?: string;
  officeLocation?: string;
  consultationDescription?: string;
}

export interface CreateAppointmentDto {
  availabilitySlotId: number;
  topic: string;
  description?: string;
}

export interface CancelAppointmentDto {
  cancellationReason?: string;
}

export interface UpdateAppointmentStatusDto {
  status: string;
  lecturerResponse?: string;
}

export interface CreateSlotDto {
  startTime: string;
  endTime: string;
  meetingType: string;
  locationOrLink?: string;
}

export interface AdminUserListItemDto {
  userId: number;
  accountName: string;
  fullName: string;
  emailAddress: string;
  phoneNumber?: string | null;
  roleName: string;
  isActive: boolean;
  studentId?: number | null;
  lecturerId?: number | null;
  createdAt: string;
}

export interface AdminUserDetailDto extends AdminUserListItemDto {
  studentInfo?: {
    studentId: number;
    studentCode: string;
    major?: string | null;
    className?: string | null;
    academicYear?: string | null;
  } | null;
  lecturerInfo?: {
    lecturerId: number;
    lecturerCode: string;
    department?: string | null;
    specialization?: string | null;
    officeLocation?: string | null;
    consultationDescription?: string | null;
  } | null;
}

export interface RoleDto {
  roleId: number;
  roleName: string;
}

export interface NotificationResponseDto {
  notificationId: number;
  userId: number;
  studentId?: number | null;
  lecturerId?: number | null;
  appointmentId?: number | null;
  title: string;
  message: string;
  isRead: boolean;
  createdAt: string;
}

export interface LoginDto {
  accountName: string;
  password: string;
}

export interface RegisterStudentDto {
  accountName: string;
  password: string;
  fullName: string;
  emailAddress: string;
  phoneNumber?: string;
  studentCode: string;
  major?: string;
  className?: string;
  academicYear?: string;
}

export interface RegisterLecturerDto {
  accountName: string;
  password: string;
  fullName: string;
  emailAddress: string;
  phoneNumber?: string;
  lecturerCode: string;
  department?: string;
  specialization?: string;
  officeLocation?: string;
  consultationDescription?: string;
}

export interface CreateAppointmentDto {
  availabilitySlotId: number;
  topic: string;
  description?: string;
}

export interface CancelAppointmentDto {
  cancellationReason?: string;
}

export interface UpdateAppointmentStatusDto {
  status: string;
  lecturerResponse?: string;
}

export interface CreateSlotDto {
  startTime: string;
  endTime: string;
  meetingType: string;
  locationOrLink?: string;
}

export interface UpdateUserStatusDto {
  isActive: boolean;
}

export interface UpdateUserRoleDto {
  roleId: number;
}

export interface UpdateStudentProfileDto {
  fullName: string;
  emailAddress: string;
  phoneNumber?: string;
  major?: string;
  className?: string;
  academicYear?: string;
  academicProgressFileUrl?: string;
}

export interface UpdateLecturerProfileDto {
  fullName: string;
  emailAddress: string;
  phoneNumber?: string;
  department?: string;
  specialization?: string;
  officeLocation?: string;
  consultationDescription?: string;
}

export interface FileUploadResponseDto {
  fileName: string;
  fileUrl: string;
  fileSize: number;
  contentType: string;
}
