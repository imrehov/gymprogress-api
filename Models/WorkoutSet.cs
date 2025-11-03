namespace GymProgressTrackerAPI.Models;

public class WorkoutSet
{
	public Guid Id { get; set; } = Guid.NewGuid();
	public Guid WorkoutId { get; set; }
	public Workout Workout { get; set; } = default;
	public string ExerciseId { get; set; } = "";
	public int Reps { get; set; }
	public decimal? Weight { get; set; }
	public decimal? Rpe { get; set; }
}
