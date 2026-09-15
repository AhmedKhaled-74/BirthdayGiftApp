using System.ComponentModel.DataAnnotations;

namespace AgeCalculator.Api.Features.Authentication.Register;

public record RegisterRequest(
    [Required, EmailAddress] string Email,
    [Required, MinLength(8)] string Password);