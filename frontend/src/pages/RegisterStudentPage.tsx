import { FormEvent, useState } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';
import { IconTeacher, IconAlert, IconArrowRight } from '../components/Icons';

type StudentForm = {
  accountName: string;
  password: string;
  fullName: string;
  emailAddress: string;
  phoneNumber: string;
  studentCode: string;
  major: string;
  className: string;
  academicYear: string;
};

export function RegisterStudentPage() {
  const { registerStudent } = useAuth();
  const navigate = useNavigate();
  const [error, setError] = useState('');
  const [busy, setBusy] = useState(false);
  const [form, setForm] = useState<StudentForm>({
    accountName: '',
    password: '',
    fullName: '',
    emailAddress: '',
    phoneNumber: '',
    studentCode: '',
    major: 'Công Nghệ Thông Tin',
    className: 'CNTT-K65',
    academicYear: '2023 - 2027',
  });

  const submit = async (event: FormEvent) => {
    event.preventDefault();
    if (!form.accountName || !form.password || !form.fullName || !form.studentCode) {
      setError('Vui lòng điền đầy đủ các thông tin bắt buộc.');
      return;
    }

    setBusy(true);
    setError('');
    try {
      await registerStudent(form);
      navigate('/app');
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Đăng ký tài khoản không thành công.');
    } finally {
      setBusy(false);
    }
  };

  return (
    <div className="auth-page">
      <div className="auth-card auth-card-wide">
        <div className="auth-header">
          <div className="brand-logo" style={{ margin: '0 auto 12px auto', width: 48, height: 48 }}>
            <IconTeacher size={26} />
          </div>
          <h1 style={{ fontSize: '1.4rem' }}>Đăng Ký Tài Khoản Sinh Viên</h1>
          <p style={{ fontSize: '0.88rem', color: 'var(--text-secondary)', marginTop: 6 }}>
            Cổng Thông Tin Tư Vấn & Đặt Lịch Hẹn Học Thuật Academic Appointment
          </p>
        </div>

        <form onSubmit={submit} style={{ display: 'flex', flexDirection: 'column', gap: 20 }}>
          {error ? (
            <div className="badge badge-danger" style={{ width: '100%', padding: '10px 14px', borderRadius: 'var(--radius-md)', textTransform: 'none', display: 'flex', alignItems: 'center', gap: 6 }}>
              <IconAlert size={16} />
              <span>{error}</span>
            </div>
          ) : null}

          {/* Section 1: Account Login Credentials */}
          <div style={{ display: 'flex', flexDirection: 'column', gap: 12 }}>
            <div style={{ fontSize: '0.8rem', fontFamily: 'JetBrains Mono', color: 'var(--text-muted)', textTransform: 'uppercase', letterSpacing: '0.05em', borderBottom: '1px solid var(--border-subtle)', paddingBottom: 6 }}>
              Thông tin đăng nhập
            </div>
            <div className="grid-2">
              <label className="field">
                <span>Tên tài khoản (Account Name) *</span>
                <input
                  value={form.accountName}
                  onChange={(e) => setForm((prev) => ({ ...prev, accountName: e.target.value }))}
                  placeholder="Ví dụ: student_02"
                  required
                />
              </label>

              <label className="field">
                <span>Mật khẩu (Password) *</span>
                <input
                  type="password"
                  value={form.password}
                  onChange={(e) => setForm((prev) => ({ ...prev, password: e.target.value }))}
                  placeholder="Tạo mật khẩu an toàn..."
                  required
                />
              </label>
            </div>
          </div>

          {/* Section 2: Personal Information */}
          <div style={{ display: 'flex', flexDirection: 'column', gap: 12 }}>
            <div style={{ fontSize: '0.8rem', fontFamily: 'JetBrains Mono', color: 'var(--text-muted)', textTransform: 'uppercase', letterSpacing: '0.05em', borderBottom: '1px solid var(--border-subtle)', paddingBottom: 6 }}>
              Thông tin cá nhân & liên hệ
            </div>
            <div className="grid-2">
              <label className="field">
                <span>Họ và tên *</span>
                <input
                  value={form.fullName}
                  onChange={(e) => setForm((prev) => ({ ...prev, fullName: e.target.value }))}
                  placeholder="Ví dụ: Nguyễn Văn A"
                  required
                />
              </label>

              <label className="field">
                <span>Địa chỉ Email</span>
                <input
                  type="email"
                  value={form.emailAddress}
                  onChange={(e) => setForm((prev) => ({ ...prev, emailAddress: e.target.value }))}
                  placeholder="student@gmail.com"
                />
              </label>

              <label className="field" style={{ gridColumn: 'span 2' }}>
                <span>Số điện thoại liên hệ</span>
                <input
                  value={form.phoneNumber}
                  onChange={(e) => setForm((prev) => ({ ...prev, phoneNumber: e.target.value }))}
                  placeholder="Ví dụ: 0912345678"
                />
              </label>
            </div>
          </div>

          {/* Section 3: Academic Details */}
          <div style={{ display: 'flex', flexDirection: 'column', gap: 12 }}>
            <div style={{ fontSize: '0.8rem', fontFamily: 'JetBrains Mono', color: 'var(--text-muted)', textTransform: 'uppercase', letterSpacing: '0.05em', borderBottom: '1px solid var(--border-subtle)', paddingBottom: 6 }}>
              Thông tin sinh viên & học tập
            </div>
            <div className="grid-2">
              <label className="field">
                <span>Mã số Sinh Viên (Student Code) *</span>
                <input
                  value={form.studentCode}
                  onChange={(e) => setForm((prev) => ({ ...prev, studentCode: e.target.value }))}
                  placeholder="Ví dụ: SV002"
                  required
                />
              </label>

              <label className="field">
                <span>Chuyên ngành (Major)</span>
                <input
                  value={form.major}
                  onChange={(e) => setForm((prev) => ({ ...prev, major: e.target.value }))}
                  placeholder="Ví dụ: Công Nghệ Thông Tin"
                />
              </label>

              <label className="field">
                <span>Lớp học phần (Class Name)</span>
                <input
                  value={form.className}
                  onChange={(e) => setForm((prev) => ({ ...prev, className: e.target.value }))}
                  placeholder="Ví dụ: CNTT-K65"
                />
              </label>

              <label className="field">
                <span>Khóa học (Academic Year)</span>
                <input
                  value={form.academicYear}
                  onChange={(e) => setForm((prev) => ({ ...prev, academicYear: e.target.value }))}
                  placeholder="Ví dụ: 2023 - 2027"
                />
              </label>
            </div>
          </div>

          <button className="btn btn-primary" type="submit" disabled={busy} style={{ width: '100%', marginTop: 12, display: 'inline-flex', alignItems: 'center', gap: 6 }}>
            <span>{busy ? 'Đang khởi tạo tài khoản...' : 'Tạo Tài Khoản Sinh Viên'}</span>
            <IconArrowRight size={16} />
          </button>
        </form>

        <div style={{ marginTop: 24, paddingTop: 16, borderTop: '1px solid var(--border-subtle)', textAlign: 'center', fontSize: '0.88rem' }}>
          <Link to="/login" style={{ color: 'var(--accent)', fontWeight: 500 }}>
            ← Quay lại trang Đăng Nhập
          </Link>
        </div>
      </div>
    </div>
  );
}
