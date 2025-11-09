using GymProgressTrackerAPI.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GymProgressTrackerAPI.Models;

namespace GymProgressTrackerAPI.Controllers;

[ApiController]
[Route("v1/workouts")]

public class WorkoutsController(AppDbContext db) : ControllerBase
{
	// maps requests on this route, fromquery frombody etc self-explanatory

	[HttpGet]
	public async Task<IActionResult> List([FromQuery] DateOnly from, [FromQuery] DateOnly to)
	{
		var items = await db.Workouts
			.Where(w => w.Date >= from && w.Date <= to)
			.Select(w => new { w.Id, w.Date, w.Notes })
			.ToListAsync();
		return Ok(items);
	}

	[HttpGet("{id:guid}")]
	public async Task<IActionResult> GetWorkoutById(Guid id)
	{
		var w = await db.Workouts
			.Include(w => w.Sets)
			.FirstOrDefaultAsync(w => w.Id == id);

		return w is null ? NotFound() : Ok(new
		{
			w.Id,
			w.Date,
			w.Notes,
			exercises = w.Sets
				.GroupBy(s => s.ExerciseId)
				.Select(g => new
				{
					id = g.Key,
					sets = g.Select(s => new
					{ s.Id, s.Reps, s.Weight, s.Rpe })
				})
		});
	}

	public record CreateWorkoutDto(DateOnly Date, string? Notes);

	[HttpPost]
	public async Task<IActionResult> Create([FromBody] CreateWorkoutDto req)
	{
		var w = new Workout
		{
			Date = req.Date,
			Notes = req.Notes
		};
		db.Workouts.Add(w);
		await db.SaveChangesAsync();

		return Created($"/v1/workouts/{w.Id}", new { w.Id, w.Date, w.Notes, exercises = Array.Empty<object>() });
	}

	public record CreateSetDto(string ExerciseId, int Reps, decimal? Weight, decimal? Rpe);


	[HttpPost("{id:guid}/sets")]
	public async Task<IActionResult> AddSet(Guid id, [FromBody] CreateSetDto req)
	{
		var exists = await db.Workouts.AnyAsync(w => w.Id == id);
		if (!exists) return NotFound();

		//make sure exercise exists
		var set = new WorkoutSet
		{
			WorkoutId = id,
			ExerciseId = req.ExerciseId,
			Reps = req.Reps,
			Weight = req.Weight,
			Rpe = req.Rpe
		};
		db.WorkoutSets.Add(set);
		await db.SaveChangesAsync();
		return Created($"/c1/sets/{set.Id}", new { set.Id, set.Reps, set.Weight, set.Rpe });
	}

	// i dont think this is very good like this but w.e for now
	//

	[HttpDelete("/v1/sets/{setId:guid}")]
	public async Task<IActionResult> DeleteSet(Guid setId)
	{
		var s = await db.WorkoutSets.FindAsync(setId);
		if (s is null) return NotFound();
		db.WorkoutSets.Remove(s);
		await db.SaveChangesAsync();
		return NoContent();
	}

	[HttpDelete("{id:guid}")]
	public async Task<IActionResult> DeleteWorkout(Guid id)
	{
		//this will cascade delete sets cuz of appdbcontext setup
		var w = await db.Workouts.FindAsync(id);
		if (w is null) return NotFound();
		db.Workouts.Remove(w);
		await db.SaveChangesAsync();
		return NoContent();
	}
}
