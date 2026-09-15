using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace AgeCalculator.Api.Tests;

public class ProfileTests(TestingWebApplicationFactory factory)
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
    public async Task GetProfile_Unauthenticated_ReturnsUnauthorized()
    {
        var client = factory.CreateAuthenticatedClient();

        var response = await client.GetAsync("/api/profile");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task UpdateBirthDate_Unauthenticated_ReturnsUnauthorized()
    {
        var client = factory.CreateAuthenticatedClient();

        var response = await client.PutAsync(
            "/api/profile/birth-date", Json(new { birthDate = "1990-05-15" }));

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetProfile_NewUser_ReturnsOwnProfileWithNullBirthDate()
    {
        var email = UniqueEmail();
        var client = await RegisterAndLoginAsync(email);

        var response = await client.GetAsync("/api/profile");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<JsonElement>();
        Assert.Equal(email, body.GetProperty("email").GetString());
        Assert.Equal(JsonValueKind.Null, body.GetProperty("birthDate").ValueKind);
    }

    [Fact]
    public async Task UpdateBirthDate_ValidDate_PersistsAndReturnsDate()
    {
        var client = await RegisterAndLoginAsync(UniqueEmail());

        var update = await client.PutAsync(
            "/api/profile/birth-date", Json(new { birthDate = "1990-05-15" }));

        Assert.Equal(HttpStatusCode.OK, update.StatusCode);

        var profile = await client.GetAsync("/api/profile");
        var body = await profile.Content.ReadFromJsonAsync<JsonElement>();
        Assert.StartsWith("1990-05-15", body.GetProperty("birthDate").GetString());
    }

    [Fact]
    public async Task UpdateBirthDate_FutureDate_ReturnsBadRequest()
    {
        var client = await RegisterAndLoginAsync(UniqueEmail());
        var future = DateTime.Today.AddDays(1).ToString("yyyy-MM-dd");

        var response = await client.PutAsync(
            "/api/profile/birth-date", Json(new { birthDate = future }));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Profile_IsIsolatedPerUser()
    {
        var alice = await RegisterAndLoginAsync(UniqueEmail());
        var bob = await RegisterAndLoginAsync(UniqueEmail());

        var aliceUpdate = await alice.PutAsync(
            "/api/profile/birth-date", Json(new { birthDate = "1990-01-01" }));
        aliceUpdate.EnsureSuccessStatusCode();

        // Bob only ever sees his own Profile: Alice's Birth Date must not leak.
        var bobProfile = await (await bob.GetAsync("/api/profile"))
            .Content.ReadFromJsonAsync<JsonElement>();
        Assert.Equal(JsonValueKind.Null, bobProfile.GetProperty("birthDate").ValueKind);

        var bobUpdate = await bob.PutAsync(
            "/api/profile/birth-date", Json(new { birthDate = "2000-02-29" }));
        bobUpdate.EnsureSuccessStatusCode();

        // Alice's Birth Date is untouched by Bob's update.
        var aliceProfile = await (await alice.GetAsync("/api/profile"))
            .Content.ReadFromJsonAsync<JsonElement>();
        Assert.StartsWith("1990-01-01", aliceProfile.GetProperty("birthDate").GetString());
    }
}
