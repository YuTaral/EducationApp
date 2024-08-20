using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EducationApp.Migrations
{
    /// <inheritdoc />
    public partial class addedLessonFromToTime : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Date",
                table: "Lessons",
                newName: "ToDateTime");

            migrationBuilder.AddColumn<DateTime>(
                name: "FromDateTime",
                table: "Lessons",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FromDateTime",
                table: "Lessons");

            migrationBuilder.RenameColumn(
                name: "ToDateTime",
                table: "Lessons",
                newName: "Date");
        }
    }
}
