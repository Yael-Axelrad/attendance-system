import { useState, useEffect, useCallback } from 'react';
import LoginForm from './components/LoginForm';
import ClockPanel from './components/ClockPanel';
import ShiftHistory from './components/ShiftHistory';
import { useSignalR } from './hooks/useSignalR';
import type {
  Employee,
  AttendanceStatus,
  ShiftLog,
  AttendanceActionResponse,
  AttendanceStateResponse,
} from './types';
import api from './api/client';

export default function App() {
  const [employee, setEmployee] = useState<Employee | null>(null);
  const [token, setToken] = useState<string | null>(
    localStorage.getItem('token')
  );
  const [status, setStatus] = useState<AttendanceStatus | null>(null);
  const [history, setHistory] = useState<ShiftLog[]>([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  // קולט עדכונים מ-SignalR ומעדכן את ה-state
  const handleStateChanged = useCallback((state: AttendanceStateResponse) => {
    setStatus(state.status);
    setHistory(state.history);
  }, []);

  useSignalR(token, handleStateChanged);

  const fetchAll = useCallback(async () => {
    const [s, h] = await Promise.all([
      api.get<AttendanceStatus>('/attendance/status'),
      api.get<ShiftLog[]>('/attendance/history'),
    ]);
    setStatus(s.data);
    setHistory(h.data);
  }, []);

  useEffect(() => {
    if (employee) fetchAll();
  }, [employee, fetchAll]);

  const handleLogin = (emp: Employee) => {
    localStorage.setItem('token', emp.token);
    setToken(emp.token);
    setEmployee(emp);
  };

  const handleLogout = () => {
    localStorage.removeItem('token');
    setToken(null);
    setEmployee(null);
    setStatus(null);
    setHistory([]);
  };

  const clockIn = async () => {
    setLoading(true);
    setError(null);
    try {
      const { data } = await api.post<AttendanceActionResponse>('/attendance/clock-in');
      setStatus(data.state.status);
      setHistory(data.state.history);
    } catch (e: any) {
      setError(e.response?.data?.error ?? 'שגיאה');
    } finally {
      setLoading(false);
    }
  };

  const clockOut = async () => {
    setLoading(true);
    setError(null);
    try {
      const { data } = await api.post<AttendanceActionResponse>('/attendance/clock-out');
      setStatus(data.state.status);
      setHistory(data.state.history);
    } catch (e: any) {
      setError(e.response?.data?.error ?? 'שגיאה');
    } finally {
      setLoading(false);
    }
  };

  const pause = async () => {
    setLoading(true);
    setError(null);
    try {
      const { data } = await api.post<AttendanceActionResponse>('/attendance/pause');
      setStatus(data.state.status);
      setHistory(data.state.history);
    } catch (e: any) {
      setError(e.response?.data?.error ?? 'שגיאה');
    } finally {
      setLoading(false);
    }
  };

  const resume = async () => {
    setLoading(true);
    setError(null);
    try {
      const { data } = await api.post<AttendanceActionResponse>('/attendance/resume');
      setStatus(data.state.status);
      setHistory(data.state.history);
    } catch (e: any) {
      setError(e.response?.data?.error ?? 'שגיאה');
    } finally {
      setLoading(false);
    }
  };

  if (!employee) return <LoginForm onLogin={handleLogin} />;

  return (
    <div className="app">
      <header className="header">
        <h2>🕐 מערכת נוכחות</h2>
        <div className="header-right">
          <span>שלום, {employee.employeeName}</span>
          <button className="btn-logout" onClick={handleLogout}>יציאה</button>
        </div>
      </header>
      <ClockPanel
        status={status}
        loading={loading}
        error={error}
        onClockIn={clockIn}
        onClockOut={clockOut}
        onPause={pause}
        onResume={resume}
      />
      <ShiftHistory history={history} />
    </div>
  );
}