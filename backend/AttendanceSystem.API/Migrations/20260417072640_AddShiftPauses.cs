using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AttendanceSystem.API.Migrations
{
    /// <inheritdoc />
    public partial class AddShiftPauses : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ShiftPauses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ShiftLogId = table.Column<int>(type: "int", nullable: false),
                    PauseStart = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PauseEnd = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShiftPauses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ShiftPauses_ShiftLogs_ShiftLogId",
                        column: x => x.ShiftLogId,
                        principalTable: "ShiftLogs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ShiftPauses_ShiftLogId",
                table: "ShiftPauses",
                column: "ShiftLogId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ShiftPauses");
        }
    }
}
