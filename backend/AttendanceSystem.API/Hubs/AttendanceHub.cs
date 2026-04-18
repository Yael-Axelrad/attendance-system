using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace AttendanceSystem.API.Hubs;

[Authorize]
public class AttendanceHub : Hub
{
    public async Task JoinEmployeeGroup()
    {
        var employeeId = Context.User?.FindFirstValue(ClaimTypes.NameIdentifier);
        if (employeeId != null)
            await Groups.AddToGroupAsync(Context.ConnectionId, $"employee-{employeeId}");
    }
}