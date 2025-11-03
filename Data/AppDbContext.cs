using Microsoft.EntityFrameworkCore;
using GymProgressTrackerAPI.Models;

namespace GymProgressTrackerAPI.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
	public DbSet<Workout> Workouts => Set<Workout>();
	public DbSet<WorkoutSet> WorkoutSets => Set<WorkoutSet>();
	public DbSet<Exercise> Exercises => Set<Exercise>();

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.Entity<Exercise>().HasKey(e => e.Id);
		modelBuilder.Entity<WorkoutSet>()
				.HasOne(s => s.Workout)
			    .WithMany(w => w.Sets)
			    .HasForeignKey(s => s.WorkoutId)
			    .OnDelete(DeleteBehavior.Cascade);

		modelBuilder.Entity<WorkoutSet>().Property(s => s.Weight).HasPrecision(8, 2);
		modelBuilder.Entity<WorkoutSet>().Property(s => s.Rpe).HasPrecision(4, 1);
	}
}
