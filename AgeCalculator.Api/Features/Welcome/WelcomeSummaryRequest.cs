using System.ComponentModel.DataAnnotations;

namespace AgeCalculator.Api.Features.Welcome;

public record WelcomeSummaryRequest(
    [Required] string Timezone);