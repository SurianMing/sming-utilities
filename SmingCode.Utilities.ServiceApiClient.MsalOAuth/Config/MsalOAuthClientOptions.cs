namespace SmingCode.Utilities.ServiceApiClient.MsalOAuth.Config;

internal record MsalOAuthClientOptions(
    string[] TokenScopes,
    string TenantId
);
