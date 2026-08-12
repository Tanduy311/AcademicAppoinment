import { FormEvent, useState } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';
import { IconUser, IconTeacher, IconShield, IconAlert, IconArrowRight } from '../components/Icons';

export function LoginPage() {
  const { login } = useAuth();
  const navigate = useNavigate();
  const [accountName, setAccountName] = useState('student_01');
  const [password, setPassword] = useState('Password123@');
  const [error, setError] = useState('');
  const [busy, setBusy] = useState(false);

  const onSubmit = async (event: FormEvent) => {
    event.preventDefault();
    setBusy(true);
    setError('');
    try {
      await login({ accountName, password });
      navigate('/app');
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Đăng nhập không thành công.');
    } finally {
      setBusy(false);
    }
  };

  const fillQuickAccount = (acc: string, pass: string) => {
    setAccountName(acc);
    setPassword(pass);
  };

  return (
    <div className="auth-page">
      <div className="auth-card">
        <div className="auth-header">
          <div className="brand-logo" style={{ margin: '0 auto 12px auto', width: 48, height: 48 }}>
            <IconTeacher size={26} />
          </div>
          <h1 style={{ fontSize: '1.4rem' }}>Academic Appointment</h1>
          <p style={{ fontSize: '0.88rem', color: 'var(--text-secondary)', marginTop: 6 }}>
            Cổng Thông Tin Tư Vấn & Đặt Lịch Hẹn Học Thuật
          </p>
        </div>

        {/* Quick Demo Accounts Selection */}
        <div style={{ marginBottom: 24, padding: 12, background: '#f8fafc', borderRadius: 'var(--radius-md)', border: '1px solid var(--border-subtle)' }}>
          <div style={{ fontSize: '0.75rem', fontFamily: 'JetBrains Mono', color: 'var(--text-muted)', marginBottom: 8, textTransform: 'uppercase' }}>
            Tài khoản dùng thử nhanh:
          </div>
          <div style={{ display: 'flex', gap: 8, flexWrap: 'wrap' }}>
            <button
              type="button"
              className="btn btn-secondary"
              style={{ fontSize: '0.75rem', padding: '6px 10px', display: 'inline-flex', alignItems: 'center', gap: 4 }}
              onClick={() => fillQuickAccount('student_01', 'Password123@')}
            >
              <IconUser size={13} />
              <span>Sinh viên</span>
            </button>
            <button
              type="button"
              className="btn btn-secondary"
              style={{ fontSize: '0.75rem', padding: '6px 10px', display: 'inline-flex', alignItems: 'center', gap: 4 }}
              onClick={() => fillQuickAccount('lecturer_a', 'Password123@')}
            >
              <IconTeacher size={13} />
              <span>Giảng viên</span>
            </button>
            <button
              type="button"
              className="btn btn-secondary"
              style={{ fontSize: '0.75rem', padding: '6px 10px', display: 'inline-flex', alignItems: 'center', gap: 4 }}
              onClick={() => fillQuickAccount('admin', 'Admin@123')}
            >
              <IconShield size={13} />
              <span>Admin</span>
            </button>
          </div>
        </div>

        <form onSubmit={onSubmit} style={{ display: 'flex', flexDirection: 'column', gap: 18 }}>
          <label className="field">
            <span>Tên tài khoản (Account Name)</span>
            <input
              value={accountName}
              onChange={(e) => setAccountName(e.target.value)}
              placeholder="Nhập tên tài khoản..."
              required
            />
          </label>

          <label className="field">
            <span>Mật khẩu (Password)</span>
            <input
              type="password"
              value={password}
              onChange={(e) => setPassword(e.target.value)}
              placeholder="Nhập mật khẩu..."
              required
            />
          </label>

          {error ? (
            <div className="badge badge-danger" style={{ width: '100%', padding: '10px 14px', borderRadius: 'var(--radius-md)', textTransform: 'none', display: 'flex', alignItems: 'center', gap: 6 }}>
              <IconAlert size={16} />
              <span>{error}</span>
            </div>
          ) : null}

          <button className="btn btn-primary" type="submit" disabled={busy} style={{ width: '100%', marginTop: 8, display: 'inline-flex', alignItems: 'center', gap: 6 }}>
            <span>{busy ? 'Đang xác thực...' : 'Đăng Nhập ngay'}</span>
            <IconArrowRight size={16} />
          </button>
        </form>

        <div style={{ marginTop: 24, paddingTop: 20, borderTop: '1px solid var(--border-subtle)', display: 'flex', justifyContent: 'space-between', fontSize: '0.85rem' }}>
          <Link to="/register/student" style={{ color: 'var(--accent)', fontWeight: 500 }}>
            + Đăng ký Sinh viên
          </Link>
          <Link to="/register/lecturer" style={{ color: 'var(--text-secondary)', fontWeight: 500 }}>
            + Đăng ký Giảng viên
          </Link>
        </div>
      </div>
    </div>
  );
}
