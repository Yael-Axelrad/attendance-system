using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using AttendanceSystem.API.Hubs;
using AttendanceSystem.API.Services;

namespace AttendanceSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AttendanceController : ControllerBase
{
    private readonly IAttendanceService _attendance;
    private readonly ITimeService _timeService;
    private readonly IHubContext<AttendanceHub> _hub;

    public AttendanceController(
        IAttendanceService attendance,
        ITimeService timeService,
        IHubContext<AttendanceHub> hub)
    {
        _attendance = attendance;
        _timeService = timeService;
        _hub = hub;
    }

    private int GetEmployeeId() =>
        int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    private async Task<object> BuildAttendanceStateAsync(int employeeId)
    {
        var active = await _attendance.GetActiveShiftAsync(employeeId);
        var history = await _attendance.GetHistoryAsync(employeeId, 30);

        if (active == null)
        {
            return new
            {
                status = new
                {
                    isClockedIn = false,
                    isPaused = false,
                    clockInTime = (DateTime?)null,
                    elapsedSeconds = 0
                },
                history
            };
        }

        DateTime currentExternalTime = await _timeService.GetCurrentTimeAsync();

        var completedPauseSeconds = active.Pauses
            .Where(p => p.PauseEnd != null)
            .Sum(p => (p.PauseEnd!.Value - p.PauseStart).TotalSeconds);

        var currentPause = active.Pauses.FirstOrDefault(p => p.PauseEnd == null);

        var currentPauseSeconds = currentPause != null
            ? (currentExternalTime - currentPause.PauseStart).TotalSeconds
            : 0;

        var totalPausedSeconds = completedPauseSeconds + currentPauseSeconds;
        var elapsedSeconds = (currentExternalTime - active.ClockInTime).TotalSeconds - totalPausedSeconds;

        return new
        {
            status = new
            {
                isClockedIn = true,
                isPaused = active.Status == "Paused",
                clockInTime = active.ClockInTime,
                elapsedSeconds = (int)Math.Max(0, elapsedSeconds)
            },
            history
        };
    }

    [HttpPost("clock-in")]
    public async Task<IActionResult> ClockIn()
    {
        var employeeId = GetEmployeeId();
        var result = await _attendance.ClockInAsync(employeeId);

        if (!result.IsSuccess)
            return BadRequest(new { error = result.Error });

        try
        {
            var state = await BuildAttendanceStateAsync(employeeId);
            await _hub.Clients
                .Group($"employee-{employeeId}")
                .SendAsync("StateChanged", state);

            return Ok(new { message = "כניסה נרשמה", state });
        }
        catch
        {
            return StatusCode(503, new { error = "שירות הזמן אינו זמין" });
        }
    }

    [HttpPost("clock-out")]
    public async Task<IActionResult> ClockOut()
    {
        var employeeId = GetEmployeeId();
        var result = await _attendance.ClockOutAsync(employeeId);

        if (!result.IsSuccess)
            return BadRequest(new { error = result.Error });

        try
        {
            var state = await BuildAttendanceStateAsync(employeeId);
            await _hub.Clients
                .Group($"employee-{employeeId}")
                .SendAsync("StateChanged", state);

            return Ok(new { message = "יציאה נרשמה", state });
        }
        catch
        {
            return StatusCode(503, new { error = "שירות הזמן אינו זמין" });
        }
    }

    [HttpPost("pause")]
    public async Task<IActionResult> Pause()
    {
        var employeeId = GetEmployeeId();
        var result = await _attendance.PauseAsync(employeeId);

        if (!result.IsSuccess)
            return BadRequest(new { error = result.Error });

        try
        {
            var state = await BuildAttendanceStateAsync(employeeId);
            await _hub.Clients
                .Group($"employee-{employeeId}")
                .SendAsync("StateChanged", state);

            return Ok(new { message = result.Message, state });
        }
        catch
        {
            return StatusCode(503, new { error = "שירות הזמן אינו זמין" });
        }
    }

    [HttpPost("resume")]
    public async Task<IActionResult> Resume()
    {
        var employeeId = GetEmployeeId();
        var result = await _attendance.ResumeAsync(employeeId);

        if (!result.IsSuccess)
            return BadRequest(new { error = result.Error });

        try
        {
            var state = await BuildAttendanceStateAsync(employeeId);
            await _hub.Clients
                .Group($"employee-{employeeId}")
                .SendAsync("StateChanged", state);

            return Ok(new { message = result.Message, state });
        }
        catch
        {
            return StatusCode(503, new { error = "שירות הזמן אינו זמין" });
        }
    }

    [HttpGet("status")]
    public async Task<IActionResult> GetStatus()
    {
        var employeeId = GetEmployeeId();

        try
        {
            var state = await BuildAttendanceStateAsync(employeeId);
            var status = state.GetType().GetProperty("status")?.GetValue(state);
            return Ok(status);
        }
        catch
        {
            return StatusCode(503, new { error = "שירות הזמן אינו זמין" });
        }
    }

    [HttpGet("history")]
    public async Task<IActionResult> GetHistory([FromQuery] int days = 30)
    {
        var logs = await _attendance.GetHistoryAsync(GetEmployeeId(), days);
        return Ok(logs);
    }
}