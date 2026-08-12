import { useEffect, useState } from 'react';
import { api } from '../services/api';
import type { AdminUserListItemDto, RoleDto } from '../types/api';
import {
  IconShield,
  IconUser,
  IconCheck,
  IconClose,
  IconSearch,
} from '../components/Icons';

export function AdminUsersPage() {
  const [users, setUsers] = useState<AdminUserListItemDto[]>([]);
  const [roles, setRoles] = useState<RoleDto[]>([]);
  const [roleSelections, setRoleSelections] = useState<Record<number, number>>({});
  const [loading, setLoading] = useState(true);
  const [busyId, setBusyId] = useState<number | null>(null);
  const [searchQuery, setSearchQuery] = useState('');
  const [selectedRoleFilter, setSelectedRoleFilter] = useState('ALL');

  const load = async () => {
    setLoading(true);
    try {
      const [u, r] = await Promise.all([api.users(), api.roles()]);
      setUsers(u);
      setRoles(r);
      setRoleSelections(
        Object.fromEntries(
          u.map((user) => {
            const currentRole = r.find((role) => role.roleName === user.roleName);
            return [user.userId, currentRole?.roleId ?? r[0]?.roleId ?? 0];
          })
        )
      );
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    load();
  }, []);

  const toggleActive = async (userId: number, current: boolean) => {
    setBusyId(userId);
    try {
      await api.updateUserStatus(userId, { isActive: !current });
      await load();
    } catch (err) {
      alert(err instanceof Error ? err.message : 'Không thể thay đổi trạng thái.');
    } finally {
      setBusyId(null);
    }
  };

  const changeRole = async (userId: number) => {
    const roleId = roleSelections[userId];
    if (!roleId) return;
    setBusyId(userId);
    try {
      await api.updateUserRole(userId, { roleId });
      await load();
    } catch (err) {
      alert(err instanceof Error ? err.message : 'Không thể cập nhật vai trò.');
    } finally {
      setBusyId(null);
    }
  };

  const filteredUsers = users.filter((u) => {
    const matchesSearch =
      u.fullName.toLowerCase().includes(searchQuery.toLowerCase()) ||
      u.accountName.toLowerCase().includes(searchQuery.toLowerCase()) ||
      (u.emailAddress && u.emailAddress.toLowerCase().includes(searchQuery.toLowerCase()));

    const matchesRole = selectedRoleFilter === 'ALL' || u.roleName === selectedRoleFilter;

    return matchesSearch && matchesRole;
  });

  if (loading) {
    return (
      <div className="panel" style={{ textAlign: 'center', padding: 48 }}>
        <div style={{ color: 'var(--text-secondary)' }}>Đang tải danh sách người dùng...</div>
      </div>
    );
  }

  return (
    <div style={{ display: 'flex', flexDirection: 'column', gap: 24 }}>
      {/* Admin Header Banner */}
      <div className="panel">
        <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', flexWrap: 'wrap', gap: 12, marginBottom: 16 }}>
          <div>
            <h1 style={{ fontSize: '1.5rem', fontWeight: 700, marginBottom: 6, display: 'flex', alignItems: 'center', gap: 8 }}>
              <IconShield size={24} style={{ color: 'var(--accent)' }} />
              <span>Quản Lý Người Dùng Hệ Thống</span>
            </h1>
            <p style={{ color: 'var(--text-secondary)', fontSize: '0.9rem' }}>
              Xem danh sách tài khoản, cập nhật vai trò (Role) và quản lý trạng thái mở/khóa tài khoản.
            </p>
          </div>

          <span className="badge badge-neutral" style={{ fontSize: '0.85rem', padding: '6px 12px' }}>
            Tổng số: {users.length} tài khoản
          </span>
        </div>

        {/* Filters bar */}
        <div style={{ display: 'flex', gap: 16, flexWrap: 'wrap' }}>
          <div className="field" style={{ flex: 1, minWidth: 260 }}>
            <span style={{ display: 'flex', alignItems: 'center', gap: 6 }}>
              <IconSearch size={15} />
              <span>Tìm kiếm tài khoản</span>
            </span>
            <input
              placeholder="Nhập họ tên, tên đăng nhập (ngduytan), hoặc email..."
              value={searchQuery}
              onChange={(e) => setSearchQuery(e.target.value)}
            />
          </div>

          <div className="field" style={{ width: 220 }}>
            <span style={{ display: 'flex', alignItems: 'center', gap: 6 }}>
              <IconUser size={15} />
              <span>Lọc theo Vai Trò</span>
            </span>
            <select value={selectedRoleFilter} onChange={(e) => setSelectedRoleFilter(e.target.value)}>
              <option value="ALL">Tất cả vai trò</option>
              <option value="Student">Sinh viên (Student)</option>
              <option value="Lecturer">Giảng viên (Lecturer)</option>
              <option value="Admin">Quản trị viên (Admin)</option>
            </select>
          </div>
        </div>
      </div>

      {/* Users Card List */}
      <div style={{ display: 'flex', flexDirection: 'column', gap: 16 }}>
        {filteredUsers.map((user) => (
          <div
            key={user.userId}
            className="panel"
            style={{
              display: 'flex',
              flexDirection: 'column',
              gap: 16,
              opacity: user.isActive ? 1 : 0.75,
            }}
          >
            {/* Top User Info Row */}
            <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', flexWrap: 'wrap', gap: 12 }}>
              <div style={{ display: 'flex', alignItems: 'center', gap: 12 }}>
                <div className="avatar-circle" style={{ width: 44, height: 44, fontSize: '1.1rem' }}>
                  {user.fullName.charAt(0)}
                </div>
                <div>
                  <div style={{ display: 'flex', alignItems: 'center', gap: 10, flexWrap: 'wrap' }}>
                    <h3 style={{ fontSize: '1.1rem', fontWeight: 700, color: 'var(--text-primary)' }}>
                      {user.fullName}
                    </h3>
                    <span className="pill-role" style={{ fontSize: '0.75rem', padding: '2px 10px' }}>
                      {user.roleName}
                    </span>
                    {!user.isActive && (
                      <span className="badge badge-neutral" style={{ fontSize: '0.7rem' }}>
                        Đã bị khóa
                      </span>
                    )}
                  </div>

                  <div style={{ fontSize: '0.85rem', color: 'var(--text-secondary)', marginTop: 2, display: 'flex', gap: 12, flexWrap: 'wrap' }}>
                    <span>Tên đăng nhập: <strong style={{ color: 'var(--text-primary)' }}>{user.accountName}</strong></span>
                    {user.emailAddress && <span>• Email: {user.emailAddress}</span>}
                  </div>
                </div>
              </div>
            </div>

            {/* Bottom Actions Bar */}
            <div
              style={{
                paddingTop: 14,
                borderTop: '1px solid var(--border-subtle)',
                display: 'flex',
                alignItems: 'center',
                justifyContent: 'space-between',
                flexWrap: 'wrap',
                gap: 12,
                background: '#f8fafc',
                margin: '-8px -24px -24px -24px',
                padding: '14px 24px',
                borderBottomLeftRadius: 'var(--radius-lg)',
                borderBottomRightRadius: 'var(--radius-lg)',
              }}
            >
              <div style={{ display: 'flex', alignItems: 'center', gap: 10, flexWrap: 'wrap', flex: 1, minWidth: 280 }}>
                <span style={{ fontSize: '0.82rem', fontWeight: 500, color: 'var(--text-secondary)', whiteSpace: 'nowrap' }}>
                  Phân quyền mới:
                </span>
                <select
                  style={{ width: 'auto', minWidth: 160, padding: '6px 34px 6px 12px', fontSize: '0.85rem' }}
                  value={roleSelections[user.userId] ?? ''}
                  onChange={(e) =>
                    setRoleSelections((prev) => ({
                      ...prev,
                      [user.userId]: Number(e.target.value),
                    }))
                  }
                >
                  {roles.map((role) => (
                    <option key={role.roleId} value={role.roleId}>
                      {role.roleName}
                    </option>
                  ))}
                </select>

                <button
                  className="btn btn-secondary"
                  type="button"
                  style={{ fontSize: '0.82rem', padding: '6px 14px' }}
                  onClick={() => changeRole(user.userId)}
                  disabled={busyId === user.userId}
                >
                  Lưu Vai Trò
                </button>
              </div>

              <button
                className="btn btn-secondary"
                type="button"
                style={{ fontSize: '0.82rem', padding: '6px 14px', display: 'inline-flex', alignItems: 'center', gap: 6 }}
                onClick={() => toggleActive(user.userId, user.isActive)}
                disabled={busyId === user.userId}
              >
                {user.isActive ? (
                  <>
                    <IconClose size={15} />
                    <span>Khóa Tài Khoản</span>
                  </>
                ) : (
                  <>
                    <IconCheck size={15} />
                    <span>Mở Khóa Tài Khoản</span>
                  </>
                )}
              </button>
            </div>
          </div>
        ))}

        {filteredUsers.length === 0 && (
          <div className="panel" style={{ textAlign: 'center', padding: 36, color: 'var(--text-muted)' }}>
            Không tìm thấy người dùng nào phù hợp.
          </div>
        )}
      </div>
    </div>
  );
}
