export function formatDateTime(value: string | Date) {
  const date = typeof value === 'string' ? new Date(value) : value;
  return new Intl.DateTimeFormat('vi-VN', {
    dateStyle: 'short',
    timeStyle: 'short',
  }).format(date);
}

export function titleCase(value: string) {
  return value
    .split(' ')
    .filter(Boolean)
    .map((part) => part.charAt(0).toUpperCase() + part.slice(1).toLowerCase())
    .join(' ');
}

export function getStatusTone(status: string) {
  const normalized = status.toLowerCase();
  if (['approved', 'confirmed', 'completed'].includes(normalized)) return 'success';
  if (['pending', 'waiting'].includes(normalized)) return 'warning';
  if (['rejected', 'cancelled', 'canceled'].includes(normalized)) return 'danger';
  return 'neutral';
}
