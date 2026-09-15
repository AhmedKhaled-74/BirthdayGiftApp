namespace AgeCalculator.Api.Features.Profile;

public record ProfileResponse(string Id, string Email, DateTime? BirthDate);