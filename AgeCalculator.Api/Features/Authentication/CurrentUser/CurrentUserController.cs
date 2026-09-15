using AgeCalculator.Api.Data;
using AgeCalculator.Api.Features.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AgeCalculator.Api.Features.Authentication.CurrentUser;

[ApiController]
[Route("api/auth")]
[Authorize]
public class CurrentUserController(UserManager<ApplicationUser> userManager) : ControllerBase
{
    [HttpGet("current")]
    public async Task<IActionResult> Current()
    {
        var user = await userManager.GetUserAsync(User);
        if (user is null)
        {
            return Unauthorized();
        }

        return Ok(new CurrentUserResponse(user.Id, user.Email!));
    }
}