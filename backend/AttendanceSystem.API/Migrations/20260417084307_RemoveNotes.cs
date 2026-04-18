using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AttendanceSystem.API.Migrations
{
    /// <inheritdoc />
    public partial class RemoveNotes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Notes",
                table: "ShiftLogs");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Notes",
                table: "ShiftLogs",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
