var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(options =>
{
	options.AddPolicy("AllowFrontend", policy =>
	{
		policy.WithOrigins("http://localhost:3000")
		  .AllowAnyHeader()
		  .AllowAnyMethod();
	});
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.MapOpenApi();
}
app.UseSwagger();
app.UseSwaggerUI();
app.UseCors("AllowFrontend");

app.UseHttpsRedirection();

var workouts = new List<Workout>();

app.MapPost("/workouts", (Workout workout) =>
{
	workouts.Add(workout);
	return TypedResults.Created($"/workouts/{workout.id}", workout);
});


app.MapGet("/", () =>
{
	Console.WriteLine("Hello");
	return TypedResults.Ok();
});

app.Run();


public record Workout(string id, DateTime date);
