using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Identity.Client;

namespace SmingCode.Utilities.ServiceApiClient.MsalOAuth.Config;
using ServiceApiClient.Config;

public static class Injection
{
    public static IServiceApiClientConfigurationBuilder<TService> AddMsalOAuth<TService>(
        this IServiceApiClientConfigurationBuilder<TService> builder,
        IConfiguration configuration,
        string configurationEntryName
    ) where TService : class
    {
        if (builder is not ServiceApiClientConfigurationBuilder<TService> serviceApiClientConfigurationBuilder)
        {
            throw new Exception();
        }

        var clientId = configuration[$"Apis:{configurationEntryName}:MsalOAuth:ClientId"]
            ?? throw new InvalidOperationException(
                "Must add client id.."
            );
        var clientSecret = configuration[$"Apis:{configurationEntryName}:MsalOAuth:ClientSecret"]
            ?? throw new InvalidOperationException(
                "Must add client secret.."
            );
        var tokenScopes = configuration.GetSection($"Apis:{configurationEntryName}:MsalOAuth:TokenScopes")
            ?.Get<string[]>()
            ?? throw new InvalidOperationException(
                "Must add subscription key.."
            );
        var tenantId = configuration[$"Apis:{configurationEntryName}:MsalOAuth:TenantId"]
            ?? throw new InvalidOperationException(
                "Must add api version.."
            );

        var options = new MsalOAuthClientOptions(
            tokenScopes,
            tenantId
        );
        serviceApiClientConfigurationBuilder.AddClientSpecificSingleton(options);
        var confidentialClientApplication = ConfidentialClientApplicationBuilder
            .Create(clientId)
            .WithClientSecret(clientSecret)
            .Build();
        serviceApiClientConfigurationBuilder.AddClientSpecificSingleton(confidentialClientApplication);

        serviceApiClientConfigurationBuilder.AddApiClientSpecificMiddleware<MsalOAuthMiddleware>();

        return builder;
    }
}
