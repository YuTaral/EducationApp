using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EducationApp.Migrations
{
    /// <inheritdoc />
    public partial class changedLessonTypeId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Lessons_LessonType_LessonTypeId",
                table: "Lessons");

            migrationBuilder.DropIndex(
                name: "IX_Lessons_LessonTypeId",
                table: "Lessons");

            migrationBuilder.DropColumn(
                name: "LessonTypeId",
                table: "Lessons");

            migrationBuilder.AddColumn<string>(
                name: "LessonType",
                table: "Lessons",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LessonType",
                table: "Lessons");

            migrationBuilder.AddColumn<int>(
                name: "LessonTypeId",
                table: "Lessons",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Lessons_LessonTypeId",
                table: "Lessons",
                column: "LessonTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Lessons_LessonType_LessonTypeId",
                table: "Lessons",
                column: "LessonTypeId",
                principalTable: "LessonType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
