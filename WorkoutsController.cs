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

	[HttpGet("{id}")]
	public IActionResult GetWorkoutById(string id) => StoredWorkouts.FirstOrDefault(w => w.Id == id) is { } w ? Ok(w) : NotFound();

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

	[HttpPost("{id}/sets")]
	public IActionResult AddSet(string id, [FromBody] CreateSet req)
	{
		var w = StoredWorkouts.FirstOrDefault(x => x.Id == id);
		if (w is null) return NotFound();

		//make sure exercise exists
		var exercise = w.Exercises.FirstOrDefault(e => e.Id == req.ExerciseId) ?? new Exercise { Id = req.ExerciseId };
		if (!w.Exercises.Any(e => e.Id == exercise.Id))
		{
			w.Exercises.Add(exercise);
		}
		var set = new Set
		{
			Id = Guid.NewGuid().ToString("n"),
			Reps = req.Reps,
			Weight = req.Weight,
			RPE = req.Rpe
		};
		exercise.Sets.Add(set);
		return Created($"/c1/sets/{set.Id}", set);
	}

	// i dont think this is very good like this but w.e for now
	//

	[HttpDelete("/v1/sets/{setId}")]
	public IActionResult DeleteSet(string setId)
	{
		foreach (var w in StoredWorkouts)
			foreach (var e in w.Exercises)
				if (e.Sets.RemoveAll(s => s.Id == setId) > 0)
				{
					return NoContent();
				}
		return NotFound();
	}
}
// use records instead of classes for easier code
public record Workout
{
	public string Id { get; init; } = "";
	public DateOnly Date { get; init; }
	public string? Notes { get; init; }
	public decimal? BodyWeight { get; init; }
	public List<Exercise> Exercises { get; init; } = new();
}

public record Exercise
{
	public string Id { get; init; } = "";
	public List<Set> Sets { get; init; } = new();
}

public record Set
{
	public string Id { get; init; } = "";
	public decimal Reps { get; init; }
	public decimal? Weight { get; init; }
	public decimal? RPE { get; init; }
}

public record CreateWorkout(DateOnly Date, string? Notes);
public record CreateSet(string ExerciseId, decimal Reps, decimal? Weight, decimal? Rpe);
