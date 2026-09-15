using AgeCalculator.Api.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AgeCalculator.Api.Features.Profile;

[ApiController]
[Route("api/profile")]
public class GetProfileController(UserManager<ApplicationUser> userManager) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetProfile()
    {
        var user = await userManager.GetUserAsync(User);
        if (user is null)
        {
            return Unauthorized();
        }

        return Ok(new ProfileResponse(user.Id, user.Email!, user.BirthDate));
    }
}