namespace AttendanceSystem.API.Models;

public class ShiftLog
{
    public int Id { get; set; }
    public int EmployeeId { get; set; }
    public DateTime ClockInTime { get; set; }
    public DateTime? ClockOutTime { get; set; }
    public string Status { get; set; } = "Open"; // Open | Paused | Closed
    public decimal? DurationHours { get; set; }
    public Employee Employee { get; set; } = null!;
    public ICollection<ShiftPause> Pauses { get; set; } = new List<ShiftPause>();
}