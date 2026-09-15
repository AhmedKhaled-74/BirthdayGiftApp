using AgeCalculator.Api.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AgeCalculator.Api.Features.Profile;

[ApiController]
[Route("api/profile")]
public class UpdateBirthDateController(UserManager<ApplicationUser> userManager) : ControllerBase
{
    [HttpPut("birth-date")]
    public async Task<IActionResult> UpdateBirthDate(UpdateBirthDateRequest request)
    {
        if (request.BirthDate.Date > DateTime.Today)
        {
            return BadRequest(new { error = "Birth date cannot be in the future." });
        }

        var user = await userManager.GetUserAsync(User);
        if (user is null)
        {
            return Unauthorized();
        }

        user.BirthDate = request.BirthDate.Date;
        var result = await userManager.UpdateAsync(user);
        if (!result.Succeeded)
        {
            return BadRequest(new { errors = result.Errors.Select(e => e.Description) });
        }

        return Ok(new ProfileResponse(user.Id, user.Email!, user.BirthDate));
    }
}