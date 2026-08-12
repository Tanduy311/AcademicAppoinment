import { ChangeEvent, FormEvent, useEffect, useState } from 'react';
import { api } from '../services/api';
import type { StudentDetailDto } from '../types/api';
import { IconUser, IconBuilding, IconFileText, IconCheck, IconAlert, IconUpload } from '../components/Icons';
import { useAuth } from '../context/AuthContext';

export function StudentProfilePage() {
  const { refreshMe } = useAuth();
  const [profile, setProfile] = useState<StudentDetailDto | null>(null);
  const [loading, setLoading] = useState(true);
  const [editing, setEditing] = useState(false);
  const [saving, setSaving] = useState(false);
  const [uploadingAvatar, setUploadingAvatar] = useState(false);
  const [msg, setMsg] = useState<{ type: 'success' | 'danger'; text: string } | null>(null);

  const [form, setForm] = useState({
    fullName: '',
    emailAddress: '',
    phoneNumber: '',
    major: '',
    className: '',
    academicYear: '',
  });

  const load = async () => {
    setLoading(true);
    try {
      const data = await api.myStudentProfile();
      setProfile(data);
      setForm({
        fullName: data.fullName || '',
        emailAddress: data.emailAddress || '',
        phoneNumber: data.phoneNumber || '',
        major: data.major || '',
        className: data.className || '',
        academicYear: data.academicYear || '',
      });
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    load();
  }, []);

  const handleAvatarUpload = async (e: ChangeEvent<HTMLInputElement>) => {
    const file = e.target.files?.[0];
    if (!file) return;

    setUploadingAvatar(true);
    setMsg(null);
    try {
      const updatedUser = await api.updateMyAvatar(file);
      setProfile((prev) => prev ? { ...prev, avatarUrl: updatedUser.avatarUrl } : prev);
      await refreshMe();
      setMsg({ type: 'success', text: 'Cập nhật ảnh đại diện thành công!' });
    } catch (err) {
      setMsg({ type: 'danger', text: err instanceof Error ? err.message : 'Không thể cập nhật ảnh đại diện.' });
    } finally {
      setUploadingAvatar(false);
      e.currentTarget.value = '';
    }
  };

  const handleSubmit = async (e: FormEvent) => {
    e.preventDefault();
    setSaving(true);
    setMsg(null);
    try {
      const updated = await api.updateStudentProfile(form);
      setProfile(updated);
      setEditing(false);
      setMsg({ type: 'success', text: 'Cập nhật thông tin hồ sơ sinh viên thành công!' });
    } catch (err) {
      setMsg({ type: 'danger', text: err instanceof Error ? err.message : 'Cập nhật hồ sơ thất bại.' });
    } finally {
      setSaving(false);
    }
  };

  if (loading) {
    return (
      <div className="panel" style={{ textAlign: 'center', padding: 48 }}>
        <div style={{ color: 'var(--text-secondary)' }}>Đang tải thông tin hồ sơ...</div>
      </div>
    );
  }

  if (!profile) {
    return <div className="panel">Không tìm thấy thông tin sinh viên.</div>;
  }

  return (
    <div style={{ display: 'flex', flexDirection: 'column', gap: 24 }}>
      {/* Header Info Panel */}
      <div className="panel" style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', flexWrap: 'wrap', gap: 20 }}>
        <div style={{ display: 'flex', gap: 20, alignItems: 'center', flexWrap: 'wrap' }}>
          <div className="avatar-circle" style={{ width: 64, height: 64, fontSize: '1.75rem', borderRadius: 'var(--radius-lg)', overflow: 'hidden', background: profile.avatarUrl ? '#fff' : undefined }}>
            {profile.avatarUrl ? (
              <img
                src={profile.avatarUrl}
                alt={profile.fullName}
                style={{ width: '100%', height: '100%', objectFit: 'cover' }}
              />
            ) : (
              profile.fullName.charAt(0)
            )}
          </div>

          <div>
            <div style={{ display: 'flex', gap: 12, alignItems: 'center', marginBottom: 4 }}>
              <h1 style={{ fontSize: '1.5rem', fontWeight: 700, color: 'var(--text-primary)' }}>{profile.fullName}</h1>
              <span className="badge badge-success">Sinh viên chính quy</span>
            </div>

            <div style={{ fontSize: '0.9rem', color: 'var(--text-secondary)', display: 'flex', gap: 16, flexWrap: 'wrap' }}>
              <span>Mã SV: <strong style={{ color: 'var(--text-primary)' }}>{profile.studentCode}</strong></span>
              {profile.major && <span>• Ngành: <strong>{profile.major}</strong></span>}
            </div>
          </div>
        </div>

        <button
          type="button"
          className="btn btn-primary"
          onClick={() => {
            setEditing(!editing);
            setMsg(null);
          }}
        >
          {editing ? 'Đóng Form' : '✏️ Chỉnh Sửa Hồ Sơ'}
        </button>
      </div>

      {msg && (
        <div className={`badge badge-${msg.type}`} style={{ padding: 14, borderRadius: 'var(--radius-md)', width: '100%', display: 'flex', alignItems: 'center', gap: 8, textTransform: 'none' }}>
          {msg.type === 'success' ? <IconCheck size={18} /> : <IconAlert size={18} />}
          <span>{msg.text}</span>
        </div>
      )}

      {/* Edit Form Panel */}
      {editing ? (
        <form className="panel" onSubmit={handleSubmit} style={{ display: 'flex', flexDirection: 'column', gap: 20 }}>
          <h2 style={{ fontSize: '1.2rem', fontWeight: 700, display: 'flex', alignItems: 'center', gap: 8 }}>
            <IconFileText size={22} style={{ color: 'var(--accent)' }} />
            <span>Cập Nhật Thông Tin Cá Nhân</span>
          </h2>

          <div style={{ background: '#f8fafc', padding: 16, borderRadius: 'var(--radius-md)', border: '1px solid var(--border-subtle)', display: 'flex', alignItems: 'center', justifyContent: 'space-between', gap: 16, flexWrap: 'wrap' }}>
            <div style={{ display: 'flex', alignItems: 'center', gap: 14 }}>
              <div className="avatar-circle" style={{ width: 72, height: 72, fontSize: '1.5rem', borderRadius: 'var(--radius-lg)', overflow: 'hidden', background: profile.avatarUrl ? '#fff' : undefined }}>
                {profile.avatarUrl ? (
                  <img
                    src={profile.avatarUrl}
                    alt={profile.fullName}
                    style={{ width: '100%', height: '100%', objectFit: 'cover' }}
                  />
                ) : (
                  profile.fullName.charAt(0)
                )}
              </div>
              <div>
                <div style={{ fontWeight: 700, color: 'var(--text-primary)' }}>Ảnh đại diện</div>
                <div style={{ fontSize: '0.85rem', color: 'var(--text-secondary)', marginTop: 4 }}>
                  Hỗ trợ JPG, PNG hoặc WEBP, tối đa 2MB.
                </div>
              </div>
            </div>

            <label className="btn btn-secondary" style={{ display: 'inline-flex', alignItems: 'center', gap: 8, cursor: uploadingAvatar ? 'not-allowed' : 'pointer' }}>
              <IconUpload size={16} />
              <span>{uploadingAvatar ? 'Đang tải...' : 'Đổi ảnh'}</span>
              <input
                type="file"
                accept="image/png,image/jpeg,image/webp"
                onChange={handleAvatarUpload}
                disabled={uploadingAvatar}
                hidden
              />
            </label>
          </div>

          <div className="grid-2">
            <label className="field">
              <span>Họ và Tên</span>
              <input
                type="text"
                value={form.fullName}
                onChange={(e) => setForm((prev) => ({ ...prev, fullName: e.target.value }))}
                required
              />
            </label>

            <label className="field">
              <span>Email Liên Hệ</span>
              <input
                type="email"
                value={form.emailAddress}
                onChange={(e) => setForm((prev) => ({ ...prev, emailAddress: e.target.value }))}
                required
              />
            </label>

            <label className="field">
              <span>Số Điện Thoại</span>
              <input
                type="text"
                value={form.phoneNumber}
                onChange={(e) => setForm((prev) => ({ ...prev, phoneNumber: e.target.value }))}
                placeholder="Nhập SĐT..."
              />
            </label>

            <label className="field">
              <span>Chuyên Ngành (Major)</span>
              <input
                type="text"
                value={form.major}
                onChange={(e) => setForm((prev) => ({ ...prev, major: e.target.value }))}
                placeholder="Ví dụ: Công Nghệ Thông Tin"
              />
            </label>

            <label className="field">
              <span>Lớp Học Phần (Class)</span>
              <input
                type="text"
                value={form.className}
                onChange={(e) => setForm((prev) => ({ ...prev, className: e.target.value }))}
                placeholder="Ví dụ: K65-CNTT1"
              />
            </label>

            <label className="field">
              <span>Khóa Học (Academic Year)</span>
              <input
                type="text"
                value={form.academicYear}
                onChange={(e) => setForm((prev) => ({ ...prev, academicYear: e.target.value }))}
                placeholder="Ví dụ: 2023 - 2027"
              />
            </label>
          </div>

          <div style={{ display: 'flex', gap: 12, justifyContent: 'flex-end' }}>
            <button type="button" className="btn btn-secondary" onClick={() => setEditing(false)}>
              Hủy Bỏ
            </button>
            <button type="submit" className="btn btn-primary" disabled={saving}>
              {saving ? 'Đang Lưu...' : 'Lưu Thay Đổi'}
            </button>
          </div>
        </form>
      ) : null}

      {/* Read-Only Details Panel */}
      <div className="panel" style={{ display: 'flex', flexDirection: 'column', gap: 16 }}>
        <h2 style={{ fontSize: '1.15rem', fontWeight: 600, color: 'var(--text-primary)', display: 'flex', alignItems: 'center', gap: 8 }}>
          <IconFileText size={20} style={{ color: 'var(--accent)' }} />
          <span>Thông Tin Học Tập Chi Tiết</span>
        </h2>

        <div className="grid-2">
          <div style={{ background: '#f8fafc', padding: 16, borderRadius: 'var(--radius-md)', border: '1px solid var(--border-subtle)' }}>
            <div style={{ fontSize: '0.8rem', color: 'var(--text-muted)', marginBottom: 4, display: 'flex', alignItems: 'center', gap: 6 }}>
              <IconBuilding size={15} />
              <span>Chuyên Ngành (Major)</span>
            </div>
            <div style={{ fontSize: '1rem', fontWeight: 600, color: 'var(--text-primary)' }}>
              {profile.major || 'Chưa cập nhật'}
            </div>
          </div>

          <div style={{ background: '#f8fafc', padding: 16, borderRadius: 'var(--radius-md)', border: '1px solid var(--border-subtle)' }}>
            <div style={{ fontSize: '0.8rem', color: 'var(--text-muted)', marginBottom: 4, display: 'flex', alignItems: 'center', gap: 6 }}>
              <IconUser size={15} />
              <span>Lớp Học Phần (Class)</span>
            </div>
            <div style={{ fontSize: '1rem', fontWeight: 600, color: 'var(--text-primary)' }}>
              {profile.className || 'Chưa cập nhật'}
            </div>
          </div>

          <div style={{ background: '#f8fafc', padding: 16, borderRadius: 'var(--radius-md)', border: '1px solid var(--border-subtle)' }}>
            <div style={{ fontSize: '0.8rem', color: 'var(--text-muted)', marginBottom: 4 }}>
              Email Sinh Viên
            </div>
            <div style={{ fontSize: '1rem', fontWeight: 600, color: 'var(--text-primary)' }}>
              {profile.emailAddress}
            </div>
          </div>

          <div style={{ background: '#f8fafc', padding: 16, borderRadius: 'var(--radius-md)', border: '1px solid var(--border-subtle)' }}>
            <div style={{ fontSize: '0.8rem', color: 'var(--text-muted)', marginBottom: 4 }}>
              Số Điện Thoại
            </div>
            <div style={{ fontSize: '1rem', fontWeight: 600, color: 'var(--text-primary)' }}>
              {profile.phoneNumber || 'Chưa cập nhật'}
            </div>
          </div>

          <div style={{ background: '#f8fafc', padding: 16, borderRadius: 'var(--radius-md)', border: '1px solid var(--border-subtle)' }}>
            <div style={{ fontSize: '0.8rem', color: 'var(--text-muted)', marginBottom: 4 }}>
              Khóa Học (Academic Year)
            </div>
            <div style={{ fontSize: '1rem', fontWeight: 600, color: 'var(--text-primary)' }}>
              {profile.academicYear || '2023 - 2027'}
            </div>
          </div>
        </div>
      </div>
    </div>
  );
}
