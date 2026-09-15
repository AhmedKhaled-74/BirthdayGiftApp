namespace AgeCalculator.Api.Features.Welcome;

public record AgeSummaryResponse(
    int Years,
    int Months,
    int Days,
    int DaysUntilNextBirthday,
    bool IsBirthday);