using Microsoft.EntityFrameworkCore;
using AttendanceSystem.API.Data;
using AttendanceSystem.API.Models;

namespace AttendanceSystem.API.Services;

public interface IAttendanceService
{
    Task<ClockInResult> ClockInAsync(int employeeId);
    Task<ClockOutResult> ClockOutAsync(int employeeId);
    Task<PauseResult> PauseAsync(int employeeId);
    Task<PauseResult> ResumeAsync(int employeeId);
    Task<ShiftLog?> GetActiveShiftAsync(int employeeId);
    Task<IEnumerable<ShiftLog>> GetHistoryAsync(int employeeId, int days = 30);
}

public class AttendanceService : IAttendanceService
{
    private readonly AppDbContext _db;
    private readonly ITimeService _timeService;

    public AttendanceService(AppDbContext db, ITimeService timeService)
    {
        _db = db;
        _timeService = timeService;
    }

    public async Task<ClockInResult> ClockInAsync(int employeeId)
    {
        var openShift = await GetActiveShiftAsync(employeeId);
        if (openShift != null)
            return ClockInResult.Fail("יש משמרת פתוחה. יש לבצע Clock Out תחילה.");

        DateTime currentExternalTime;
        try
        {
            currentExternalTime = await _timeService.GetCurrentTimeAsync();
        }
        catch (TimeServiceException ex)
        {
            return ClockInResult.Fail(ex.Message);
        }

        var shift = new ShiftLog
        {
            EmployeeId = employeeId,
            ClockInTime = currentExternalTime,
            Status = "Open"
        };

        _db.ShiftLogs.Add(shift);
        await _db.SaveChangesAsync();

        return ClockInResult.Success(shift);
    }

    public async Task<ClockOutResult> ClockOutAsync(int employeeId)
    {
        var openShift = await _db.ShiftLogs
            .Include(s => s.Pauses)
            .FirstOrDefaultAsync(s => s.EmployeeId == employeeId &&
                (s.Status == "Open" || s.Status == "Paused"));

        if (openShift == null)
            return ClockOutResult.Fail("אין משמרת פתוחה לסגירה.");

        DateTime currentExternalTime;
        try
        {
            currentExternalTime = await _timeService.GetCurrentTimeAsync();
        }
        catch (TimeServiceException ex)
        {
            return ClockOutResult.Fail(ex.Message);
        }

        var openPause = openShift.Pauses.FirstOrDefault(p => p.PauseEnd == null);
        if (openPause != null)
            openPause.PauseEnd = currentExternalTime;

        var totalPauseMinutes = openShift.Pauses
            .Where(p => p.PauseEnd != null)
            .Sum(p => (p.PauseEnd!.Value - p.PauseStart).TotalMinutes);

        openShift.ClockOutTime = currentExternalTime;
        openShift.Status = "Closed";

        var totalMinutes = (currentExternalTime - openShift.ClockInTime).TotalMinutes - totalPauseMinutes;
        openShift.DurationHours = (decimal)(totalMinutes / 60);

        await _db.SaveChangesAsync();

        return ClockOutResult.Success(openShift);
    }

    public async Task<PauseResult> PauseAsync(int employeeId)
    {
        var openShift = await _db.ShiftLogs
            .Include(s => s.Pauses)
            .FirstOrDefaultAsync(s => s.EmployeeId == employeeId && s.Status == "Open");

        if (openShift == null)
            return PauseResult.Fail("אין משמרת פתוחה להשהיה.");

        DateTime currentExternalTime;
        try
        {
            currentExternalTime = await _timeService.GetCurrentTimeAsync();
        }
        catch (TimeServiceException ex)
        {
            return PauseResult.Fail(ex.Message);
        }

        var pause = new ShiftPause
        {
            ShiftLogId = openShift.Id,
            PauseStart = currentExternalTime
        };

        openShift.Status = "Paused";
        _db.ShiftPauses.Add(pause);

        await _db.SaveChangesAsync();

        return PauseResult.Success("המשמרת הושהתה");
    }

    public async Task<PauseResult> ResumeAsync(int employeeId)
    {
        var pausedShift = await _db.ShiftLogs
            .Include(s => s.Pauses)
            .FirstOrDefaultAsync(s => s.EmployeeId == employeeId && s.Status == "Paused");

        if (pausedShift == null)
            return PauseResult.Fail("אין משמרת מושהית.");

        DateTime currentExternalTime;
        try
        {
            currentExternalTime = await _timeService.GetCurrentTimeAsync();
        }
        catch (TimeServiceException ex)
        {
            return PauseResult.Fail(ex.Message);
        }

        var openPause = pausedShift.Pauses.FirstOrDefault(p => p.PauseEnd == null);
        if (openPause != null)
            openPause.PauseEnd = currentExternalTime;

        pausedShift.Status = "Open";

        await _db.SaveChangesAsync();

        return PauseResult.Success("המשמרת חודשה");
    }

    public async Task<ShiftLog?> GetActiveShiftAsync(int employeeId)
    {
        return await _db.ShiftLogs
            .Include(s => s.Pauses)
            .FirstOrDefaultAsync(s => s.EmployeeId == employeeId &&
                (s.Status == "Open" || s.Status == "Paused"));
    }

    public async Task<IEnumerable<ShiftLog>> GetHistoryAsync(int employeeId, int days = 30)
    {
        DateTime currentExternalTime;
        try
        {
            currentExternalTime = await _timeService.GetCurrentTimeAsync();
        }
        catch (TimeServiceException ex)
        {
            throw new TimeServiceException($"לא ניתן לשלוף היסטוריה כי שירות הזמן אינו זמין: {ex.Message}", ex);
        }

        var fromDate = currentExternalTime.AddDays(-days);

        return await _db.ShiftLogs
            .Include(s => s.Pauses)
            .Where(s => s.EmployeeId == employeeId && s.ClockInTime >= fromDate)
            .OrderByDescending(s => s.ClockInTime)
            .ToListAsync();
    }
}

public record ClockInResult(bool IsSuccess, string? Error, ShiftLog? Shift)
{
    public static ClockInResult Success(ShiftLog s) => new(true, null, s);
    public static ClockInResult Fail(string e) => new(false, e, null);
}

public record ClockOutResult(bool IsSuccess, string? Error, ShiftLog? Shift)
{
    public static ClockOutResult Success(ShiftLog s) => new(true, null, s);
    public static ClockOutResult Fail(string e) => new(false, e, null);
}

public record PauseResult(bool IsSuccess, string? Error, string? Message)
{
    public static PauseResult Success(string m) => new(true, null, m);
    public static PauseResult Fail(string e) => new(false, e, null);
}