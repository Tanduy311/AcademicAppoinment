import { useEffect, useState } from 'react';
import { Link } from 'react-router-dom';
import { api } from '../services/api';
import type { LecturerListItemDto } from '../types/api';
import {
  IconTeacher,
  IconSearch,
  IconBuilding,
  IconTarget,
  IconMapPin,
  IconClock,
  IconArrowRight,
} from '../components/Icons';

export function LecturersPage() {
  const [items, setItems] = useState<LecturerListItemDto[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const [searchQuery, setSearchQuery] = useState('');
  const [selectedDept, setSelectedDept] = useState('ALL');

  useEffect(() => {
    api
      .lecturers()
      .then(setItems)
      .catch((err) => setError(err instanceof Error ? err.message : 'Không thể tải danh sách giảng viên.'))
      .finally(() => setLoading(false));
  }, []);

  const departments = ['ALL', ...Array.from(new Set(items.map((i) => i.department).filter((d): d is string => Boolean(d))))];

  const filteredItems = items.filter((item) => {
    const matchesSearch =
      item.fullName.toLowerCase().includes(searchQuery.toLowerCase()) ||
      item.lecturerCode.toLowerCase().includes(searchQuery.toLowerCase()) ||
      (item.specialization && item.specialization.toLowerCase().includes(searchQuery.toLowerCase()));

    const matchesDept = selectedDept === 'ALL' || item.department === selectedDept;

    return matchesSearch && matchesDept;
  });

  if (loading) {
    return (
      <div className="panel" style={{ textAlign: 'center', padding: 48 }}>
        <div style={{ fontSize: '1.2rem', color: 'var(--text-secondary)' }}>Đang tải danh sách Giảng Viên...</div>
      </div>
    );
  }

  if (error) {
    return <div className="badge badge-danger" style={{ width: '100%', padding: 16 }}>{error}</div>;
  }

  return (
    <div style={{ display: 'flex', flexDirection: 'column', gap: 24 }}>
      {/* Header Banner & Search Filters */}
      <div className="panel">
        <h1 style={{ fontSize: '1.5rem', fontWeight: 700, marginBottom: 6, display: 'flex', alignItems: 'center', gap: 8 }}>
          <IconTeacher size={24} style={{ color: 'var(--accent)' }} />
          <span>Danh Sách Giảng Viên Tư Vấn</span>
        </h1>
        <p style={{ color: 'var(--text-secondary)', fontSize: '0.9rem', marginBottom: 20 }}>
          Tìm kiếm giảng viên theo tên, khoa hoặc chuyên ngành để đăng ký lịch tư vấn học thuật.
        </p>

        <div style={{ display: 'flex', gap: 16, flexWrap: 'wrap' }}>
          <div className="field" style={{ flex: 1, minWidth: 260 }}>
            <span style={{ display: 'flex', alignItems: 'center', gap: 6 }}>
              <IconSearch size={15} />
              <span>Tìm kiếm Giảng viên</span>
            </span>
            <input
              placeholder="Nhập tên, mã GV (GV001), hoặc chuyên ngành..."
              value={searchQuery}
              onChange={(e) => setSearchQuery(e.target.value)}
            />
          </div>

          <div className="field" style={{ width: 240 }}>
            <span style={{ display: 'flex', alignItems: 'center', gap: 6 }}>
              <IconBuilding size={15} />
              <span>Lọc theo Khoa / Viện</span>
            </span>
            <select value={selectedDept} onChange={(e) => setSelectedDept(e.target.value)}>
              {departments.map((dept) => (
                <option key={dept} value={dept}>
                  {dept === 'ALL' ? 'Tất cả các Khoa' : dept}
                </option>
              ))}
            </select>
          </div>
        </div>
      </div>

      {/* Lecturer Cards Grid */}
      <div className="grid-2">
        {filteredItems.map((item) => (
          <Link
            key={item.lecturerId}
            to={`/app/lecturers/${item.lecturerId}`}
            className="panel panel-interactive"
            style={{ display: 'flex', flexDirection: 'column', justifyContent: 'space-between', gap: 16, textDecoration: 'none' }}
          >
            <div>
              <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'flex-start', marginBottom: 12 }}>
                <div style={{ display: 'flex', alignItems: 'center', gap: 12 }}>
                  <div className="avatar-circle" style={{ width: 44, height: 44, fontSize: '1.1rem' }}>
                    {item.fullName.charAt(0)}
                  </div>
                  <div>
                    <h3 style={{ fontSize: '1.1rem', fontWeight: 700, color: 'var(--text-primary)' }}>{item.fullName}</h3>
                    <div style={{ fontSize: '0.75rem', fontFamily: 'JetBrains Mono', color: 'var(--text-muted)' }}>
                      Mã GV: {item.lecturerCode}
                    </div>
                  </div>
                </div>

                <span className="badge badge-neutral">{item.department || 'Khoa CNTT'}</span>
              </div>

              {item.specialization ? (
                <div style={{ fontSize: '0.85rem', color: 'var(--text-secondary)', marginBottom: 8, display: 'flex', alignItems: 'center', gap: 6 }}>
                  <IconTarget size={15} style={{ color: 'var(--accent)' }} />
                  <span><strong>Chuyên môn:</strong> {item.specialization}</span>
                </div>
              ) : null}

              {item.officeLocation ? (
                <div style={{ fontSize: '0.85rem', color: 'var(--text-muted)', display: 'flex', alignItems: 'center', gap: 6 }}>
                  <IconMapPin size={15} />
                  <span><strong>Văn phòng:</strong> {item.officeLocation}</span>
                </div>
              ) : null}
            </div>

            <div style={{ paddingTop: 12, borderTop: '1px solid var(--border-subtle)', display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
              <span className={`badge ${item.upcomingSlotCount > 0 ? 'badge-success' : 'badge-neutral'}`} style={{ display: 'inline-flex', alignItems: 'center', gap: 4 }}>
                <IconClock size={13} />
                <span>{item.upcomingSlotCount > 0 ? `${item.upcomingSlotCount} slot rảnh sắp tới` : 'Chưa mở slot'}</span>
              </span>

              <span style={{ fontSize: '0.85rem', color: 'var(--accent)', fontWeight: 600, display: 'inline-flex', alignItems: 'center', gap: 4 }}>
                <span>Xem Hồ Sơ & Đặt Lịch</span>
                <IconArrowRight size={14} />
              </span>
            </div>
          </Link>
        ))}

        {filteredItems.length === 0 && (
          <div className="panel bento-col-12" style={{ textAlign: 'center', padding: 36, color: 'var(--text-muted)' }}>
            Không tìm thấy giảng viên nào phù hợp với từ khóa tìm kiếm.
          </div>
        )}
      </div>
    </div>
  );
}
