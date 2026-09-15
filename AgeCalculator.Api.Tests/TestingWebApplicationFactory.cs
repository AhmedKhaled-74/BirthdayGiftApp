using System.Net;
using AgeCalculator.Api.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace AgeCalculator.Api.Tests;

/// <summary>
/// API factory backed by a shared SQLite in-memory database.
/// Each factory instance owns an isolated database; the app skips
/// Postgres migrations when running under the Testing environment
/// (see Program.cs) and the schema is created with EnsureCreated.
/// </summary>
public class TestingWebApplicationFactory : WebApplicationFactory<Program>
{
    private SqliteConnection? _connection;

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureServices(services =>
        {
            // Program.cs skips the Postgres registration under the
            // Testing environment, so SQLite is the only provider here.
            _connection = new SqliteConnection("DataSource=:memory:");
            _connection.Open();

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlite(_connection));

            var sp = services.BuildServiceProvider();
            using var scope = sp.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            db.Database.EnsureCreated();
        });
    }

    /// <summary>
    /// Creates a client with its own cookie jar so authentication
    /// cookies persist across requests made with that client.
    /// </summary>
    public HttpClient CreateAuthenticatedClient()
    {
        return CreateDefaultClient(new CookieContainerHandler(new CookieContainer()));
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        if (disposing)
        {
            _connection?.Dispose();
        }
    }
}
