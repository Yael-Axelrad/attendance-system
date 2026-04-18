export interface Employee {
  employeeName: string;
  role: string;
  token: string;
}

export interface ShiftLog {
  id: number;
  clockInTime: string;
  clockOutTime?: string;
  durationHours?: number;
  status: 'Open' | 'Paused' | 'Closed';
}

export interface AttendanceStatus {
  isClockedIn: boolean;
  isPaused: boolean;
  clockInTime?: string;
  elapsedSeconds: number;
}

export interface AttendanceStateResponse {
  status: AttendanceStatus;
  history: ShiftLog[];
}

export interface AttendanceActionResponse {
  message: string;
  state: AttendanceStateResponse;
}