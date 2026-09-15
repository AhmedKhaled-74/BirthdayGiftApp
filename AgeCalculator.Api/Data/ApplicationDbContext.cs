using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AgeCalculator.Api.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : IdentityDbContext<ApplicationUser>(options)
{
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Birth Date is a calendar date without time or time zone (see CONTEXT.md).
        // Postgres `date` also avoids Npgsql rejecting Kind=Unspecified DateTime
        // values written to `timestamp with time zone`.
        builder.Entity<ApplicationUser>()
            .Property(u => u.BirthDate)
            .HasColumnType("date");
    }
}