var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(options =>
{
	options.AddPolicy("AllowFrontend", policy =>
	{
		policy.WithOrigins("http://localhost:3000")
		  .AllowAnyHeader()
		  .AllowAnyMethod()
		  .AllowCredentials();
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

app.MapControllers();

app.UseHttpsRedirection();

app.MapGet("/", () =>
{
	Console.WriteLine("Hello");
	return TypedResults.Ok();
});

app.Run();


