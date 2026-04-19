# 🕐 Attendance System

A full-stack employee attendance system built with **React, ASP.NET Core, and SQL Server**, designed for real-time clock-in/clock-out operations using an external time API.

## Features

* Clock In / Clock Out / Pause / Resume
* Real-time updates via SignalR
* Shift history tracking
* Accurate time from external API (Europe/Zurich)

## Tech Stack

* **Frontend:** React, TypeScript
* **Backend:** ASP.NET Core Web API
* **Database:** SQL Server
* **Realtime:** SignalR

## Setup

### Backend

```bash
cd AttendanceSystem.API
dotnet restore
dotnet run
```

### Frontend

```bash
cd client
npm install
npm run dev
```

## Notes

* Uses external time service (no local or server time)

## Demo User

* Employee Number: `EMP001`
* PIN: `1234`

---

# 🕐 מערכת נוכחות

מערכת נוכחות עובדים הבנויה באמצעות **React, ASP.NET Core ו-SQL Server**, המאפשרת ניהול כניסה ויציאה ממשמרת בזמן אמת באמצעות שירות זמן חיצוני.

## פיצ'רים

* התחלת משמרת / סיום משמרת / השהיה / חידוש
* עדכונים בזמן אמת באמצעות SignalR
* צפייה בהיסטוריית משמרות
* שימוש בזמן מדויק משירות חיצוני (Europe/Zurich)

## טכנולוגיות

* **Frontend:** React, TypeScript
* **Backend:** ASP.NET Core Web API
* **Database:** SQL Server
* **Realtime:** SignalR

## הרצה

### צד שרת

```bash
cd AttendanceSystem.API
dotnet restore
dotnet run
```

### צד לקוח

```bash
cd client
npm install
npm run dev
```

## הערות

* המערכת משתמשת בשירות זמן חיצוני (ללא שימוש בזמן מקומי או זמן השרת)

## משתמש לדוגמה

* מספר עובד: `EMP001`
* קוד: `1234`
