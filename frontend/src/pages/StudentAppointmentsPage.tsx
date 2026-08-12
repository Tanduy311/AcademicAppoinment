import { useEffect, useState } from 'react';
import { api } from '../services/api';
import { formatDateTime } from '../utils/format';
import type { AppointmentResponseDto } from '../types/api';
import { StatusBadge } from '../components/StatusBadge';
import {
  IconBookmark,
  IconTeacher,
  IconCalendar,
  IconMapPin,
  IconChat,
  IconClose,
  IconFileText,
  IconArrowRight,
  IconAlert,
  IconCheck,
  IconHistory,
} from '../components/Icons';

export function StudentAppointmentsPage() {
  const [items, setItems] = useState<AppointmentResponseDto[]>([]);
  const [loading, setLoading] = useState(true);
  const [activeTab, setActiveTab] = useState<'current' | 'history'>('current');
  const [detailLoadingId, setDetailLoadingId] = useState<number | null>(null);
  
  // Separate Modal States: 'view' | 'cancel' | null
  const [activeModal, setActiveModal] = useState<{
    item: AppointmentResponseDto;
    mode: 'view' | 'cancel';
  } | null>(null);

  const [cancelBusyId, setCancelBusyId] = useState<number | null>(null);
  const [reason, setReason] = useState('Em bận đột xuất lịch học trên lớp.');

  const load = async () => {
    setLoading(true);
    try {
      setItems(await api.myAppointments());
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    load();
  }, []);

  const canCancel = (status: string) => ['Pending', 'Confirmed'].includes(status);

  // Categorize appointments into Current vs History
  const currentItems = items.filter((item) => {
    const isPast = new Date(item.endTime).getTime() < Date.now();
    return (item.status === 'Pending' || item.status === 'Confirmed' || item.status === 'Approved') && !isPast;
  });

  const historyItems = items.filter((item) => {
    const isPast = new Date(item.endTime).getTime() < Date.now();
    return item.status === 'Cancelled' || item.status === 'Rejected' || isPast;
  });

  const displayItems = activeTab === 'current' ? currentItems : historyItems;

  const openViewModal = async (item: AppointmentResponseDto) => {
    setDetailLoadingId(item.appointmentId);
    try {
      const detail = await api.appointmentsById(item.appointmentId);
      setActiveModal({ item: detail, mode: 'view' });
    } catch (err) {
      alert(err instanceof Error ? err.message : 'Không thể tải chi tiết lịch hẹn.');
    } finally {
      setDetailLoadingId(null);
    }
  };

  const openCancelModal = (item: AppointmentResponseDto) => {
    setReason('Em bận đột xuất lịch học trên lớp.');
    setActiveModal({ item, mode: 'cancel' });
  };

  const handleCancelSubmit = async () => {
    if (!activeModal?.item) return;
    setCancelBusyId(activeModal.item.appointmentId);
    try {
      await api.cancelAppointment(activeModal.item.appointmentId, { cancellationReason: reason });
      setActiveModal(null);
      await load();
    } catch (err) {
      alert(err instanceof Error ? err.message : 'Không thể hủy lịch hẹn.');
    } finally {
      setCancelBusyId(null);
    }
  };

  if (loading) {
    return (
      <div className="panel" style={{ textAlign: 'center', padding: 48 }}>
        <div style={{ color: 'var(--text-secondary)' }}>Đang tải danh sách lịch hẹn...</div>
      </div>
    );
  }

  return (
    <div style={{ display: 'flex', flexDirection: 'column', gap: 20 }}>
      {/* Header & Tabs */}
      <div className="panel" style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', flexWrap: 'wrap', gap: 16 }}>
        <div>
          <h1 style={{ fontSize: '1.5rem', fontWeight: 700, marginBottom: 4, display: 'flex', alignItems: 'center', gap: 8 }}>
            <IconFileText size={24} style={{ color: 'var(--accent)' }} />
            <span>Lịch Hẹn Của Tôi</span>
          </h1>
          <p style={{ color: 'var(--text-secondary)', fontSize: '0.88rem' }}>
            Theo dõi và quản lý các buổi tư vấn học thuật bạn đã đăng ký với giảng viên.
          </p>
        </div>

        {/* Tab Navigation */}
        <div style={{ display: 'inline-flex', background: '#f1f5f9', padding: 4, borderRadius: 'var(--radius-md)', gap: 4 }}>
          <button
            type="button"
            className="btn"
            style={{
              fontSize: '0.88rem',
              padding: '6px 14px',
              borderRadius: 'var(--radius-sm)',
              border: 'none',
              background: activeTab === 'current' ? '#fff' : 'transparent',
              color: activeTab === 'current' ? 'var(--accent)' : 'var(--text-secondary)',
              fontWeight: activeTab === 'current' ? 700 : 500,
              boxShadow: activeTab === 'current' ? '0 1px 3px rgba(0,0,0,0.1)' : 'none',
              transition: 'all 0.15s ease',
              display: 'flex',
              alignItems: 'center',
              gap: 8,
            }}
            onClick={() => setActiveTab('current')}
          >
            <IconCalendar size={16} style={{ color: activeTab === 'current' ? 'var(--accent)' : 'var(--text-muted)' }} />
            <span>Cuộc Hẹn Hiện Tại</span>
            <span
              style={{
                background: activeTab === 'current' ? 'var(--accent)' : '#cbd5e1',
                color: '#fff',
                borderRadius: 12,
                padding: '2px 8px',
                fontSize: '0.75rem',
                fontWeight: 700,
              }}
            >
              {currentItems.length}
            </span>
          </button>

          <button
            type="button"
            className="btn"
            style={{
              fontSize: '0.88rem',
              padding: '6px 14px',
              borderRadius: 'var(--radius-sm)',
              border: 'none',
              background: activeTab === 'history' ? '#fff' : 'transparent',
              color: activeTab === 'history' ? 'var(--text-primary)' : 'var(--text-secondary)',
              fontWeight: activeTab === 'history' ? 700 : 500,
              boxShadow: activeTab === 'history' ? '0 1px 3px rgba(0,0,0,0.1)' : 'none',
              transition: 'all 0.15s ease',
              display: 'flex',
              alignItems: 'center',
              gap: 8,
            }}
            onClick={() => setActiveTab('history')}
          >
            <IconHistory size={16} style={{ color: activeTab === 'history' ? 'var(--text-primary)' : 'var(--text-muted)' }} />
            <span>Lịch Sử Cuộc Hẹn</span>
            <span
              style={{
                background: activeTab === 'history' ? '#64748b' : '#cbd5e1',
                color: '#fff',
                borderRadius: 12,
                padding: '2px 8px',
                fontSize: '0.75rem',
                fontWeight: 700,
              }}
            >
              {historyItems.length}
            </span>
          </button>
        </div>
      </div>

      {/* Appointment Cards List */}
      <div style={{ display: 'flex', flexDirection: 'column', gap: 16 }}>
        {displayItems.map((item) => (
          <div key={item.appointmentId} className="panel" style={{ display: 'flex', flexDirection: 'column', gap: 12 }}>
            <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'flex-start', flexWrap: 'wrap', gap: 12 }}>
              <div>
                <h3 style={{ fontSize: '1.1rem', fontWeight: 700, color: 'var(--text-primary)', marginBottom: 4, display: 'flex', alignItems: 'center', gap: 6 }}>
                  <IconBookmark size={18} style={{ color: 'var(--accent)' }} />
                  <span>{item.topic}</span>
                </h3>
                <div style={{ fontSize: '0.85rem', color: 'var(--text-secondary)', display: 'flex', alignItems: 'center', gap: 6, marginTop: 4 }}>
                  <IconTeacher size={16} />
                  <span>Giảng viên: <strong>{item.lecturerName}</strong> ({item.department || 'Khoa CNTT'})</span>
                </div>
              </div>

              <StatusBadge value={item.status} />
            </div>

            <div style={{ display: 'flex', gap: 20, flexWrap: 'wrap', background: '#f8fafc', padding: 12, borderRadius: 'var(--radius-md)', border: '1px solid var(--border-subtle)', fontSize: '0.85rem' }}>
              <div style={{ display: 'flex', alignItems: 'center', gap: 6 }}>
                <IconCalendar size={16} style={{ color: 'var(--text-muted)' }} />
                <span><strong>Thời gian:</strong> {formatDateTime(item.startTime)} - {formatDateTime(item.endTime)}</span>
              </div>
              <div style={{ display: 'flex', alignItems: 'center', gap: 6 }}>
                <IconMapPin size={16} style={{ color: 'var(--text-muted)' }} />
                <span><strong>Hình thức:</strong> <span className="badge badge-neutral">{item.meetingType}</span> ({item.locationOrLink || 'Chưa cập nhật'})</span>
              </div>
            </div>

            {item.description ? (
              <div style={{ fontSize: '0.85rem', color: 'var(--text-muted)', display: 'flex', alignItems: 'flex-start', gap: 6 }}>
                <IconChat size={16} style={{ marginTop: 2, flexShrink: 0 }} />
                <span><strong>Ghi chú sinh viên:</strong> {item.description}</span>
              </div>
            ) : null}

            {item.lecturerResponse ? (
              <div style={{ background: '#f0fdf4', border: '1px solid #bbf7d0', padding: 12, borderRadius: 'var(--radius-md)', fontSize: '0.85rem', color: '#16a34a', display: 'flex', alignItems: 'flex-start', gap: 6 }}>
                <IconChat size={16} style={{ marginTop: 2, flexShrink: 0 }} />
                <span><strong>Phản hồi từ Giảng viên:</strong> {item.lecturerResponse}</span>
              </div>
            ) : null}

            <div style={{ paddingTop: 8, display: 'flex', justifyContent: 'flex-end', gap: 10, flexWrap: 'wrap' }}>
              <button
                type="button"
                className="btn btn-secondary"
                style={{ fontSize: '0.82rem', padding: '6px 14px', display: 'inline-flex', alignItems: 'center', gap: 6 }}
                onClick={() => void openViewModal(item)}
                disabled={detailLoadingId === item.appointmentId}
              >
                <IconArrowRight size={15} />
                <span>{detailLoadingId === item.appointmentId ? 'Đang mở...' : 'Xem Chi Tiết'}</span>
              </button>
              {canCancel(item.status) && activeTab === 'current' ? (
                <button
                  type="button"
                  className="btn btn-danger"
                  style={{ fontSize: '0.82rem', padding: '6px 14px', display: 'inline-flex', alignItems: 'center', gap: 6 }}
                  onClick={() => openCancelModal(item)}
                  disabled={cancelBusyId === item.appointmentId}
                >
                  <IconClose size={15} />
                  <span>Hủy Lịch Hẹn</span>
                </button>
              ) : null}
            </div>
          </div>
        ))}

        {displayItems.length === 0 && (
          <div className="panel" style={{ textAlign: 'center', padding: 48, color: 'var(--text-muted)' }}>
            {activeTab === 'current' ? (
              <div>
                <div style={{ fontSize: '1.1rem', fontWeight: 600, color: 'var(--text-primary)', marginBottom: 4 }}>Không có cuộc hẹn nào sắp tới</div>
                <div style={{ fontSize: '0.85rem' }}>Bạn chưa có lịch hẹn tư vấn nào đang hoạt động. Vui lòng chọn Giảng viên để đặt lịch mới.</div>
              </div>
            ) : (
              <div>
                <div style={{ fontSize: '1.1rem', fontWeight: 600, color: 'var(--text-primary)', marginBottom: 4 }}>Chưa có lịch sử cuộc hẹn cũ</div>
                <div style={{ fontSize: '0.85rem' }}>Các buổi tư vấn đã kết thúc hoặc bị hủy sẽ xuất hiện tại đây.</div>
              </div>
            )}
          </div>
        )}
      </div>

      {/* MODAL 1: VIEW DETAILS ONLY */}
      {activeModal && activeModal.mode === 'view' && (
        <div className="modal-overlay">
          <div className="modal-content" style={{ maxWidth: 540 }}>
            <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: 16, paddingBottom: 12, borderBottom: '1px solid var(--border-subtle)' }}>
              <h2 style={{ fontSize: '1.25rem', fontWeight: 700, display: 'flex', alignItems: 'center', gap: 8, color: 'var(--text-primary)' }}>
                <IconFileText size={22} style={{ color: 'var(--accent)' }} />
                <span>Chi Tiết Buổi Tư Vấn</span>
              </h2>
              <StatusBadge value={activeModal.item.status} />
            </div>

            <div style={{ display: 'flex', flexDirection: 'column', gap: 16 }}>
              <div>
                <div style={{ fontSize: '1.15rem', fontWeight: 700, color: 'var(--text-primary)', marginBottom: 4 }}>
                  {activeModal.item.topic}
                </div>
                <div style={{ fontSize: '0.88rem', color: 'var(--text-secondary)', display: 'flex', alignItems: 'center', gap: 6 }}>
                  <IconTeacher size={16} />
                  <span>Giảng viên: <strong style={{ color: 'var(--text-primary)' }}>{activeModal.item.lecturerName}</strong> ({activeModal.item.department || 'Khoa CNTT'})</span>
                </div>
              </div>

              {/* Grid Information Card */}
              <div style={{ background: '#f8fafc', padding: 16, borderRadius: 'var(--radius-md)', border: '1px solid var(--border-subtle)', display: 'flex', flexDirection: 'column', gap: 12, fontSize: '0.88rem' }}>
                <div style={{ display: 'flex', alignItems: 'center', gap: 8 }}>
                  <IconCalendar size={16} style={{ color: 'var(--accent)' }} />
                  <span><strong>Thời gian:</strong> {formatDateTime(activeModal.item.startTime)} - {formatDateTime(activeModal.item.endTime)}</span>
                </div>

                <div style={{ display: 'flex', alignItems: 'center', gap: 8 }}>
                  <IconMapPin size={16} style={{ color: 'var(--accent)' }} />
                  <span><strong>Hình thức:</strong> <span className="badge badge-neutral">{activeModal.item.meetingType}</span> {activeModal.item.locationOrLink ? `(${activeModal.item.locationOrLink})` : ''}</span>
                </div>
              </div>

              {/* Notes */}
              {activeModal.item.description && (
                <div style={{ background: '#fff', padding: 14, borderRadius: 'var(--radius-md)', border: '1px solid var(--border-subtle)' }}>
                  <div style={{ fontSize: '0.8rem', fontWeight: 600, color: 'var(--text-muted)', marginBottom: 4, display: 'flex', alignItems: 'center', gap: 6 }}>
                    <IconChat size={14} />
                    <span>Ghi chú yêu cầu của Sinh viên</span>
                  </div>
                  <div style={{ fontSize: '0.88rem', color: 'var(--text-primary)' }}>{activeModal.item.description}</div>
                </div>
              )}

              {/* Lecturer Response */}
              {activeModal.item.lecturerResponse && (
                <div style={{ background: '#eff6ff', border: '1px solid #bfdbfe', padding: 14, borderRadius: 'var(--radius-md)', fontSize: '0.88rem', color: '#1d4ed8' }}>
                  <div style={{ fontWeight: 600, marginBottom: 4, display: 'flex', alignItems: 'center', gap: 6 }}>
                    <IconCheck size={16} />
                    <span>Lời nhắn phản hồi từ Giảng viên:</span>
                  </div>
                  <div>{activeModal.item.lecturerResponse}</div>
                </div>
              )}

              {/* Cancellation Reason */}
              {activeModal.item.cancellationReason && (
                <div style={{ background: '#fef2f2', border: '1px solid #fecaca', padding: 14, borderRadius: 'var(--radius-md)', fontSize: '0.88rem', color: '#dc2626' }}>
                  <div style={{ fontWeight: 600, marginBottom: 4, display: 'flex', alignItems: 'center', gap: 6 }}>
                    <IconAlert size={16} />
                    <span>Lý do đã hủy lịch:</span>
                  </div>
                  <div>{activeModal.item.cancellationReason}</div>
                </div>
              )}

              {/* Modal Footer Actions */}
              <div style={{ display: 'flex', gap: 12, justifyContent: 'space-between', alignItems: 'center', marginTop: 8, paddingTop: 14, borderTop: '1px solid var(--border-subtle)' }}>
                {canCancel(activeModal.item.status) && activeTab === 'current' ? (
                  <button
                    type="button"
                    className="btn btn-secondary"
                    style={{ color: '#dc2626', fontSize: '0.82rem' }}
                    onClick={() => openCancelModal(activeModal.item)}
                  >
                    Chuyển sang Hủy Lịch
                  </button>
                ) : <div />}

                <button type="button" className="btn btn-primary" onClick={() => setActiveModal(null)}>
                  Đóng
                </button>
              </div>
            </div>
          </div>
        </div>
      )}

      {/* MODAL 2: CANCEL APPOINTMENT ONLY */}
      {activeModal && activeModal.mode === 'cancel' && (
        <div className="modal-overlay">
          <div className="modal-content" style={{ maxWidth: 500 }}>
            <h2 style={{ fontSize: '1.25rem', fontWeight: 700, marginBottom: 12, display: 'flex', alignItems: 'center', gap: 8, color: '#dc2626' }}>
              <IconAlert size={22} />
              <span>Hủy Lịch Hẹn Tư Vấn</span>
            </h2>

            <div style={{ background: '#fef2f2', padding: 12, borderRadius: 'var(--radius-md)', border: '1px solid #fecaca', marginBottom: 16, fontSize: '0.85rem', color: '#991b1b' }}>
              Bạn đang yêu cầu hủy lịch hẹn <strong>"{activeModal.item.topic}"</strong> với Giảng viên <strong>{activeModal.item.lecturerName}</strong>.
            </div>

            {/* Quick Reasons Selection */}
            <div style={{ marginBottom: 14 }}>
              <div style={{ fontSize: '0.78rem', color: 'var(--text-muted)', marginBottom: 6 }}>Chọn nhanh lý do:</div>
              <div style={{ display: 'flex', gap: 6, flexWrap: 'wrap' }}>
                {[
                  'Em bận đột xuất lịch học trên lớp.',
                  'Trùng lịch thi đột xuất.',
                  'Đã tự giải quyết được thắc mắc.',
                ].map((qReason) => (
                  <button
                    key={qReason}
                    type="button"
                    className="btn btn-secondary"
                    style={{ fontSize: '0.75rem', padding: '4px 8px' }}
                    onClick={() => setReason(qReason)}
                  >
                    {qReason}
                  </button>
                ))}
              </div>
            </div>

            <label className="field" style={{ marginBottom: 20 }}>
              <span>Vui lòng nhập chi tiết lý do hủy lịch:</span>
              <textarea
                rows={3}
                value={reason}
                onChange={(e) => setReason(e.target.value)}
                placeholder="Nhập lý do gửi tới Giảng viên..."
                required
              />
            </label>

            <div style={{ display: 'flex', gap: 12, justifyContent: 'flex-end' }}>
              <button type="button" className="btn btn-secondary" onClick={() => setActiveModal(null)}>
                Quay Lại
              </button>
              <button
                type="button"
                className="btn btn-danger"
                onClick={handleCancelSubmit}
                disabled={cancelBusyId === activeModal.item.appointmentId}
              >
                {cancelBusyId === activeModal.item.appointmentId ? 'Đang Hủy...' : 'Xác Nhận Hủy Lịch'}
              </button>
            </div>
          </div>
        </div>
      )}
    </div>
  );
}
