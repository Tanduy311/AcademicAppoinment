import { FormEvent, useEffect, useState } from 'react';
import { api } from '../services/api';
import { formatDateTime } from '../utils/format';
import type { SlotResponseDto } from '../types/api';
import { IconClock, IconPlus, IconMapPin, IconTrash, IconFileText, IconAlert } from '../components/Icons';

export function LecturerSlotsPage() {
  const [items, setItems] = useState<SlotResponseDto[]>([]);
  const [busy, setBusy] = useState(false);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const [form, setForm] = useState({
    startTime: '',
    endTime: '',
    meetingType: 'Offline',
    locationOrLink: 'Phòng 302-A1',
  });

  const load = async () => {
    setLoading(true);
    try {
      setItems(await api.mySlots());
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Không thể tải danh sách khung giờ.');
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    load();
  }, []);

  const submit = async (event: FormEvent) => {
    event.preventDefault();
    if (!form.startTime || !form.endTime) {
      alert('Vui lòng chọn thời gian bắt đầu và kết thúc.');
      return;
    }

    setBusy(true);
    setError('');
    try {
      await api.createSlot(form);
      await load();
      setForm({
        startTime: '',
        endTime: '',
        meetingType: 'Offline',
        locationOrLink: 'Phòng 302-A1',
      });
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Không thể tạo khung giờ rảnh.');
    } finally {
      setBusy(false);
    }
  };

  const remove = async (id: number) => {
    if (!confirm('Bạn có chắc chắn muốn xóa khung giờ rảnh này?')) return;
    try {
      await api.deleteSlot(id);
      await load();
    } catch (err) {
      alert(err instanceof Error ? err.message : 'Không thể xóa khung giờ.');
    }
  };

  return (
    <div style={{ display: 'flex', flexDirection: 'column', gap: 24 }}>
      <div className="panel">
        <h1 style={{ fontSize: '1.5rem', fontWeight: 700, marginBottom: 6, display: 'flex', alignItems: 'center', gap: 8 }}>
          <IconClock size={24} style={{ color: 'var(--accent)' }} />
          <span>Quản Lý Khung Giờ Rảnh Tư Vấn</span>
        </h1>
        <p style={{ color: 'var(--text-secondary)', fontSize: '0.9rem' }}>
          Tạo mới và quản lý các khung giờ bạn sẵn sàng tư vấn cho sinh viên.
        </p>
      </div>

      {/* Create Slot Form Panel */}
      <form className="panel" onSubmit={submit} style={{ display: 'flex', flexDirection: 'column', gap: 18 }}>
        <h2 style={{ fontSize: '1.15rem', fontWeight: 600, color: 'var(--text-primary)', display: 'flex', alignItems: 'center', gap: 8 }}>
          <IconPlus size={20} style={{ color: 'var(--accent)' }} />
          <span>Tạo Khung Giờ Rảnh Mới</span>
        </h2>

        {error ? (
          <div className="badge badge-danger" style={{ padding: 12, width: '100%', display: 'flex', alignItems: 'center', gap: 6 }}>
            <IconAlert size={16} />
            <span>{error}</span>
          </div>
        ) : null}

        <div className="grid-2">
          <label className="field">
            <span>Thời gian bắt đầu (Start Time)</span>
            <input
              type="datetime-local"
              value={form.startTime}
              onChange={(e) => setForm((prev) => ({ ...prev, startTime: e.target.value }))}
              required
            />
          </label>

          <label className="field">
            <span>Thời gian kết thúc (End Time)</span>
            <input
              type="datetime-local"
              value={form.endTime}
              onChange={(e) => setForm((prev) => ({ ...prev, endTime: e.target.value }))}
              required
            />
          </label>

          <label className="field">
            <span>Hình thức gặp (Meeting Type)</span>
            <select
              value={form.meetingType}
              onChange={(e) => setForm((prev) => ({ ...prev, meetingType: e.target.value }))}
            >
              <option value="Offline">Offline (Tại trường)</option>
              <option value="Online">Online (Google Meet / Zoom)</option>
            </select>
          </label>

          <label className="field">
            <span>Địa điểm hoặc Đường link gặp</span>
            <input
              placeholder="Ví dụ: Phòng 302-A1 hoặc https://meet.google.com/abc..."
              value={form.locationOrLink}
              onChange={(e) => setForm((prev) => ({ ...prev, locationOrLink: e.target.value }))}
            />
          </label>
        </div>

        <div style={{ display: 'flex', justifyContent: 'flex-end' }}>
          <button className="btn btn-primary" type="submit" disabled={busy} style={{ display: 'inline-flex', alignItems: 'center', gap: 6 }}>
            <IconPlus size={16} />
            <span>{busy ? 'Đang Lưu Khung Giờ...' : 'Thêm Slot Rảnh Này'}</span>
          </button>
        </div>
      </form>

      {/* Existing Slots List */}
      <div style={{ display: 'flex', flexDirection: 'column', gap: 16 }}>
        <h3 style={{ fontSize: '1.1rem', fontWeight: 600, display: 'flex', alignItems: 'center', gap: 8 }}>
          <IconFileText size={18} />
          <span>Các Khung Giờ Đã Tạo</span>
        </h3>

        {loading ? (
          <div className="panel" style={{ textAlign: 'center', padding: 36, color: 'var(--text-secondary)' }}>
            Đang tải danh sách slot...
          </div>
        ) : (
          items.map((slot) => (
            <div key={slot.availabilitySlotId} className="panel" style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', flexWrap: 'wrap', gap: 12 }}>
              <div>
                <div style={{ fontSize: '1.05rem', fontWeight: 600, color: 'var(--text-primary)', marginBottom: 4, display: 'flex', alignItems: 'center', gap: 6 }}>
                  <IconClock size={16} style={{ color: 'var(--accent)' }} />
                  <span>{formatDateTime(slot.startTime)} ➔ {formatDateTime(slot.endTime)}</span>
                </div>
                <div style={{ fontSize: '0.85rem', color: 'var(--text-secondary)', display: 'flex', gap: 12, alignItems: 'center' }}>
                  <span className="badge badge-neutral">{slot.meetingType}</span>
                  <span style={{ display: 'flex', alignItems: 'center', gap: 4 }}>
                    <IconMapPin size={14} />
                    <span>{slot.locationOrLink || 'Chưa nhập địa điểm'}</span>
                  </span>
                  <span className={`badge ${slot.isAvailable ? 'badge-success' : 'badge-warning'}`}>
                    {slot.isAvailable ? 'Còn trống' : 'Đã có sinh viên đặt'}
                  </span>
                </div>
              </div>

              <button
                className="btn btn-danger"
                style={{ fontSize: '0.8rem', padding: '6px 12px', display: 'inline-flex', alignItems: 'center', gap: 4 }}
                type="button"
                onClick={() => remove(slot.availabilitySlotId)}
              >
                <IconTrash size={14} />
                <span>Xóa Slot</span>
              </button>
            </div>
          ))
        )}

        {!loading && items.length === 0 && (
          <div className="panel" style={{ textAlign: 'center', padding: 36, color: 'var(--text-muted)' }}>
            Bạn chưa tạo khung giờ rảnh nào.
          </div>
        )}
      </div>
    </div>
  );
}
