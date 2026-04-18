import { useState } from 'react';
import api from '../api/client';
import type { Employee } from '../types';

interface Props { onLogin: (emp: Employee) => void; }

export default function LoginForm({ onLogin }: Props) {
  const [employeeNumber, setEmployeeNumber] = useState('');
  const [pin, setPin] = useState('');
  const [error, setError] = useState('');
  const [loading, setLoading] = useState(false);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setLoading(true); setError('');
    try {
      const { data } = await api.post<Employee>('/auth/login', { employeeNumber, pin });
      localStorage.setItem('token', data.token);
      onLogin(data);
    } catch { setError('מספר עובד או PIN שגוי'); }
    finally { setLoading(false); }
  };

  return (
    <div className="login-page">
      <div className="login-card">
        <h1>🕐 מערכת נוכחות</h1>
        <form onSubmit={handleSubmit}>
          <div className="field">
            <label>מספר עובד</label>
            <input value={employeeNumber} onChange={e => { setEmployeeNumber(e.target.value); setError(''); }} placeholder="יש להכניס מספר עובד" required />          </div>
          <div className="field">
            <label>קוד</label>
            <input type="password" value={pin} onChange={e => { setPin(e.target.value); setError(''); }} placeholder="יש להכניס קוד" required maxLength={6} />          </div>
          {error && <p className="error-text">{error}</p>}
          <button className="btn-login" type="submit" disabled={loading}>
            {loading ? 'מתחבר...' : 'כניסה למערכת'}
          </button>
        </form>
      </div>
    </div>
  );
}