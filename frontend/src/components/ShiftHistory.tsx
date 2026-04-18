import type { ShiftLog } from '../types';

interface Props { history: ShiftLog[]; }

export default function ShiftHistory({ history }: Props) {
  const fmt = (iso?: string) => {
    if (!iso) return '—';
    const parts = iso.split('T');
    return parts.length < 2 ? iso : `${parts[0]} ${parts[1].substring(0, 5)}`;
  };

  const getBadgeStyle = (status: string) => ({
    background: status === 'Open' ? '#d4edda' : status === 'Paused' ? '#cce5ff' : '#ffcdcdff',
    color: status === 'Open' ? '#856404' : status === 'Paused' ? '#004085' : '#155724',
  });

  const getLabel = (status: string) =>
    status === 'Open' ? 'פתוחה' : status === 'Paused' ? 'מושהית' : 'סגורה';

  return (
    <div className="history-container">
      <h2>היסטוריית משמרות ({history.length})</h2>
      {history.length === 0 ? (
        <p style={{ textAlign: 'center', color: '#888' }}>אין משמרות עדיין</p>
      ) : (
        <table className="history-table">
          <thead>
            <tr>
              <th>כניסה</th>
              <th>יציאה</th>
              <th>שעות</th>
              <th>דקות</th>
              <th>סטטוס</th>
            </tr>
          </thead>
          <tbody>
            {history.map(log => (
              <tr key={log.id}>
                <td>{fmt(log.clockInTime)}</td>
                <td>{fmt(log.clockOutTime)}</td>
                <td>{log.durationHours != null ? Math.floor(log.durationHours) : '—'}</td>
                <td>{log.durationHours != null ? Math.round((log.durationHours % 1) * 60) : '—'}</td>
                <td><span className="badge" style={getBadgeStyle(log.status)}>{getLabel(log.status)}</span></td>
              </tr>
            ))}
          </tbody>
        </table>
      )}
    </div>
  );
}