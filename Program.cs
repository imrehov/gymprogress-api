using GymProgressTrackerAPI.Data;
using Microsoft.EntityFrameworkCore;
using GymProgressTrackerAPI.Models;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi

// dbcontext
builder.Services.AddDbContext<AppDbContext>(opt =>
		opt.UseSqlite("Data Source=gym.db"));

//auth stuff

builder.Services
.AddIdentity<AppUser, IdentityRole<Guid>>(options =>
		{
			options.Password.RequireNonAlphanumeric = false;
			options.Password.RequireUppercase = false;
			options.Password.RequiredLength = 6;
			options.Password.RequireDigit = false;
			options.Password.RequireLowercase = false;
		})
	.AddEntityFrameworkStores<AppDbContext>()
	.AddDefaultTokenProviders();

//cookie config
builder.Services.ConfigureApplicationCookie(options =>
		{
			options.Cookie.Name = "GymAuth";
			options.Cookie.SameSite = SameSiteMode.None;
			options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
			options.SlidingExpiration = true;
		}
	);

builder.Services.AddAuthorization();

builder.Services.AddOpenApi();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(options =>
{
	options.AddPolicy("AllowFrontend", policy =>
	{
		policy
			  .SetIsOriginAllowed(origin =>
			      origin == "http://localhost:3000" ||
			      origin == "https://gym.hovodzak.hu" ||
			      origin.EndsWith(".vercel.app")
			  )
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

app.UseHttpsRedirection();
app.UseRouting();
app.UseSwagger();
app.UseSwaggerUI();
app.UseCors("AllowFrontend");

//auth stuff again

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();


app.MapGet("/", () =>
{
	Console.WriteLine("Hello");
	return TypedResults.Ok();
});

app.Run();


