import { useRef, useState } from 'react';
import { Link } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';
import { api } from '../services/api';
import {
  IconTeacher,
  IconCalendar,
  IconClock,
  IconFileText,
  IconShield,
  IconBell,
  IconSearch,
  IconPlus,
  IconUser,
  IconMail,
  IconPhone,
  IconUpload,
} from '../components/Icons';

export function DashboardPage() {
  const { user, refreshMe } = useAuth();
  const fileInputRef = useRef<HTMLInputElement | null>(null);
  const [uploading, setUploading] = useState(false);
  const [uploadError, setUploadError] = useState<string | null>(null);
  const role = user?.roleName;

  async function handleAvatarChange(file: File | null) {
    if (!file) return;
    setUploading(true);
    setUploadError(null);
    try {
      await api.updateMyAvatar(file);
      await refreshMe();
    } catch (error) {
      setUploadError(error instanceof Error ? error.message : 'Upload avatar thất bại.');
    } finally {
      setUploading(false);
    }
  }

  return (
    <div style={{ display: 'flex', flexDirection: 'column', gap: 24 }}>
      {/* Welcome Banner Panel */}
      <div className="panel" style={{ background: 'linear-gradient(135deg, #eff6ff 0%, #f0fdf4 100%)', borderColor: '#bfdbfe' }}>
        <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', flexWrap: 'wrap', gap: 16 }}>
          <div style={{ display: 'flex', alignItems: 'center', gap: 16, flexWrap: 'wrap' }}>
            <div className="avatar-circle" style={{ width: 72, height: 72, fontSize: '1.5rem', overflow: 'hidden', background: user?.avatarUrl ? '#fff' : undefined }}>
              {user?.avatarUrl ? (
                <img
                  src={user.avatarUrl}
                  alt={user.fullName || 'Avatar'}
                  style={{ width: '100%', height: '100%', objectFit: 'cover' }}
                />
              ) : (
                user?.fullName?.charAt(0).toUpperCase() || 'U'
              )}
            </div>
            <div>
            <div className="pill-role" style={{ marginBottom: 10, display: 'inline-block' }}>{role} PORTAL</div>
            <h1 style={{ fontSize: '1.75rem', fontWeight: 700, letterSpacing: '-0.02em', color: 'var(--text-primary)' }}>
              Xin chào, {user?.fullName || 'Người dùng'}
            </h1>
            <p style={{ color: 'var(--text-secondary)', marginTop: 4, fontSize: '0.95rem' }}>
              Chào mừng bạn đến với Cổng quản lý lịch hẹn tư vấn học thuật khoa Công Nghệ Thông Tin.
            </p>
            </div>
          </div>

          <div style={{ display: 'flex', gap: 12, flexWrap: 'wrap' }}>
            <input
              ref={fileInputRef}
              type="file"
              accept="image/png,image/jpeg,image/webp"
              hidden
              onChange={(e) => {
                const file = e.target.files?.[0] ?? null;
                void handleAvatarChange(file);
                e.currentTarget.value = '';
              }}
            />
            <button
              type="button"
              className="btn btn-secondary"
              style={{ display: 'inline-flex', alignItems: 'center', gap: 8 }}
              onClick={() => fileInputRef.current?.click()}
              disabled={uploading}
            >
              <IconUpload size={16} />
              <span>{uploading ? 'Đang tải...' : 'Đổi Avatar'}</span>
            </button>
            {uploadError && (
              <div style={{ width: '100%', color: 'var(--danger)', fontSize: '0.85rem' }}>
                {uploadError}
              </div>
            )}
            {role === 'Student' && (
              <Link to="/app/lecturers" className="btn btn-primary" style={{ display: 'inline-flex', alignItems: 'center', gap: 8 }}>
                <IconSearch size={16} />
                <span>Tìm Giảng Viên Ngay</span>
              </Link>
            )}
            {role === 'Lecturer' && (
              <Link to="/app/slots" className="btn btn-primary" style={{ display: 'inline-flex', alignItems: 'center', gap: 8 }}>
                <IconPlus size={16} />
                <span>Tạo Slot Rảnh Mới</span>
              </Link>
            )}
            {role === 'Admin' && (
              <Link to="/app/admin/users" className="btn btn-primary" style={{ display: 'inline-flex', alignItems: 'center', gap: 8 }}>
                <IconShield size={16} />
                <span>Quản Lý Hệ Thống</span>
              </Link>
            )}
          </div>
        </div>
      </div>

      {/* Bento Grid Stats & Actions */}
      <div className="bento-grid">
        {/* User Card Info */}
        <div className="panel bento-col-4">
          <div style={{ fontSize: '0.75rem', fontFamily: 'JetBrains Mono', color: 'var(--text-muted)', textTransform: 'uppercase', marginBottom: 12, display: 'flex', alignItems: 'center', gap: 6 }}>
            <IconUser size={14} />
            <span>Thông tin tài khoản</span>
          </div>
          <div style={{ display: 'flex', flexDirection: 'column', gap: 10 }}>
            <div style={{ fontSize: '1.1rem', fontWeight: 600 }}>{user?.fullName}</div>
            <div style={{ fontSize: '0.85rem', color: 'var(--text-secondary)', display: 'flex', alignItems: 'center', gap: 6 }}>
              <IconMail size={15} style={{ color: 'var(--text-muted)' }} />
              <span>{user?.emailAddress}</span>
            </div>
            <div style={{ fontSize: '0.85rem', color: 'var(--text-secondary)', display: 'flex', alignItems: 'center', gap: 6 }}>
              <IconPhone size={15} style={{ color: 'var(--text-muted)' }} />
              <span>{user?.phoneNumber || 'Chưa cập nhật SĐT'}</span>
            </div>
            <div style={{ marginTop: 6 }}>
              <span className="badge badge-success">Tài Khoản Hoạt Động</span>
            </div>
          </div>
        </div>

        {/* Quick Action Navigation Grid */}
        <div className="panel bento-col-8">
          <div style={{ fontSize: '0.75rem', fontFamily: 'JetBrains Mono', color: 'var(--text-muted)', textTransform: 'uppercase', marginBottom: 16 }}>
            Thao tác nhanh hệ thống
          </div>

          <div className="grid-2">
            {role === 'Student' && (
              <>
                <Link to="/app/lecturers" className="panel panel-interactive" style={{ textDecoration: 'none' }}>
                  <div style={{ marginBottom: 10, color: 'var(--accent)' }}>
                    <IconTeacher size={26} />
                  </div>
                  <div style={{ fontWeight: 600, color: 'var(--text-primary)' }}>Danh Sách Giảng Viên</div>
                  <div style={{ fontSize: '0.8rem', color: 'var(--text-secondary)', marginTop: 4 }}>
                    Xem hồ sơ, phòng làm việc và đặt lịch tư vấn.
                  </div>
                </Link>
                <Link to="/app/appointments" className="panel panel-interactive" style={{ textDecoration: 'none' }}>
                  <div style={{ marginBottom: 10, color: 'var(--accent)' }}>
                    <IconCalendar size={26} />
                  </div>
                  <div style={{ fontWeight: 600, color: 'var(--text-primary)' }}>Lịch Hẹn Của Tôi</div>
                  <div style={{ fontSize: '0.8rem', color: 'var(--text-secondary)', marginTop: 4 }}>
                    Theo dõi tiến độ duyệt lịch và trạng thái phản hồi.
                  </div>
                </Link>
              </>
            )}

            {role === 'Lecturer' && (
              <>
                <Link to="/app/slots" className="panel panel-interactive" style={{ textDecoration: 'none' }}>
                  <div style={{ marginBottom: 10, color: 'var(--accent)' }}>
                    <IconClock size={26} />
                  </div>
                  <div style={{ fontWeight: 600, color: 'var(--text-primary)' }}>Quản Lý Khung Giờ Rảnh</div>
                  <div style={{ fontSize: '0.8rem', color: 'var(--text-secondary)', marginTop: 4 }}>
                    Tạo slot tư vấn Online / Offline cho sinh viên.
                  </div>
                </Link>
                <Link to="/app/lecturer-appointments" className="panel panel-interactive" style={{ textDecoration: 'none' }}>
                  <div style={{ marginBottom: 10, color: 'var(--accent)' }}>
                    <IconFileText size={26} />
                  </div>
                  <div style={{ fontWeight: 600, color: 'var(--text-primary)' }}>Duyệt Lịch Sinh Viên</div>
                  <div style={{ fontSize: '0.8rem', color: 'var(--text-secondary)', marginTop: 4 }}>
                    Phê duyệt hoặc từ chối lịch hẹn sinh viên gửi lên.
                  </div>
                </Link>
              </>
            )}

            {role === 'Admin' && (
              <>
                <Link to="/app/admin/users" className="panel panel-interactive" style={{ textDecoration: 'none' }}>
                  <div style={{ marginBottom: 10, color: 'var(--accent)' }}>
                    <IconShield size={26} />
                  </div>
                  <div style={{ fontWeight: 600, color: 'var(--text-primary)' }}>Quản Lý Người Dùng</div>
                  <div style={{ fontSize: '0.8rem', color: 'var(--text-secondary)', marginTop: 4 }}>
                    Xem danh sách, phân quyền và khóa/mở tài khoản.
                  </div>
                </Link>
                <Link to="/app/notifications" className="panel panel-interactive" style={{ textDecoration: 'none' }}>
                  <div style={{ marginBottom: 10, color: 'var(--accent)' }}>
                    <IconBell size={26} />
                  </div>
                  <div style={{ fontWeight: 600, color: 'var(--text-primary)' }}>Trung Tâm Thông Báo</div>
                  <div style={{ fontSize: '0.8rem', color: 'var(--text-secondary)', marginTop: 4 }}>
                    Xem tất cả thông báo cập nhật hệ thống real-time.
                  </div>
                </Link>
              </>
            )}
          </div>
        </div>
      </div>
    </div>
  );
}
