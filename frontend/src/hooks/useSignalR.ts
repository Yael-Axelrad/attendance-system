import { useEffect, useRef } from 'react';
import * as signalR from '@microsoft/signalr';
import type { AttendanceStateResponse } from '../types';

export function useSignalR(
  token: string | null,
  onStateChanged: (state: AttendanceStateResponse) => void
) {
  const connectionRef = useRef<signalR.HubConnection | null>(null);

  useEffect(() => {
    if (!token) return;

    const connection = new signalR.HubConnectionBuilder()
      .withUrl(`${import.meta.env.VITE_API_URL}/hubs/attendance`, {
        accessTokenFactory: () => token,
      })
      .withAutomaticReconnect()
      .configureLogging(signalR.LogLevel.Warning)
      .build();

    connection.on('StateChanged', (state: AttendanceStateResponse) => {
      onStateChanged(state);
    });

    connection
      .start()
      .then(() => connection.invoke('JoinEmployeeGroup'))
      .catch(err => console.error('SignalR connection error:', err));

    connectionRef.current = connection;

    return () => {
      connection.stop();
    };
  }, [token]); // eslint-disable-line react-hooks/exhaustive-deps
}