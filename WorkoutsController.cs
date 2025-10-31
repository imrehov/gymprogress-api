using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("v1/workouts")]

public class WorkoutsController : ControllerBase
{
	private static readonly List<Workout> StoredWorkouts = new();

	// maps requests on this route, fromquery frombody etc self-explanatory

	[HttpGet]
	public IActionResult List([FromQuery] DateOnly from, [FromQuery] DateOnly to)
	{
		var items = StoredWorkouts.Where(w => w.Date >= from && w.Date <= to).ToList();
		return Ok(items);
	}

	[HttpPost]
	public IActionResult Create([FromBody] CreateWorkout req)
	{
		var w = new Workout
		{
			Id = Guid.NewGuid().ToString("n"),
			Date = req.Date,
			Notes = req.Notes
		};
		StoredWorkouts.Add(w);

		return Created($"/v1/workouts/{w.Id}", w);
	}
}
// use records instead of classes for easier code
public record Workout
{
	public string Id { get; init; } = "";
	public DateOnly Date { get; init; }
	public string? Notes { get; init; }
}

public record CreateWorkout(DateOnly Date, string? Notes);
