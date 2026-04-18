using System.Text.Json.Serialization;

namespace AttendanceSystem.API.Models;

public class ShiftPause
{
    public int Id { get; set; }
    public int ShiftLogId { get; set; }
    public DateTime PauseStart { get; set; }
    public DateTime? PauseEnd { get; set; }

    [JsonIgnore]
    public ShiftLog ShiftLog { get; set; } = null!;
}