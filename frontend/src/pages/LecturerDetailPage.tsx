import { FormEvent, useEffect, useMemo, useState } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { api } from '../services/api';
import { formatDateTime } from '../utils/format';
import type { LecturerDetailDto } from '../types/api';
import { useAuth } from '../context/AuthContext';
import {
  IconBuilding,
  IconTarget,
  IconMapPin,
  IconChat,
  IconClock,
  IconCheck,
  IconFileText,
  IconAlert,
} from '../components/Icons';

export function LecturerDetailPage() {
  const { lecturerId } = useParams();
  const navigate = useNavigate();
  const { user } = useAuth();
  const [data, setData] = useState<LecturerDetailDto | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const [topic, setTopic] = useState('Tư vấn đồ án học kỳ 2');
  const [description, setDescription] = useState('Em muốn xin thầy tư vấn định hướng đề tài và thuật toán.');
  const [selectedSlotId, setSelectedSlotId] = useState<number | null>(null);
  const [busy, setBusy] = useState(false);
  const [successMsg, setSuccessMsg] = useState('');

  useEffect(() => {
    if (!lecturerId) return;
    api
      .lecturerById(Number(lecturerId))
      .then((res) => {
        setData(res);
        if (res.upcomingSlots && res.upcomingSlots.length > 0) {
          setSelectedSlotId(res.upcomingSlots[0].availabilitySlotId);
        }
      })
      .catch((err) => setError(err instanceof Error ? err.message : 'Không thể tải thông tin giảng viên.'))
      .finally(() => setLoading(false));
  }, [lecturerId]);

  const availableSlots = useMemo(() => data?.upcomingSlots ?? [], [data]);

  const submit = async (event: FormEvent) => {
    event.preventDefault();
    if (!selectedSlotId) {
      alert('Vui lòng chọn 1 khung giờ rảnh.');
      return;
    }
    setBusy(true);
    setError('');
    setSuccessMsg('');
    try {
      await api.createAppointment({
        availabilitySlotId: selectedSlotId,
        topic,
        description,
      });
      setSuccessMsg('Đặt lịch hẹn thành công! Đã gửi yêu cầu đến Giảng viên.');
      await api.lecturerById(Number(lecturerId)).then(setData);
      setTimeout(() => {
        navigate('/app/appointments');
      }, 1500);
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Không thể đặt lịch hẹn.');
    } finally {
      setBusy(false);
    }
  };

  if (loading) {
    return (
      <div className="panel" style={{ textAlign: 'center', padding: 48 }}>
        <div style={{ color: 'var(--text-secondary)' }}>Đang tải hồ sơ Giảng viên...</div>
      </div>
    );
  }

  if (error) return <div className="badge badge-danger" style={{ width: '100%', padding: 16 }}>{error}</div>;
  if (!data) return <div className="panel">Không tìm thấy thông tin giảng viên.</div>;

  const canBook = user?.roleName === 'Student';

  return (
    <div style={{ display: 'flex', flexDirection: 'column', gap: 24 }}>
      {/* Lecturer Profile Header Card */}
      <div className="panel" style={{ display: 'flex', gap: 20, alignItems: 'flex-start', flexWrap: 'wrap' }}>
        <div className="avatar-circle" style={{ width: 64, height: 64, fontSize: '1.75rem', borderRadius: 'var(--radius-lg)' }}>
          {data.fullName.charAt(0)}
        </div>
        <div style={{ flex: 1, minWidth: 260 }}>
          <div style={{ display: 'flex', gap: 12, alignItems: 'center', flexWrap: 'wrap', marginBottom: 4 }}>
            <h1 style={{ fontSize: '1.5rem', fontWeight: 700, color: 'var(--text-primary)' }}>{data.fullName}</h1>
            <span className="badge badge-neutral">Mã GV: {data.lecturerCode}</span>
          </div>

          <div style={{ fontSize: '0.9rem', color: 'var(--accent)', fontWeight: 600, marginBottom: 8, display: 'flex', alignItems: 'center', gap: 6 }}>
            <IconBuilding size={16} />
            <span>{data.department || 'Khoa Công Nghệ Thông Tin'}</span>
          </div>

          {data.specialization ? (
            <div style={{ fontSize: '0.88rem', color: 'var(--text-secondary)', marginBottom: 4, display: 'flex', alignItems: 'center', gap: 6 }}>
              <IconTarget size={16} />
              <span><strong>Chuyên môn:</strong> {data.specialization}</span>
            </div>
          ) : null}

          {data.officeLocation ? (
            <div style={{ fontSize: '0.88rem', color: 'var(--text-muted)', marginBottom: 8, display: 'flex', alignItems: 'center', gap: 6 }}>
              <IconMapPin size={16} />
              <span><strong>Văn phòng làm việc:</strong> {data.officeLocation}</span>
            </div>
          ) : null}

          {data.consultationDescription ? (
            <div style={{ marginTop: 12, padding: 12, background: '#f8fafc', borderRadius: 'var(--radius-md)', border: '1px solid var(--border-subtle)', fontSize: '0.88rem', color: 'var(--text-secondary)', display: 'flex', alignItems: 'flex-start', gap: 6 }}>
              <IconChat size={16} style={{ marginTop: 2, flexShrink: 0 }} />
              <span>{data.consultationDescription}</span>
            </div>
          ) : null}
        </div>
      </div>

      {/* Slots Selection Grid */}
      <div className="panel" style={{ display: 'flex', flexDirection: 'column', gap: 16 }}>
        <h2 style={{ fontSize: '1.2rem', fontWeight: 700, color: 'var(--text-primary)', display: 'flex', alignItems: 'center', gap: 8 }}>
          <IconClock size={20} style={{ color: 'var(--accent)' }} />
          <span>Chọn Khung Giờ Rảnh Tư Vấn ({availableSlots.length})</span>
        </h2>

        <div style={{ display: 'flex', flexDirection: 'column', gap: 10 }}>
          {availableSlots.map((slot) => {
            const isSelected = selectedSlotId === slot.availabilitySlotId;
            return (
              <button
                key={slot.availabilitySlotId}
                type="button"
                className={`select-row ${isSelected ? 'active' : ''}`}
                onClick={() => setSelectedSlotId(slot.availabilitySlotId)}
                disabled={!slot.isAvailable || !canBook}
                style={{
                  padding: 14,
                  cursor: slot.isAvailable && canBook ? 'pointer' : 'not-allowed',
                  opacity: slot.isAvailable ? 1 : 0.6,
                }}
              >
                <div>
                  <div style={{ fontSize: '1rem', fontWeight: 600, color: isSelected ? 'var(--accent)' : 'var(--text-primary)', display: 'flex', alignItems: 'center', gap: 6 }}>
                    <IconClock size={16} />
                    <span>{formatDateTime(slot.startTime)} - {formatDateTime(slot.endTime)}</span>
                  </div>
                  <div style={{ fontSize: '0.82rem', color: 'var(--text-secondary)', marginTop: 2, display: 'flex', alignItems: 'center', gap: 6 }}>
                    <IconMapPin size={14} />
                    <span>Hình thức: <strong style={{ color: 'var(--text-primary)' }}>{slot.meetingType}</strong> ({slot.locationOrLink || 'Chưa có địa điểm'})</span>
                  </div>
                </div>

                <div>
                  {isSelected ? (
                    <span className="badge badge-success" style={{ display: 'inline-flex', alignItems: 'center', gap: 4 }}>
                      <IconCheck size={14} />
                      <span>Đã chọn slot này</span>
                    </span>
                  ) : slot.isAvailable ? (
                    <span className="badge badge-neutral">Bấm chọn</span>
                  ) : (
                    <span className="badge badge-danger">Đã kín lịch</span>
                  )}
                </div>
              </button>
            );
          })}

          {availableSlots.length === 0 && (
            <div style={{ textAlign: 'center', padding: 24, color: 'var(--text-muted)' }}>
              Giảng viên hiện chưa mở khung giờ rảnh nào sắp tới.
            </div>
          )}
        </div>
      </div>

      {/* Booking Form Panel */}
      {canBook ? (
        <form className="panel" onSubmit={submit} style={{ display: 'flex', flexDirection: 'column', gap: 18 }}>
          <h2 style={{ fontSize: '1.2rem', fontWeight: 700, color: 'var(--text-primary)', display: 'flex', alignItems: 'center', gap: 8 }}>
            <IconFileText size={20} style={{ color: 'var(--accent)' }} />
            <span>Đăng Ký Thông Tin Lịch Hẹn</span>
          </h2>

          {successMsg ? (
            <div className="badge badge-success" style={{ padding: 12, width: '100%', textTransform: 'none', display: 'flex', alignItems: 'center', gap: 6 }}>
              <IconCheck size={16} />
              <span>{successMsg}</span>
            </div>
          ) : null}
          {error ? (
            <div className="badge badge-danger" style={{ padding: 12, width: '100%', textTransform: 'none', display: 'flex', alignItems: 'center', gap: 6 }}>
              <IconAlert size={16} />
              <span>{error}</span>
            </div>
          ) : null}

          <label className="field">
            <span>Chủ đề cần tư vấn (Topic)</span>
            <input
              value={topic}
              onChange={(e) => setTopic(e.target.value)}
              placeholder="Nhập chủ đề tư vấn..."
              required
            />
          </label>

          <label className="field">
            <span>Nội dung chi tiết / Câu hỏi gửi Thầy/Cô (Description)</span>
            <textarea
              value={description}
              onChange={(e) => setDescription(e.target.value)}
              rows={4}
              placeholder="Mô tả thắc mắc hoặc thông tin cần tư vấn..."
            />
          </label>

          <div style={{ display: 'flex', justifyContent: 'flex-end' }}>
            <button className="btn btn-primary" type="submit" disabled={busy || !selectedSlotId}>
              {busy ? 'Đang gửi yêu cầu...' : 'Bấm Xác Nhận Đặt Lịch Hẹn'}
            </button>
          </div>
        </form>
      ) : null}
    </div>
  );
}
