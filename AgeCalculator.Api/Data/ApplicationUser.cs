using Microsoft.AspNetCore.Identity;

namespace AgeCalculator.Api.Data;

public class ApplicationUser : IdentityUser
{
    public DateTime? BirthDate { get; set; }
}