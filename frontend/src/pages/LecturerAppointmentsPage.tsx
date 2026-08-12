import { useEffect, useState } from 'react';
import { api } from '../services/api';
import { formatDateTime } from '../utils/format';
import type { AppointmentResponseDto } from '../types/api';
import { StatusBadge } from '../components/StatusBadge';
import {
  IconFileText,
  IconBookmark,
  IconUser,
  IconCalendar,
  IconMapPin,
  IconChat,
  IconCheck,
  IconClose,
  IconHistory,
} from '../components/Icons';

export function LecturerAppointmentsPage() {
  const [items, setItems] = useState<AppointmentResponseDto[]>([]);
  const [loading, setLoading] = useState(true);
  const [activeTab, setActiveTab] = useState<'current' | 'history'>('current');
  const [busyId, setBusyId] = useState<number | null>(null);
  const [activeItem, setActiveItem] = useState<AppointmentResponseDto | null>(null);
  const [actionType, setActionType] = useState<'Approved' | 'Rejected' | 'Cancelled'>('Approved');
  const [responseMsg, setResponseMsg] = useState('Thầy đồng ý phê duyệt lịch hẹn, em đến đúng giờ nhé.');

  const load = async () => {
    setLoading(true);
    try {
      setItems(await api.lecturerAppointments());
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    load();
  }, []);

  // Categorize into Current vs History
  const currentItems = items.filter((item) => {
    const isPast = new Date(item.endTime).getTime() < Date.now();
    return (item.status === 'Pending' || item.status === 'Confirmed' || item.status === 'Approved') && !isPast;
  });

  const historyItems = items.filter((item) => {
    const isPast = new Date(item.endTime).getTime() < Date.now();
    return item.status === 'Cancelled' || item.status === 'Rejected' || isPast;
  });

  const displayItems = activeTab === 'current' ? currentItems : historyItems;

  const openDecisionModal = (item: AppointmentResponseDto, status: 'Approved' | 'Rejected') => {
    setActiveItem(item);
    setActionType(status);
    setResponseMsg(status === 'Approved' ? 'Thầy đồng ý phê duyệt lịch hẹn, em đến đúng giờ nhé.' : 'Thầy bận lịch họp đột xuất, em đăng ký lại khung giờ khác giúp thầy nhé.');
  };

  const openCancelModal = (item: AppointmentResponseDto) => {
    setActiveItem(item);
    setActionType('Cancelled');
    setResponseMsg('Thầy bận lịch đột xuất nên xin phép hủy lịch hẹn này, em thông cảm nhé.');
  };

  const handleDecisionSubmit = async () => {
    if (!activeItem) return;
    setBusyId(activeItem.appointmentId);
    try {
      await api.updateAppointmentStatus(activeItem.appointmentId, {
        status: actionType === 'Approved' ? 'Confirmed' : actionType === 'Cancelled' ? 'Cancelled' : 'Rejected',
        lecturerResponse: responseMsg,
      });
      setActiveItem(null);
      await load();
    } catch (err) {
      alert(err instanceof Error ? err.message : 'Không thể cập nhật trạng thái lịch hẹn.');
    } finally {
      setBusyId(null);
    }
  };

  if (loading) {
    return (
      <div className="panel" style={{ textAlign: 'center', padding: 48 }}>
        <div style={{ color: 'var(--text-secondary)' }}>Đang tải hàng đợi lịch hẹn...</div>
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
            <span>Hàng Đợi Duyệt Lịch Hẹn</span>
          </h1>
          <p style={{ color: 'var(--text-secondary)', fontSize: '0.88rem' }}>
            Quản lý các yêu cầu tư vấn học thuật do sinh viên gửi đến cho bạn.
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
            <span>Yêu Cầu Cần Duyệt & Hiện Tại</span>
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
            <span>Lịch Sử & Đã Hủy</span>
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
                  <IconUser size={16} />
                  <span>Sinh viên đăng ký: <strong>{item.studentName}</strong> (Mã SV: {item.studentCode || 'SV001'})</span>
                </div>
              </div>

              <StatusBadge value={item.status} />
            </div>

            <div style={{ display: 'flex', gap: 20, flexWrap: 'wrap', background: '#f8fafc', padding: 12, borderRadius: 'var(--radius-md)', border: '1px solid var(--border-subtle)', fontSize: '0.85rem' }}>
              <div style={{ display: 'flex', alignItems: 'center', gap: 6 }}>
                <IconCalendar size={16} style={{ color: 'var(--text-muted)' }} />
                <span><strong>Thời gian tư vấn:</strong> {formatDateTime(item.startTime)} - {formatDateTime(item.endTime)}</span>
              </div>
              <div style={{ display: 'flex', alignItems: 'center', gap: 6 }}>
                <IconMapPin size={16} style={{ color: 'var(--text-muted)' }} />
                <span><strong>Hình thức:</strong> <span className="badge badge-neutral">{item.meetingType}</span> ({item.locationOrLink || 'Chưa cập nhật'})</span>
              </div>
            </div>

            {item.description ? (
              <div style={{ fontSize: '0.85rem', color: 'var(--text-muted)', display: 'flex', alignItems: 'flex-start', gap: 6 }}>
                <IconChat size={16} style={{ marginTop: 2, flexShrink: 0 }} />
                <span><strong>Nội dung câu hỏi sinh viên:</strong> {item.description}</span>
              </div>
            ) : null}

            {item.lecturerResponse ? (
              <div style={{ background: '#eff6ff', border: '1px solid #bfdbfe', padding: 12, borderRadius: 'var(--radius-md)', fontSize: '0.85rem', color: '#1d4ed8', display: 'flex', alignItems: 'flex-start', gap: 6 }}>
                <IconChat size={16} style={{ marginTop: 2, flexShrink: 0 }} />
                <span><strong>Lời nhắn của bạn:</strong> {item.lecturerResponse}</span>
              </div>
            ) : null}

            {item.cancellationReason ? (
              <div style={{ background: '#fef2f2', border: '1px solid #fecaca', padding: 12, borderRadius: 'var(--radius-md)', fontSize: '0.85rem', color: '#dc2626', display: 'flex', alignItems: 'flex-start', gap: 6 }}>
                <IconChat size={16} style={{ marginTop: 2, flexShrink: 0 }} />
                <span><strong>Lý do sinh viên hủy lịch:</strong> {item.cancellationReason}</span>
              </div>
            ) : null}

            {item.status === 'Pending' ? (
              <div style={{ paddingTop: 8, display: 'flex', gap: 12, justifyContent: 'flex-end' }}>
                <button
                  type="button"
                  className="btn btn-danger"
                  style={{ fontSize: '0.82rem', padding: '6px 14px', display: 'inline-flex', alignItems: 'center', gap: 6 }}
                  onClick={() => openDecisionModal(item, 'Rejected')}
                  disabled={busyId === item.appointmentId}
                >
                  <IconClose size={15} />
                  <span>Từ Chối</span>
                </button>
                <button
                  type="button"
                  className="btn btn-success"
                  style={{ fontSize: '0.82rem', padding: '6px 16px', display: 'inline-flex', alignItems: 'center', gap: 6 }}
                  onClick={() => openDecisionModal(item, 'Approved')}
                  disabled={busyId === item.appointmentId}
                >
                  <IconCheck size={15} />
                  <span>Phê Duyệt Lịch Hẹn</span>
                </button>
              </div>
            ) : (item.status === 'Confirmed' || item.status === 'Approved') && activeTab === 'current' ? (
              <div style={{ paddingTop: 8, display: 'flex', gap: 12, justifyContent: 'flex-end' }}>
                <button
                  type="button"
                  className="btn btn-danger"
                  style={{ fontSize: '0.82rem', padding: '6px 14px', display: 'inline-flex', alignItems: 'center', gap: 6 }}
                  onClick={() => openCancelModal(item)}
                  disabled={busyId === item.appointmentId}
                >
                  <IconClose size={15} />
                  <span>Hủy Lịch Hẹn Này</span>
                </button>
              </div>
            ) : null}
          </div>
        ))}

        {displayItems.length === 0 && (
          <div className="panel" style={{ textAlign: 'center', padding: 48, color: 'var(--text-muted)' }}>
            {activeTab === 'current' ? (
              <div>
                <div style={{ fontSize: '1.1rem', fontWeight: 600, color: 'var(--text-primary)', marginBottom: 4 }}>Không có yêu cầu tư vấn nào mới</div>
                <div style={{ fontSize: '0.85rem' }}>Hiện tại không có lịch hẹn mới nào cần bạn xử lý phê duyệt.</div>
              </div>
            ) : (
              <div>
                <div style={{ fontSize: '1.1rem', fontWeight: 600, color: 'var(--text-primary)', marginBottom: 4 }}>Chưa có lịch sử yêu cầu cũ</div>
                <div style={{ fontSize: '0.85rem' }}>Các cuộc hẹn đã hoàn thành hoặc bị từ chối/hủy sẽ lưu tại đây.</div>
              </div>
            )}
          </div>
        )}
      </div>

      {/* Decision Response Modal Overlay */}
      {activeItem && (
        <div className="modal-overlay">
          <div className="modal-content">
            <h2 style={{ fontSize: '1.25rem', fontWeight: 700, marginBottom: 12, display: 'flex', alignItems: 'center', gap: 8 }}>
              {actionType === 'Approved' ? (
                <>
                  <IconCheck size={20} style={{ color: 'var(--success)' }} />
                  <span>Phê Duyệt Lịch Hẹn</span>
                </>
              ) : actionType === 'Cancelled' ? (
                <>
                  <IconClose size={20} style={{ color: '#dc2626' }} />
                  <span>Hủy Lịch Hẹn Đã Duyệt</span>
                </>
              ) : (
                <>
                  <IconClose size={20} style={{ color: 'var(--danger)' }} />
                  <span>Từ Chối Lịch Hẹn</span>
                </>
              )}
            </h2>
            <p style={{ fontSize: '0.88rem', color: 'var(--text-secondary)', marginBottom: 16 }}>
              Lịch tư vấn: <strong>"{activeItem.topic}"</strong> của SV {activeItem.studentName}.
            </p>

            <label className="field" style={{ marginBottom: 20 }}>
              <span>{actionType === 'Cancelled' ? 'Lý do hủy lịch gửi tới Sinh viên:' : 'Lời nhắn phản hồi gửi tới Sinh viên:'}</span>
              <textarea
                rows={3}
                value={responseMsg}
                onChange={(e) => setResponseMsg(e.target.value)}
                placeholder="Nhập ghi chú phản hồi..."
              />
            </label>

            <div style={{ display: 'flex', gap: 12, justifyContent: 'flex-end' }}>
              <button type="button" className="btn btn-secondary" onClick={() => setActiveItem(null)}>
                Hủy Bỏ
              </button>
              <button
                type="button"
                className={actionType === 'Approved' ? 'btn btn-success' : 'btn btn-danger'}
                onClick={handleDecisionSubmit}
                disabled={busyId === activeItem.appointmentId}
              >
                {busyId === activeItem.appointmentId ? 'Đang lưu...' : actionType === 'Approved' ? 'Xác Nhận Duyệt' : actionType === 'Cancelled' ? 'Xác Nhận Hủy' : 'Xác Nhận Từ Chối'}
              </button>
            </div>
          </div>
        </div>
      )}
    </div>
  );
}
