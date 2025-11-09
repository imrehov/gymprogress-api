namespace GymProgressTrackerAPI.Models;

public class Workout
{
	//foreign key for user
	public Guid UserId { get; set; }
	public AppUser User { get; set; } = default!;

	public Guid Id { get; set; } = Guid.NewGuid();
	public DateOnly Date { get; set; }
	public string? Notes { get; set; }
	public ICollection<WorkoutSet> Sets { get; set; } = new List<WorkoutSet>();
}

