using Microsoft.Identity.Client;

namespace SmingCode.Utilities.ServiceApiClient.MsalOAuth;
using Microsoft.Extensions.Logging;
using Config;
using ServiceApiClient;

internal class MsalOAuthMiddleware(
    SendDelegate _next,
    MsalOAuthClientOptions _msalOAuthClientOptions,
    IConfidentialClientApplication confidentialClientApplication,
    ILogger<MsalOAuthMiddleware> _logger
)
{
    private const int TOKEN_EXPIRY_MARGIN_SECONDS = 5;
    private DateTimeOffset _tokenExpiry = DateTimeOffset.UtcNow;
    private string _accessToken = string.Empty;

    public async Task HandleAsync(
        ApiClientSendContext context
    )
    {
        if (_logger.IsEnabled(LogLevel.Trace))
        {
            _logger.LogTrace(
                "Adding OAuth header to http message - {TraceType}",
                Constants.UTILITY_TRACE_TYPE
            );
        }

        if (_tokenExpiry.AddSeconds(-TOKEN_EXPIRY_MARGIN_SECONDS) < DateTimeOffset.UtcNow)
        {
            var response = await confidentialClientApplication
                .AcquireTokenForClient(_msalOAuthClientOptions.TokenScopes)
                .WithTenantId(_msalOAuthClientOptions.TenantId)
                .ExecuteAsync();

            _tokenExpiry = response.ExpiresOn.UtcDateTime;
            _accessToken = response.AccessToken;
        }

        context.MessageHeaders.Add("Authorization", $"Bearer {_accessToken}");

        await _next(context);
    }
}
