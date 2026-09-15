using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace AgeCalculator.Api.Tests;

public class WelcomeSummaryTests(TestingWebApplicationFactory factory)
    : IClassFixture<TestingWebApplicationFactory>
{
    private static string UniqueEmail() => $"user-{Guid.NewGuid():N}@example.com";

    private static JsonContent Json(object value) => JsonContent.Create(value);

    private async Task<HttpClient> RegisterAndLoginAsync(string email)
    {
        var client = factory.CreateAuthenticatedClient();
        var register = await client.PostAsync(
            "/api/auth/register", Json(new { email, password = "Test123!" }));
        register.EnsureSuccessStatusCode();
        var login = await client.PostAsync(
            "/api/auth/login", Json(new { email, password = "Test123!" }));
        login.EnsureSuccessStatusCode();
        return client;
    }

    [Fact]
    public async Task Summary_Unauthenticated_ReturnsUnauthorized()
    {
        var client = factory.CreateAuthenticatedClient();

        var response = await client.GetAsync("/api/welcome/summary?timezone=UTC");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Summary_WithoutBirthDate_ReturnsNotFound()
    {
        var client = await RegisterAndLoginAsync(UniqueEmail());

        var response = await client.GetAsync("/api/welcome/summary?timezone=UTC");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Summary_WithBirthDate_ReturnsAgeSummary()
    {
        var client = await RegisterAndLoginAsync(UniqueEmail());
        var birthDate = DateTime.Today.AddYears(-30).ToString("yyyy-MM-dd");
        var update = await client.PutAsync(
            "/api/profile/birth-date", Json(new { birthDate }));
        update.EnsureSuccessStatusCode();

        var response = await client.GetAsync("/api/welcome/summary?timezone=UTC");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<JsonElement>();
        Assert.Equal(30, body.GetProperty("years").GetInt32());
        Assert.True(body.GetProperty("months").GetInt32() >= 0);
        Assert.True(body.GetProperty("days").GetInt32() >= 0);
        Assert.True(body.GetProperty("daysUntilNextBirthday").GetInt32() >= 0);
        Assert.Equal(JsonValueKind.True, body.GetProperty("isBirthday").ValueKind);
    }

    [Fact]
    public async Task Summary_InvalidTimezone_ReturnsBadRequest()
    {
        var client = await RegisterAndLoginAsync(UniqueEmail());

        var response = await client.GetAsync("/api/welcome/summary?timezone=Not/AZone");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Summary_MissingTimezone_ReturnsBadRequest()
    {
        var client = await RegisterAndLoginAsync(UniqueEmail());

        var response = await client.GetAsync("/api/welcome/summary");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}
