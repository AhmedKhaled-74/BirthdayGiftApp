using System.ComponentModel.DataAnnotations;

namespace AgeCalculator.Api.Features.Profile;

public record UpdateBirthDateRequest(
    [Required] DateTime BirthDate);