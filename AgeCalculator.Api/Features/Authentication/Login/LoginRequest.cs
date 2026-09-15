using System.ComponentModel.DataAnnotations;

namespace AgeCalculator.Api.Features.Authentication.Login;

public record LoginRequest(
    [Required, EmailAddress] string Email,
    [Required] string Password);