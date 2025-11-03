namespace GymProgressTrackerAPI.Models;

public class Workout
{
	public Guid Id { get; set; } = Guid.NewGuid();
	public DateOnly Date { get; set; }
	public string? Notes { get; set; }
	public ICollection<WorkoutSet> Sets { get; set; } = new List<WorkoutSet>();
}

