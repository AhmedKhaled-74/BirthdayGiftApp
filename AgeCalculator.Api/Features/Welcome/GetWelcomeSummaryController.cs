using AgeCalculator.Api.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AgeCalculator.Api.Features.Welcome;

[ApiController]
[Route("api/welcome")]
public class GetWelcomeSummaryController(UserManager<ApplicationUser> userManager) : ControllerBase
{
    [HttpGet("summary")]
    public async Task<IActionResult> GetWelcomeSummary([FromQuery] WelcomeSummaryRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Timezone))
        {
            return BadRequest(new { error = "Timezone is required." });
        }

        TimeZoneInfo timeZone;
        try
        {
            timeZone = TimeZoneInfo.FindSystemTimeZoneById(request.Timezone);
        }
        catch (TimeZoneNotFoundException)
        {
            return BadRequest(new { error = "Invalid timezone." });
        }

        var user = await userManager.GetUserAsync(User);
        if (user is null)
        {
            return Unauthorized();
        }

        if (user.BirthDate is null)
        {
            return NotFound(new { error = "Birth date not set. Please add your birth date in Profile." });
        }

        var localNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZone);
        var today = localNow.Date;
        var (years, months, days, daysUntilNextBirthday, isBirthday) =
            AgeCalculation.Calculate(user.BirthDate.Value, today);

        return Ok(new AgeSummaryResponse(years, months, days, daysUntilNextBirthday, isBirthday));
    }
}