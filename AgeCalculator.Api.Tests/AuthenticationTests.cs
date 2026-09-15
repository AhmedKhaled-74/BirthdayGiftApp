using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace AgeCalculator.Api.Tests;

public class AuthenticationTests(TestingWebApplicationFactory factory)
    : IClassFixture<TestingWebApplicationFactory>
{
    private static string UniqueEmail() => $"user-{Guid.NewGuid():N}@example.com";

    private static JsonContent Json(object value) => JsonContent.Create(value);

    [Fact]
    public async Task Register_ValidRequest_ReturnsOkWithUser()
    {
        var client = factory.CreateAuthenticatedClient();
        var email = UniqueEmail();

        var response = await client.PostAsync(
            "/api/auth/register", Json(new { email, password = "Test123!" }));

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<JsonElement>();
        Assert.Equal(email, body.GetProperty("email").GetString());
        Assert.False(string.IsNullOrWhiteSpace(body.GetProperty("id").GetString()));
    }

    [Fact]
    public async Task Register_DuplicateEmail_ReturnsBadRequest()
    {
        var client = factory.CreateAuthenticatedClient();
        var email = UniqueEmail();

        var first = await client.PostAsync(
            "/api/auth/register", Json(new { email, password = "Test123!" }));
        Assert.Equal(HttpStatusCode.OK, first.StatusCode);

        var second = await client.PostAsync(
            "/api/auth/register", Json(new { email, password = "Test123!" }));
        Assert.Equal(HttpStatusCode.BadRequest, second.StatusCode);
    }

    [Fact]
    public async Task Register_WeakPassword_ReturnsBadRequest()
    {
        var client = factory.CreateAuthenticatedClient();

        var response = await client.PostAsync(
            "/api/auth/register", Json(new { email = UniqueEmail(), password = "password" }));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Login_ValidCredentials_AuthenticatesCurrentUser()
    {
        var client = factory.CreateAuthenticatedClient();
        var email = UniqueEmail();
        await client.PostAsync("/api/auth/register", Json(new { email, password = "Test123!" }));
        await client.PostAsync("/api/auth/logout", Json(new { }));

        var login = await client.PostAsync(
            "/api/auth/login", Json(new { email, password = "Test123!" }));
        Assert.Equal(HttpStatusCode.OK, login.StatusCode);

        var current = await client.GetAsync("/api/auth/current");
        Assert.Equal(HttpStatusCode.OK, current.StatusCode);
        var body = await current.Content.ReadFromJsonAsync<JsonElement>();
        Assert.Equal(email, body.GetProperty("email").GetString());
    }

    [Fact]
    public async Task Login_InvalidPassword_ReturnsUnauthorized()
    {
        var client = factory.CreateAuthenticatedClient();
        var email = UniqueEmail();
        await client.PostAsync("/api/auth/register", Json(new { email, password = "Test123!" }));

        var response = await client.PostAsync(
            "/api/auth/login", Json(new { email, password = "Wrong123!" }));

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Current_Unauthenticated_ReturnsUnauthorized()
    {
        var client = factory.CreateAuthenticatedClient();

        var response = await client.GetAsync("/api/auth/current");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Logout_ClearsSession()
    {
        var client = factory.CreateAuthenticatedClient();
        var email = UniqueEmail();
        await client.PostAsync("/api/auth/register", Json(new { email, password = "Test123!" }));
        await client.PostAsync("/api/auth/login", Json(new { email, password = "Test123!" }));

        var logout = await client.PostAsync("/api/auth/logout", Json(new { }));
        Assert.Equal(HttpStatusCode.NoContent, logout.StatusCode);

        var current = await client.GetAsync("/api/auth/current");
        Assert.Equal(HttpStatusCode.Unauthorized, current.StatusCode);
    }
}
