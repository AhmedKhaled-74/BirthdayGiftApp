using AgeCalculator.Api.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AgeCalculator.Api.Features.Authentication.Logout;

[ApiController]
[Route("api/auth")]
public class LogoutController(SignInManager<ApplicationUser> signInManager) : ControllerBase
{
    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        await signInManager.SignOutAsync();
        return NoContent();
    }
}