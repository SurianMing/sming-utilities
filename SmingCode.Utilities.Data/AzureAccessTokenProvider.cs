using Azure.Identity;

namespace SmingCode.Utilities.Data;

internal static class AzureAccessTokenProvider
{
    public static async Task<string> GetTokenForDefaultCredentialAsync()
    {
        var identity = new DefaultAzureCredential();
        var token = await identity.GetTokenAsync(
            new Azure.Core.TokenRequestContext(
                [
                    "https://database.windows.net/.default"
                ]
            )
        );

        return token.Token;
    }

    public static async Task<string> GetTokenForManagedIdentityAsync(string managedIdentityClientId)
    {
        var identity = new DefaultAzureCredential(new DefaultAzureCredentialOptions
        {
            ManagedIdentityClientId = managedIdentityClientId
        });
        var token = await identity.GetTokenAsync(
            new Azure.Core.TokenRequestContext(
                [
                    "https://database.windows.net/.default"
                ]
            )
        );

        return token.Token;
    }
}