using GymProgressTrackerAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ActionConstraints;

namespace GymProgressTrackerAPI.Controllers;

[ApiController]
[Route("v1/auth")]
public class AuthController(
		UserManager<AppUser> userManager,
		SignInManager<AppUser> signInManager) : ControllerBase
{
	public record RegisterDto(string Email, string Password);
	public record LoginDto(string Email, string Password);
	public record UserDto(Guid Id, string Email);

	[HttpPost("register")]
	public async Task<IActionResult> Register([FromBody] RegisterDto dto)
	{
		var user = new AppUser { UserName = dto.Email, Email = dto.Email };
		var result = await userManager.CreateAsync(user, dto.Password);

		if (!result.Succeeded)
		{
			return BadRequest(new
			{
				errors = result.Errors.Select(e => e.Description)
			});
		}

		await signInManager.SignInAsync(user, isPersistent: false);
		return Ok(new UserDto(user.Id, user.Email!));
	}

	[HttpPost("login")]
	public async Task<IActionResult> Login([FromBody] LoginDto dto)
	{
		var user = await userManager.FindByEmailAsync(dto.Email);
		if (user is null)
		{
			return Unauthorized();
		}

		var result = await signInManager.CheckPasswordSignInAsync(user, dto.Password, false);
		if (!result.Succeeded)
		{
			return Unauthorized();
		}

		await signInManager.SignInAsync(user, isPersistent: false);
		return Ok(new UserDto(user.Id, user.Email!));
	}

	[Authorize]
	[HttpGet("me")]
	public async Task<IActionResult> Me()
	{
		var user = await userManager.GetUserAsync(User);
		return user is null ? Unauthorized() : Ok(new UserDto(user.Id, user.Email!));
	}

	[Authorize]
	[HttpPost("logout")]
	public async Task<IActionResult> Logout()
	{
		await signInManager.SignOutAsync();
		return NoContent();
	}
}
