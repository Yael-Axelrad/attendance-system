# CLAUDE.md — מערכת נוכחות עובדים

## מה המערכת הזאת עושה

מערכת שעון נוכחות לעובדים. עובד מתחבר עם מספר עובד + PIN,
מחתים כניסה/יציאה, יכול להשהות משמרת ולחזור. כל פעולה נרשמת
עם זמן שמגיע משירות חיצוני בלבד.

---

## ארכיטקטורה

```
Frontend (React + TypeScript + Vite) — localhost:5173
    │ HTTP REST + WebSocket (SignalR)
    ▼
Backend (ASP.NET Core 9.0) — localhost:5261
    │ Entity Framework Core
    ▼
SQL Server (AttendanceDB)
    │ HTTP
    ▼
timeapi.io — זמן אמיתי לאזור Europe/Zurich
```

### שכבות Backend

- **Controllers** — קבלת HTTP requests, חילוץ userId מ-JWT, קריאה ל-Services
- **Services** — כל הלוגיקה העסקית (AttendanceService, TimeService)
- **Models** — Employee, ShiftLog, ShiftPause
- **Data/AppDbContext** — Entity Framework, קשרים, seed data
- **Hubs/AttendanceHub** — SignalR, עדכונים בזמן אמת

### שכבות Frontend

- **App.tsx** — state מרכזי, ניהול auth, קריאות API
- **components/** — LoginForm, ClockPanel, ShiftHistory
- **api/client.ts** — axios עם JWT interceptor אוטומטי
- **hooks/useSignalR.ts** — חיבור SignalR וטיפול בעדכונים
- **types/index.ts** — כל ה-TypeScript interfaces

---

## חוקי מערכת — חובה לשמור עליהם

### זמן חיצוני בלבד

- **אסור** להשתמש ב-`DateTime.Now`, `DateTime.UtcNow`, או זמן הדפדפן
- **חובה** לקרוא ל-`TimeService.GetCurrentTimeAsync()` לפני כל clock-in/out
- מקור הזמן: `https://timeapi.io/api/time/current/zone?timeZone=Europe/Zurich`
- אם ה-API נכשל — יש לזרוק שגיאה ברורה, לא להחליף בזמן לוקלי

### סטטוסי משמרת

- `Open` — עובד בפנים
- `Paused` — משמרת מושהית
- `Closed` — יצא

### חישוב שעות עבודה

- DurationHours = (ClockOut - ClockIn) - סך כל ההפסקות
- הפסקות מחושבות לפי ShiftPause records בלבד

---

## אבטחה — JWT

### איך זה עובד

1. עובד שולח `{ employeeNumber, pin }` ל-`POST /api/auth/login`
2. Backend מוצא את העובד, מריץ `BCrypt.Verify(pin, employee.PinHash)`
3. אם תקין — יוצר JWT עם claims: `Id`, `FullName`, `Role`, expires: 12h
4. Frontend שומר token ב-`localStorage`
5. כל request כולל header: `Authorization: Bearer <token>`
6. SignalR שולח token ב-query string (`?access_token=...`) כי WebSocket לא תומך ב-headers

### מה נשלח ברשת

**Request התחברות:**
```
POST /api/auth/login
Body: { "employeeNumber": "EMP001", "pin": "1234" }
```

**Response:**
```json
{ "token": "eyJ...", "employeeName": "ישראל ישראלי", "role": "Employee" }
```

**כל request מאומת:**
```
Authorization: Bearer eyJ...
```

### נקודות אבטחה

- ה-PIN נשמר כ-BCrypt hash — גם אם DB נפרץ, ה-PIN לא חשוף
- מפתח JWT חייב להיות ב-Environment Variable, לא ב-appsettings.json
- Rate Limiting על Login — לא מומש, נקודת שיפור ידועה
- localStorage חשוף ל-XSS — חלופה מאובטחת יותר היא httpOnly cookie

---

## הנחיות עבודה עם Claude

### כשמוסיפים פיצ'ר חדש

1. תאר את הפיצ'ר ב-1-2 משפטים
2. ציין אילו קבצים רלוונטיים
3. בקש תחילה את הגישה — לא ישר קוד

### כשמתקנים באג

1. ציין את הקובץ והשורה אם ידוע
2. תאר מה אמור לקרות ומה קורה בפועל
3. בקש תיקון מינימלי — לא refactor שלם

### עקרונות קוד

- קוד פשוט > קוד "חכם"
- שמות משתנים ברורים — אין צורך בתגובות
- אין לשנות קוד שעובד רק כדי "לנקות"
- לא להוסיף abstraction שלא נדרש עכשיו

---

## Endpoints — סיכום

| Method | Path | מה עושה |
|--------|------|----------|
| POST | `/api/auth/login` | התחברות, מחזיר JWT |
| POST | `/api/attendance/clock-in` | פתיחת משמרת |
| POST | `/api/attendance/clock-out` | סגירת משמרת |
| POST | `/api/attendance/pause` | השהיית משמרת |
| POST | `/api/attendance/resume` | חזרה ממושהה |
| GET | `/api/attendance/status` | סטטוס נוכחי |
| GET | `/api/attendance/history?days=30` | היסטוריה |
| WS | `/hubs/attendance` | SignalR — עדכונים live |

---

## מודל הנתונים

```
Employees (Id, FullName, EmployeeNumber, PinHash, Role, IsActive)
    │ 1:N
ShiftLogs (Id, EmployeeId, ClockInTime, ClockOutTime, Status, DurationHours)
    │ 1:N
ShiftPauses (Id, ShiftLogId, PauseStart, PauseEnd)
```

---

## טיימר — איך עובד

הטיימר **לא מבצע polling**:

1. השרת מחשב `elapsedSeconds` (כולל הפחתת הפסקות) ושולח לפרונטאנד
2. הפרונטאנד מריץ `setInterval(1000ms)` ומוסיף 1 בכל שנייה
3. אם `isPaused === true` — הטיימר מפסיק לספור
4. עדכון מלא מהשרת מגיע רק בעת פעולה דרך SignalR

---

## UI / UX

### תמיכה ב-RTL

- שפת הממשק: עברית
- כל הדפים מוגדרים עם `dir="rtl"` ב-`index.html`
- CSS כתוב עם RTL בתור ברירת מחדל — אין flip ידני

### עיצוב רספונסיבי

- תמיכה ב-mobile ו-desktop מהקופסה
- רוחב מקסימלי של 800px, ממורכז עם `margin: auto`
- גדלי טקסט מסתגלים עם `clamp()` — ללא breakpoints ידניים

### פריסה

- שימוש ב-Flexbox לכפתורים ולכותרת (`.clock-buttons`, `.header`)
- אין תלות בספריית UI חיצונית — עיצוב ידני בלבד

### חווית משתמש

- כפתורי פעולה גדולים, ברורים, ומשתנים לפי סטטוס
- הודעות שגיאה בעברית ישירות מהשרת

---

## תלויות עיקריות

- **BCrypt:** `BCrypt.Net-Next` v4.1.0
- **Auth:** `Microsoft.AspNetCore.Authentication.JwtBearer` v9.0.4
- **ORM:** `Microsoft.EntityFrameworkCore.SqlServer` v9.0.4
- **Real-time:** `@microsoft/signalr` v10.0.0
- **HTTP Client:** `axios` v1.15.0
