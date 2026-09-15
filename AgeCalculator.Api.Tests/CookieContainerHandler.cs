using System.Net;

namespace AgeCalculator.Api.Tests;

/// <summary>
/// Persists Set-Cookie responses into a CookieContainer and replays
/// matching cookies on subsequent requests.
/// </summary>
public sealed class CookieContainerHandler(CookieContainer container) : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var uri = request.RequestUri!;
        var cookieHeader = container.GetCookieHeader(uri);
        if (!string.IsNullOrEmpty(cookieHeader))
        {
            request.Headers.Remove("Cookie");
            request.Headers.TryAddWithoutValidation("Cookie", cookieHeader);
        }

        var response = await base.SendAsync(request, cancellationToken);

        if (response.Headers.TryGetValues("Set-Cookie", out var setCookies))
        {
            foreach (var setCookie in setCookies)
            {
                container.SetCookies(uri, setCookie);
            }
        }

        return response;
    }
}
