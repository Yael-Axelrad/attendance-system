using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace AttendanceSystem.API.Migrations
{
    /// <inheritdoc />
    public partial class SeedDemoShifts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "ShiftLogs",
                columns: new[] { "Id", "ClockInTime", "ClockOutTime", "DurationHours", "EmployeeId", "Status" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 4, 14, 8, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 4, 14, 17, 0, 0, 0, DateTimeKind.Unspecified), 8.00m, 1, "Closed" },
                    { 2, new DateTime(2026, 4, 15, 9, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 4, 15, 18, 0, 0, 0, DateTimeKind.Unspecified), 7.50m, 1, "Closed" },
                    { 3, new DateTime(2026, 4, 16, 8, 30, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 4, 16, 16, 45, 0, 0, DateTimeKind.Unspecified), 7.25m, 1, "Closed" }
                });

            migrationBuilder.InsertData(
                table: "ShiftPauses",
                columns: new[] { "Id", "PauseEnd", "PauseStart", "ShiftLogId" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 4, 14, 13, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 4, 14, 12, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 2, new DateTime(2026, 4, 15, 13, 30, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 4, 15, 13, 0, 0, 0, DateTimeKind.Unspecified), 2 },
                    { 3, new DateTime(2026, 4, 16, 12, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 4, 16, 11, 30, 0, 0, DateTimeKind.Unspecified), 3 },
                    { 4, new DateTime(2026, 4, 16, 15, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 4, 16, 14, 30, 0, 0, DateTimeKind.Unspecified), 3 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "ShiftPauses",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "ShiftPauses",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "ShiftPauses",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "ShiftPauses",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "ShiftLogs",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "ShiftLogs",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "ShiftLogs",
                keyColumn: "Id",
                keyValue: 3);
        }
    }
}
