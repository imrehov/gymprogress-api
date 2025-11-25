using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GymProgressTrackerAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddWorkoutTitleAndExerciseNotes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "Workouts",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ExerciseNotes",
                table: "Exercises",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Title",
                table: "Workouts");

            migrationBuilder.DropColumn(
                name: "ExerciseNotes",
                table: "Exercises");
        }
    }
}
