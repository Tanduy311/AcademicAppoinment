import { useEffect, useState } from 'react';
import { api } from '../services/api';
import { formatDateTime } from '../utils/format';
import type { NotificationResponseDto } from '../types/api';
import { IconBell, IconCheck, IconClock } from '../components/Icons';

export function NotificationsPage() {
  const [items, setItems] = useState<NotificationResponseDto[]>([]);
  const [unreadCount, setUnreadCount] = useState(0);
  const [loading, setLoading] = useState(true);
  const [busyId, setBusyId] = useState<number | null>(null);

  const load = async () => {
    setLoading(true);
    try {
      const [notifications, unread] = await Promise.all([api.notifications(), api.unreadCount()]);
      setItems(notifications);
      setUnreadCount(unread.unreadCount);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    load();
  }, []);

  const markRead = async (id: number) => {
    setBusyId(id);
    try {
      await api.markNotificationRead(id);
      await load();
    } catch (err) {
      alert(err instanceof Error ? err.message : 'Không thể đánh dấu đã đọc.');
    } finally {
      setBusyId(null);
    }
  };

  if (loading) {
    return (
      <div className="panel" style={{ textAlign: 'center', padding: 48 }}>
        <div style={{ color: 'var(--text-secondary)' }}>Đang tải danh sách thông báo...</div>
      </div>
    );
  }

  return (
    <div style={{ display: 'flex', flexDirection: 'column', gap: 24 }}>
      <div className="panel" style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', flexWrap: 'wrap', gap: 12 }}>
        <div>
          <h1 style={{ fontSize: '1.5rem', fontWeight: 700, marginBottom: 6, display: 'flex', alignItems: 'center', gap: 8 }}>
            <IconBell size={24} style={{ color: 'var(--accent)' }} />
            <span>Trung Tâm Thông Báo</span>
          </h1>
          <p style={{ color: 'var(--text-secondary)', fontSize: '0.9rem' }}>
            Cập nhật các thông báo real-time về trạng thái lịch hẹn tư vấn.
          </p>
        </div>

        <div style={{ display: 'flex', alignItems: 'center', gap: 10 }}>
          <span className="badge badge-neutral">Tổng: {items.length}</span>
          <span className={`badge ${unreadCount > 0 ? 'badge-danger' : 'badge-success'}`}>
            {unreadCount > 0 ? `Chưa đọc: ${unreadCount}` : 'Đã đọc tất cả'}
          </span>
        </div>
      </div>

      <div style={{ display: 'flex', flexDirection: 'column', gap: 14 }}>
        {items.map((item) => (
          <div
            key={item.notificationId}
            className="panel"
            style={{
              display: 'flex',
              flexDirection: 'column',
              gap: 8,
              background: item.isRead ? 'var(--panel-solid)' : '#eff6ff',
              borderColor: item.isRead ? 'var(--border-subtle)' : '#bfdbfe',
            }}
          >
            <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', gap: 12 }}>
              <div style={{ display: 'flex', alignItems: 'center', gap: 10 }}>
                {!item.isRead && <span className="unread-indicator-dot" style={{ position: 'static' }} />}
                <h4 style={{ fontSize: '1.05rem', fontWeight: 600, color: 'var(--text-primary)' }}>{item.title}</h4>
              </div>

              {!item.isRead ? (
                <button
                  type="button"
                  className="btn btn-secondary"
                  style={{ fontSize: '0.78rem', padding: '4px 10px', display: 'inline-flex', alignItems: 'center', gap: 4 }}
                  onClick={() => markRead(item.notificationId)}
                  disabled={busyId === item.notificationId}
                >
                  <IconCheck size={14} />
                  <span>{busyId === item.notificationId ? 'Đang lưu...' : 'Đánh dấu đã đọc'}</span>
                </button>
              ) : (
                <span className="badge badge-neutral" style={{ fontSize: '0.7rem' }}>Đã đọc</span>
              )}
            </div>

            <p style={{ fontSize: '0.9rem', color: 'var(--text-secondary)', margin: 0 }}>{item.message}</p>

            <div style={{ fontSize: '0.75rem', fontFamily: 'JetBrains Mono', color: 'var(--text-muted)', marginTop: 4, display: 'flex', alignItems: 'center', gap: 4 }}>
              <IconClock size={13} />
              <span>{formatDateTime(item.createdAt)}</span>
            </div>
          </div>
        ))}

        {items.length === 0 && (
          <div className="panel" style={{ textAlign: 'center', padding: 36, color: 'var(--text-muted)' }}>
            Bạn chưa có thông báo nào.
          </div>
        )}
      </div>
    </div>
  );
}
