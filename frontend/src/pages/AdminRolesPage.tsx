import { useEffect, useState } from 'react';
import { api } from '../services/api';
import type { RoleDto } from '../types/api';
import { IconShield } from '../components/Icons';

export function AdminRolesPage() {
  const [roles, setRoles] = useState<RoleDto[]>([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    api
      .roles()
      .then(setRoles)
      .finally(() => setLoading(false));
  }, []);

  if (loading) {
    return (
      <div className="panel" style={{ textAlign: 'center', padding: 48 }}>
        <div style={{ color: 'var(--text-secondary)' }}>Đang tải danh sách vai trò...</div>
      </div>
    );
  }

  return (
    <div style={{ display: 'flex', flexDirection: 'column', gap: 24 }}>
      <div className="panel">
        <h1 style={{ fontSize: '1.5rem', fontWeight: 700, marginBottom: 6, display: 'flex', alignItems: 'center', gap: 8 }}>
          <IconShield size={24} style={{ color: 'var(--accent)' }} />
          <span>Danh Sách Vai Trò (Roles)</span>
        </h1>
        <p style={{ color: 'var(--text-secondary)', fontSize: '0.9rem' }}>
          Xem danh mục vai trò được cấu hình trên hệ thống phân quyền Academic Appointment.
        </p>
      </div>

      <div className="grid-3">
        {roles.map((role) => (
          <div key={role.roleId} className="panel" style={{ display: 'flex', flexDirection: 'column', gap: 8 }}>
            <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
              <h3 style={{ fontSize: '1.15rem', fontWeight: 700, color: 'var(--text-primary)' }}>
                {role.roleName}
              </h3>
              <span className="pill-role" style={{ fontSize: '0.75rem' }}>
                ID: {role.roleId}
              </span>
            </div>

            <p style={{ fontSize: '0.85rem', color: 'var(--text-secondary)', margin: 0 }}>
              {role.roleName === 'Admin'
                ? 'Quản trị viên toàn quyền quản lý người dùng và cấu hình hệ thống.'
                : role.roleName === 'Lecturer'
                ? 'Giảng viên quản lý slot rảnh và phê duyệt lịch tư vấn sinh viên.'
                : 'Sinh viên tìm kiếm giảng viên và đặt lịch hẹn tư vấn học thuật.'}
            </p>
          </div>
        ))}
      </div>
    </div>
  );
}
