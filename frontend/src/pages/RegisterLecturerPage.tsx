import { FormEvent, useState } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';
import { IconTeacher, IconAlert, IconArrowRight } from '../components/Icons';

type LecturerForm = {
  accountName: string;
  password: string;
  fullName: string;
  emailAddress: string;
  phoneNumber: string;
  lecturerCode: string;
  department: string;
  specialization: string;
  officeLocation: string;
  consultationDescription: string;
};

export function RegisterLecturerPage() {
  const { registerLecturer } = useAuth();
  const navigate = useNavigate();
  const [error, setError] = useState('');
  const [busy, setBusy] = useState(false);
  const [form, setForm] = useState<LecturerForm>({
    accountName: '',
    password: '',
    fullName: '',
    emailAddress: '',
    phoneNumber: '',
    lecturerCode: '',
    department: 'Khoa Công Nghệ Thông Tin',
    specialization: 'Khoa Học Dữ Liệu & AI',
    officeLocation: 'Phòng 302-A1',
    consultationDescription: 'Tư vấn đồ án tốt nghiệp và định hướng nghiên cứu sinh.',
  });

  const submit = async (event: FormEvent) => {
    event.preventDefault();
    if (!form.accountName || !form.password || !form.fullName || !form.lecturerCode) {
      setError('Vui lòng điền đầy đủ các thông tin bắt buộc.');
      return;
    }

    setBusy(true);
    setError('');
    try {
      await registerLecturer(form);
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
          <h1 style={{ fontSize: '1.4rem' }}>Đăng Ký Tài Khoản Giảng Viên</h1>
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
                  placeholder="Ví dụ: lecturer_d"
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
              Thông tin Giảng viên & liên hệ
            </div>
            <div className="grid-2">
              <label className="field">
                <span>Họ và tên *</span>
                <input
                  value={form.fullName}
                  onChange={(e) => setForm((prev) => ({ ...prev, fullName: e.target.value }))}
                  placeholder="Ví dụ: PGS.TS. Nguyễn Văn B"
                  required
                />
              </label>

              <label className="field">
                <span>Địa chỉ Email</span>
                <input
                  type="email"
                  value={form.emailAddress}
                  onChange={(e) => setForm((prev) => ({ ...prev, emailAddress: e.target.value }))}
                  placeholder="lecturer@vnu.edu.vn"
                />
              </label>

              <label className="field">
                <span>Mã Giảng Viên (Lecturer Code) *</span>
                <input
                  value={form.lecturerCode}
                  onChange={(e) => setForm((prev) => ({ ...prev, lecturerCode: e.target.value }))}
                  placeholder="Ví dụ: GV004"
                  required
                />
              </label>

              <label className="field">
                <span>Số điện thoại liên hệ</span>
                <input
                  value={form.phoneNumber}
                  onChange={(e) => setForm((prev) => ({ ...prev, phoneNumber: e.target.value }))}
                  placeholder="Ví dụ: 0912345678"
                />
              </label>
            </div>
          </div>

          {/* Section 3: Academic Work & Consultation Details */}
          <div style={{ display: 'flex', flexDirection: 'column', gap: 12 }}>
            <div style={{ fontSize: '0.8rem', fontFamily: 'JetBrains Mono', color: 'var(--text-muted)', textTransform: 'uppercase', letterSpacing: '0.05em', borderBottom: '1px solid var(--border-subtle)', paddingBottom: 6 }}>
              Khoa công tác & Chuyên môn
            </div>
            <div className="grid-2">
              <label className="field">
                <span>Khoa / Viện công tác</span>
                <input
                  value={form.department}
                  onChange={(e) => setForm((prev) => ({ ...prev, department: e.target.value }))}
                  placeholder="Ví dụ: Khoa Công Nghệ Thông Tin"
                />
              </label>

              <label className="field">
                <span>Chuyên môn chính</span>
                <input
                  value={form.specialization}
                  onChange={(e) => setForm((prev) => ({ ...prev, specialization: e.target.value }))}
                  placeholder="Ví dụ: Trí Tuệ Nhân Tạo & AI"
                />
              </label>

              <label className="field" style={{ gridColumn: 'span 2' }}>
                <span>Văn phòng làm việc</span>
                <input
                  value={form.officeLocation}
                  onChange={(e) => setForm((prev) => ({ ...prev, officeLocation: e.target.value }))}
                  placeholder="Ví dụ: Phòng 302-A1"
                />
              </label>

              <label className="field" style={{ gridColumn: 'span 2' }}>
                <span>Mô tả tư vấn & Định hướng tiếp nhận sinh viên</span>
                <textarea
                  rows={3}
                  value={form.consultationDescription}
                  onChange={(e) => setForm((prev) => ({ ...prev, consultationDescription: e.target.value }))}
                  placeholder="Mô tả các lĩnh vực tư vấn..."
                />
              </label>
            </div>
          </div>

          <button className="btn btn-primary" type="submit" disabled={busy} style={{ width: '100%', marginTop: 12, display: 'inline-flex', alignItems: 'center', gap: 6 }}>
            <span>{busy ? 'Đang khởi tạo tài khoản...' : 'Tạo Tài Khoản Giảng Viên'}</span>
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
