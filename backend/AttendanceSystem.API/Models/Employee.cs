namespace AttendanceSystem.API.Models;

public class Employee
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string EmployeeNumber { get; set; } = string.Empty;
    public string PinHash { get; set; } = string.Empty;
    public string Role { get; set; } = "Employee";
    public bool IsActive { get; set; } = true;
    public ICollection<ShiftLog> ShiftLogs { get; set; } = new List<ShiftLog>();
}