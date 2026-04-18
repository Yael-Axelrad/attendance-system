import { useEffect, useState } from 'react';
import type { AttendanceStatus } from '../types';

interface Props {
  status: AttendanceStatus | null;
  loading: boolean;
  error: string | null;
  onClockIn: () => void;
  onClockOut: () => void;
  onPause: () => void;
  onResume: () => void;
}

export default function ClockPanel({ status, loading, error, onClockIn, onClockOut, onPause, onResume }: Props) {
  const [elapsed, setElapsed] = useState(0);

  useEffect(() => {
    if (!status?.isClockedIn) { setElapsed(0); return; }
    setElapsed(status.elapsedSeconds ?? 0);
    if (status.isPaused) return;
    const interval = setInterval(() => setElapsed(prev => prev + 1), 1000);
    return () => clearInterval(interval);
  }, [status]);

  const formatElapsed = (s: number) => {
    const h = Math.floor(s / 3600);
    const m = Math.floor((s % 3600) / 60);
    const sec = s % 60;
    return `${String(h).padStart(2,'0')}:${String(m).padStart(2,'0')}:${String(sec).padStart(2,'0')}`;
  };

  const formatTime = (iso?: string) => {
    if (!iso) return '—';
    const parts = iso.split('T');
    return parts.length < 2 ? iso : parts[1].substring(0, 5);
  };

  if (!status) return <div className="loading-page">טוען...</div>;

  const badgeBg = !status.isClockedIn ? '#f8d7da' : status.isPaused ? '#cdf8ffff' : '#d4edda';
  const badgeColor = !status.isClockedIn ? '#721c24' : status.isPaused ? '#856404' : '#155724';

  return (
    <div className="clock-panel">
      <div className="status-badge" style={{ background: badgeBg, color: badgeColor }}>
        {!status.isClockedIn ? '🔴 מחוץ למשמרת' : status.isPaused ? '⏸️ מושהה' : '🟢 במשמרת'}
      </div>

      {status.isClockedIn && (
        <>
          <p className="clock-time">כניסה: <strong>{formatTime(status.clockInTime)}</strong> (שעון ציריך)</p>
          <div className="timer">{formatElapsed(elapsed)}</div>
          {status.isPaused}
        </>
      )}

      {error && <p className="error-text">⚠️ {error}</p>}

      <div className="clock-buttons">
        {!status.isClockedIn && (
          <button className="btn-clock" style={{ background: '#28a745' }} onClick={onClockIn} disabled={loading}>
            🟢 התחלת משמרת
          </button>
        )}
        {status.isClockedIn && !status.isPaused && (
          <>
            <button className="btn-clock" style={{ background: '#93b7ffff', color: '#333' }} onClick={onPause} disabled={loading}>⏸️ השהה</button>
            <button className="btn-clock" style={{ background: '#d86470ff' }} onClick={onClockOut} disabled={loading}>🔴 סגירת משמרת</button>
          </>
        )}
        {status.isClockedIn && status.isPaused && (
          <>
            <button className="btn-clock" style={{ background: '#52d38eff' }} onClick={onResume} disabled={loading}> 🟢 המשך</button>
            <button className="btn-clock" style={{ background: '#ea5463ff' }} onClick={onClockOut} disabled={loading}>🔴 סגירת משמרת</button>
          </>
        )}
      </div>

      {loading && <p className="loading-text">מעבד...</p>}
    </div>
  );
}