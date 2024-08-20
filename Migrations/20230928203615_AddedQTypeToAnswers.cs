using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EducationApp.Migrations
{
    /// <inheritdoc />
    public partial class AddedQTypeToAnswers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "QuestionType",
                table: "Answer",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "QuestionType",
                table: "Answer");
        }
    }
}
