using Microsoft.EntityFrameworkCore;
using AttendanceSystem.API.Models;

namespace AttendanceSystem.API.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Employee> Employees => Set<Employee>();
    public DbSet<ShiftLog> ShiftLogs => Set<ShiftLog>();
    public DbSet<ShiftPause> ShiftPauses => Set<ShiftPause>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Employee>(e =>
        {
            e.HasIndex(x => x.EmployeeNumber).IsUnique();
            e.Property(x => x.FullName).HasMaxLength(100).IsRequired();
        });

        modelBuilder.Entity<ShiftLog>(e =>
        {
            e.HasOne(x => x.Employee)
             .WithMany(x => x.ShiftLogs)
             .HasForeignKey(x => x.EmployeeId)
             .OnDelete(DeleteBehavior.Restrict);

            e.Property(s => s.DurationHours)
             .HasColumnType("decimal(18,2)");
        });

        modelBuilder.Entity<ShiftPause>(e =>
        {
            e.HasOne(x => x.ShiftLog)
             .WithMany(x => x.Pauses)
             .HasForeignKey(x => x.ShiftLogId)
             .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Employee>().HasData(new Employee
        {
            Id = 1,
            FullName = "ישראל ישראלי",
            EmployeeNumber = "EMP001",
            PinHash = "$2a$11$0aHbxOfMh5o4peJPZoaYZ.uouacakVwmbE/vPREcGqpZNgz6ixI0m",
            Role = "Employee",
            IsActive = true,
        });
         modelBuilder.Entity<ShiftLog>().HasData(
            new ShiftLog
            {
                Id = 1,
                EmployeeId = 1,
                ClockInTime = new DateTime(2026, 4, 14, 8, 0, 0),
                ClockOutTime = new DateTime(2026, 4, 14, 17, 0, 0),
                Status = "Closed",
                DurationHours = 8.00m
            },
            new ShiftLog
            {
                Id = 2,
                EmployeeId = 1,
                ClockInTime = new DateTime(2026, 4, 15, 9, 0, 0),
                ClockOutTime = new DateTime(2026, 4, 15, 18, 0, 0),
                Status = "Closed",
                DurationHours = 7.50m
            },
            new ShiftLog
            {
                Id = 3,
                EmployeeId = 1,
                ClockInTime = new DateTime(2026, 4, 16, 8, 30, 0),
                ClockOutTime = new DateTime(2026, 4, 16, 16, 45, 0),
                Status = "Closed",
                DurationHours = 7.25m
            }
        );

        modelBuilder.Entity<ShiftPause>().HasData(
            new ShiftPause
            {
                Id = 1,
                ShiftLogId = 1,
                PauseStart = new DateTime(2026, 4, 14, 12, 0, 0),
                PauseEnd = new DateTime(2026, 4, 14, 13, 0, 0)
            },
            new ShiftPause
            {
                Id = 2,
                ShiftLogId = 2,
                PauseStart = new DateTime(2026, 4, 15, 13, 0, 0),
                PauseEnd = new DateTime(2026, 4, 15, 13, 30, 0)
            },
            new ShiftPause
            {
                Id = 3,
                ShiftLogId = 3,
                PauseStart = new DateTime(2026, 4, 16, 11, 30, 0),
                PauseEnd = new DateTime(2026, 4, 16, 12, 0, 0)
            },
            new ShiftPause
            {
                Id = 4,
                ShiftLogId = 3,
                PauseStart = new DateTime(2026, 4, 16, 14, 30, 0),
                PauseEnd = new DateTime(2026, 4, 16, 15, 0, 0)
            }
        );
    }
}