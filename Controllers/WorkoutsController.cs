using GymProgressTrackerAPI.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GymProgressTrackerAPI.Models;
using Microsoft.AspNetCore.Identity;

namespace GymProgressTrackerAPI.Controllers;

[ApiController]
[Route("v1/workouts")]

public class WorkoutsController : ControllerBase
{
	private readonly AppDbContext _db;
	private readonly UserManager<AppUser> _userManager;

	public WorkoutsController(AppDbContext db, UserManager<AppUser> userManager)
	{
		_db = db;
		_userManager = userManager;
	}

	//auth add on
	private bool TryGetUserId(out Guid userId)
	{
		userId = default;

		var idString = _userManager.GetUserId(User);
		if (string.IsNullOrEmpty(idString))
		{
			return false;
		}
		return Guid.TryParse(idString, out userId);
	}
	// maps requests on this route, fromquery frombody etc self-explanatory


	//now only return specific user data
	[HttpGet]
	public async Task<IActionResult> List([FromQuery] DateOnly from, [FromQuery] DateOnly to)
	{
		if (!TryGetUserId(out var userId))
		{
			return Unauthorized();
		}
		var items = await _db.Workouts
			.Where(w => w.UserId == userId && w.Date >= from && w.Date <= to)
			.Select(w => new { w.Id, w.Date, w.Notes })
			.ToListAsync();
		return Ok(items);
	}

	[HttpGet("{id:guid}")]
	public async Task<IActionResult> GetWorkoutById(Guid id)
	{
		if (!TryGetUserId(out var userId))
		{
			return Unauthorized();
		}

		var w = await _db.Workouts
			.Where(w => w.UserId == userId)
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

	//adding user specific stuff to create aswell
	[HttpPost]
	public async Task<IActionResult> Create([FromBody] CreateWorkoutDto req)
	{
		if (!TryGetUserId(out var userId))
		{
			return Unauthorized();
		}

		var w = new Workout
		{
			UserId = userId,
			Date = req.Date,
			Notes = req.Notes
		};
		_db.Workouts.Add(w);
		await _db.SaveChangesAsync();

		return Created($"/v1/workouts/{w.Id}", new { w.Id, w.Date, w.Notes, exercises = Array.Empty<object>() });
	}

	public record CreateSetDto(string ExerciseId, int Reps, decimal? Weight, decimal? Rpe);


	//adding user stuff
	[HttpPost("{id:guid}/sets")]
	public async Task<IActionResult> AddSet(Guid id, [FromBody] CreateSetDto req)
	{
		if (!TryGetUserId(out var userId))
		{
			return Unauthorized();
		}

		var exists = await _db.Workouts.AnyAsync(w => w.UserId == userId && w.Id == id);
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
		_db.WorkoutSets.Add(set);
		await _db.SaveChangesAsync();
		return Created($"/c1/sets/{set.Id}", new { set.Id, set.Reps, set.Weight, set.Rpe });
	}


	//adding userid stuff 
	[HttpDelete("/v1/sets/{setId:guid}")]
	public async Task<IActionResult> DeleteSet(Guid setId)
	{
		var s = await _db.WorkoutSets.FindAsync(setId);
		if (s is null) return NotFound();
		_db.WorkoutSets.Remove(s);
		await _db.SaveChangesAsync();
		return NoContent();
	}

	[HttpDelete("{id:guid}")]
	public async Task<IActionResult> DeleteWorkout(Guid id)
	{
		if (!TryGetUserId(out var userId))
		{
			return Unauthorized();
		}
		//this will cascade delete sets cuz of app_dbcontext setup
		var w = await _db.Workouts.FindAsync(id);
		if (w is null) return NotFound();
		if (w.UserId == userId)
		{
			_db.Workouts.Remove(w);
			await _db.SaveChangesAsync();
			return NoContent();
		}
		else
		{
			return Unauthorized();
		}
	}
}
