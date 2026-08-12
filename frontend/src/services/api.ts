import { request } from './apiClient';
import type {
  AdminUserDetailDto,
  AdminUserListItemDto,
  AppointmentResponseDto,
  AuthResponseDto,
  CancelAppointmentDto,
  CreateAppointmentDto,
  CreateSlotDto,
  CurrentUserResponseDto,
  LecturerDetailDto,
  LecturerListItemDto,
  LoginDto,
  NotificationResponseDto,
  RegisterLecturerDto,
  RegisterStudentDto,
  RoleDto,
  SlotResponseDto,
  StudentDetailDto,
  StudentListItemDto,
  UpdateAppointmentStatusDto,
  UpdateUserRoleDto,
  UpdateUserStatusDto,
} from '../types/api';

export const api = {
  login: (dto: LoginDto) =>
    request<AuthResponseDto>('/api/auth/login', {
      method: 'POST',
      body: JSON.stringify(dto),
    }),
  registerStudent: (dto: RegisterStudentDto) =>
    request<AuthResponseDto>('/api/auth/register-student', {
      method: 'POST',
      body: JSON.stringify(dto),
    }),
  registerLecturer: (dto: RegisterLecturerDto) =>
    request<AuthResponseDto>('/api/auth/register-lecturer', {
      method: 'POST',
      body: JSON.stringify(dto),
    }),
  me: () => request<CurrentUserResponseDto>('/api/auth/me'),
  updateMyAvatar: (file: File) => {
    const formData = new FormData();
    formData.append('avatar', file);
    return request<CurrentUserResponseDto>('/api/auth/me/avatar', {
      method: 'PUT',
      body: formData,
    });
  },

  lecturers: () => request<LecturerListItemDto[]>('/api/lecturers'),
  lecturerById: (lecturerId: number) =>
    request<LecturerDetailDto>(`/api/lecturers/${lecturerId}`),

  appointmentsById: (id: number) => request<AppointmentResponseDto>(`/api/appointments/${id}`),
  myAppointments: () => request<AppointmentResponseDto[]>('/api/appointments/my-appointments'),
  lecturerAppointments: () => request<AppointmentResponseDto[]>('/api/appointments/lecturer-appointments'),
  createAppointment: (dto: CreateAppointmentDto) =>
    request<AppointmentResponseDto>('/api/appointments', {
      method: 'POST',
      body: JSON.stringify(dto),
    }),
  updateAppointmentStatus: (id: number, dto: UpdateAppointmentStatusDto) =>
    request<AppointmentResponseDto>(`/api/appointments/${id}/status`, {
      method: 'PUT',
      body: JSON.stringify(dto),
    }),
  cancelAppointment: (id: number, dto: CancelAppointmentDto) =>
    request<AppointmentResponseDto>(`/api/appointments/${id}/cancel`, {
      method: 'PUT',
      body: JSON.stringify(dto),
    }),

  slotsByLecturer: (lecturerId: number) =>
    request<SlotResponseDto[]>(`/api/availabilityslots/lecturer/${lecturerId}`),
  mySlots: () => request<SlotResponseDto[]>('/api/availabilityslots/my-slots'),
  createSlot: (dto: CreateSlotDto) =>
    request<SlotResponseDto>('/api/availabilityslots', {
      method: 'POST',
      body: JSON.stringify(dto),
    }),
  deleteSlot: (id: number) =>
    request<{ message: string }>(`/api/availabilityslots/${id}`, {
      method: 'DELETE',
    }),

  students: () => request<StudentListItemDto[]>('/api/students'),
  studentById: (studentId: number) =>
    request<StudentDetailDto>(`/api/students/${studentId}`),
  myStudentProfile: () => request<StudentDetailDto>('/api/students/me'),
  updateStudentProfile: (dto: import('../types/api').UpdateStudentProfileDto) =>
    request<StudentDetailDto>('/api/students/me', {
      method: 'PUT',
      body: JSON.stringify(dto),
    }),

  myLecturerProfile: () => request<LecturerDetailDto>('/api/lecturers/me'),
  updateLecturerProfile: (dto: import('../types/api').UpdateLecturerProfileDto) =>
    request<LecturerDetailDto>('/api/lecturers/me', {
      method: 'PUT',
      body: JSON.stringify(dto),
    }),

  uploadFile: (file: File, folder?: string) => {
    const formData = new FormData();
    formData.append('file', file);
    const query = folder ? `?folder=${encodeURIComponent(folder)}` : '';
    return request<import('../types/api').FileUploadResponseDto>(`/api/fileupload/upload${query}`, {
      method: 'POST',
      body: formData,
    });
  },

  users: () => request<AdminUserListItemDto[]>('/api/admin/users'),
  userById: (userId: number) => request<AdminUserDetailDto>(`/api/admin/users/${userId}`),
  roles: () => request<RoleDto[]>('/api/admin/roles'),
  updateUserStatus: (userId: number, dto: UpdateUserStatusDto) =>
    request<{ message: string }>(`/api/admin/users/${userId}/status`, {
      method: 'PUT',
      body: JSON.stringify(dto),
    }),
  updateUserRole: (userId: number, dto: UpdateUserRoleDto) =>
    request<{ message: string }>(`/api/admin/users/${userId}/role`, {
      method: 'PUT',
      body: JSON.stringify(dto),
    }),

  notifications: () => request<NotificationResponseDto[]>('/api/notifications'),
  unreadCount: () => request<{ unreadCount: number }>('/api/notifications/unread-count'),
  markNotificationRead: (id: number) =>
    request<{ message: string }>(`/api/notifications/${id}/read`, {
      method: 'PUT',
    }),
};
