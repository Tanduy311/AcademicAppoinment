import { useEffect, useState } from 'react';
import { NavLink, Outlet, useNavigate } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';
import { api } from '../services/api';
import {
  IconTeacher,
  IconCalendar,
  IconClock,
  IconFileText,
  IconShield,
  IconBell,
  IconUser,
} from '../components/Icons';

export function AppShell() {
  const { user, logout } = useAuth();
  const navigate = useNavigate();
  const role = user?.roleName ?? '';
  const [unreadCount, setUnreadCount] = useState<number>(0);

  useEffect(() => {
    let isMounted = true;
    async function fetchUnread() {
      try {
        const res = await api.unreadCount();
        if (isMounted) {
          setUnreadCount(res.unreadCount || 0);
        }
      } catch {
        // Silent catch for background unread fetch
      }
    }

    if (user) {
      fetchUnread();
      const interval = setInterval(fetchUnread, 15000);
      return () => {
        isMounted = false;
        clearInterval(interval);
      };
    }
  }, [user]);

  const navItems = [
    {
      to: '/app',
      label: 'Dashboard',
      show: true,
      icon: (
        <svg className="nav-icon" fill="none" viewBox="0 0 24 24" stroke="currentColor" strokeWidth="2">
          <path strokeLinecap="round" strokeLinejoin="round" d="M3 12l2-2m0 0l7-7 7 7M5 10v10a1 1 0 001 1h3m10-11l2 2m-2-2v10a1 1 0 01-1 1h-3m-6 0a1 1 0 001-1v-4a1 1 0 011-1h2a1 1 0 011 1v4a1 1 0 001 1m-6 0h6" />
        </svg>
      )
    },
    {
      to: '/app/lecturers',
      label: 'Giảng Viên',
      show: true,
      icon: <IconTeacher className="nav-icon" />
    },
    {
      to: '/app/appointments',
      label: 'Lịch Hẹn Của Tôi',
      show: role === 'Student',
      icon: <IconCalendar className="nav-icon" />
    },
    {
      to: '/app/slots',
      label: 'Khung Giờ Rảnh',
      show: role === 'Lecturer',
      icon: <IconClock className="nav-icon" />
    },
    {
      to: '/app/lecturer-appointments',
      label: 'Hàng Đợi Lịch Hẹn',
      show: role === 'Lecturer',
      icon: <IconFileText className="nav-icon" />
    },
    {
      to: '/app/admin/users',
      label: 'Quản Lý Người Dùng',
      show: role === 'Admin',
      icon: <IconShield className="nav-icon" />
    },
    {
      to: '/app/notifications',
      label: 'Thông Báo',
      show: true,
      badge: unreadCount > 0 ? unreadCount : null,
      icon: (
        <div className="unread-bell-badge">
          <IconBell className="nav-icon" />
          {unreadCount > 0 && <span className="unread-indicator-dot" />}
        </div>
      )
    },
    {
      to: '/app/profile',
      label: 'Hồ Sơ Cá Nhân',
      show: role === 'Student',
      icon: <IconUser className="nav-icon" />
    },
  ].filter((item) => item.show);

  return (
    <div className="app-shell">
      <aside className="sidebar">
        <div>
          <div className="brand-section">
            <div className="brand-logo">
              <IconTeacher size={22} />
            </div>
            <div>
              <div className="brand-title">Academic Appointment</div>
              <div className="brand-subtitle">Portal v1.0</div>
            </div>
          </div>

          <div className="user-card-mini">
            <div className="avatar-circle" style={{ overflow: 'hidden', background: user?.avatarUrl ? '#fff' : undefined }}>
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
            <div style={{ minWidth: 0 }}>
              <div className="user-info-name">{user?.fullName || 'User'}</div>
              <div className="user-info-role">{role}</div>
            </div>
          </div>

          <nav className="nav">
            {navItems.map((item) => (
              <NavLink key={item.to} to={item.to} end={item.to === '/app'} className="nav-link">
                {item.icon}
                <span>{item.label}</span>
                {item.badge ? (
                  <span className="badge badge-danger" style={{ marginLeft: 'auto', padding: '2px 6px', fontSize: '0.7rem' }}>
                    {item.badge}
                  </span>
                ) : null}
              </NavLink>
            ))}
          </nav>
        </div>

        <div style={{ paddingTop: '20px', borderTop: '1px solid var(--border-subtle)' }}>
          <button
            className="btn btn-secondary"
            style={{ width: '100%' }}
            type="button"
            onClick={() => {
              logout();
              navigate('/login');
            }}
          >
            <svg style={{ width: 16, height: 16 }} fill="none" viewBox="0 0 24 24" stroke="currentColor" strokeWidth="2">
              <path strokeLinecap="round" strokeLinejoin="round" d="M17 16l4-4m0 0l-4-4m4 4H7m6 4v1a3 3 0 01-3 3H6a3 3 0 01-3-3V7a3 3 0 013-3h4a3 3 0 013 3v1" />
            </svg>
            <span>Đăng Xuất</span>
          </button>
        </div>
      </aside>

      <main className="main">
        <header className="topbar">
          <div>
            <div className="page-title">Hệ Thống Tư Vấn & Đặt Lịch Học Thuật</div>
            <div className="muted small-text" style={{ fontSize: '0.8rem', color: 'var(--text-muted)' }}>
              Academic Appointment & Consultation Portal
            </div>
          </div>
          <div className="pill-role">{role || 'Guest'}</div>
        </header>

        <section className="content">
          <Outlet />
        </section>
      </main>
    </div>
  );
}
