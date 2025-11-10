using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using GymProgressTrackerAPI.Models;
using Microsoft.AspNetCore.Identity;

namespace GymProgressTrackerAPI.Data;

//dont forget primarykey for identity
public class AppDbContext : IdentityDbContext<AppUser, IdentityRole<Guid>, Guid>
{
	public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
	{
	}
	public DbSet<Workout> Workouts => Set<Workout>();
	public DbSet<WorkoutSet> WorkoutSets => Set<WorkoutSet>();
	public DbSet<Exercise> Exercises => Set<Exercise>();

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		//it crashes without this line
		base.OnModelCreating(modelBuilder);


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
