using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AttendanceSystem.API.Migrations
{
    /// <inheritdoc />
    public partial class _1111111 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Employees",
                keyColumn: "Id",
                keyValue: 1,
                column: "PinHash",
                value: "$2a$11$0aHbxOfMh5o4peJPZoaYZ.uouacakVwmbE/vPREcGqpZNgz6ixI0m");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Employees",
                keyColumn: "Id",
                keyValue: 1,
                column: "PinHash",
                value: "$2a$11$SHm.eJ4t2U4G0ZvaOeDhy.PM8wensBstri3R8yiUHMkpZZSlPiky");
        }
    }
}
