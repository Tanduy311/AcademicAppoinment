import { getStatusTone, titleCase } from '../utils/format';

export function StatusBadge({ value }: { value: string }) {
  const tone = getStatusTone(value);
  return <span className={`badge badge-${tone}`}>{titleCase(value)}</span>;
}
