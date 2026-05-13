using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace SmingCode.Utilities.Nullability;

public static class NullabilityHelper
{
    private static readonly NullabilityInfoContext _nullabilityContext = new();

    public static IServiceCollection AddOptionsWithNullCheck<T>(
        this IServiceCollection services,
        IConfiguration configuration,
        string configSectionName
    ) where T : class
    {
        var configSection = configuration.GetRequiredSection(configSectionName);

        PerformNullCheck<T>(configSection);
        
        services.AddOptions<T>()
            .BindConfiguration(configSectionName);

        return services;
    }

    public static T GetOptionsWithNullCheck<T>(
        this IConfiguration configuration,
        string configSectionName
    ) where T : class
    {
        var configSection = configuration.GetRequiredSection(configSectionName);

        PerformNullCheck<T>(configSection);
        
        var options = configSection
            .Get<T>()!;

        return options;
    }

    public static T AddAndReturnOptionsWithNullCheck<T>(
        this IServiceCollection services,
        IConfiguration configuration,
        string configSectionName
    ) where T : class
    {
        var configSection = configuration.GetRequiredSection(configSectionName);

        PerformNullCheck<T>(configSection);

        services.AddOptions<T>()
            .BindConfiguration(configSectionName);
        var options = configSection
            .Get<T>()!;

        return options;
    }

    private static void PerformNullCheck<T>(
        this IConfigurationSection configSection
    ) where T : class
    {
        var invalidNullables = typeof(T)
            .GetProperties(
                BindingFlags.Public |
                BindingFlags.Instance
            )
            .Select(
                prop => new
                {
                    PropName = prop.Name,
                    NullabilityStateNotNullable = _nullabilityContext.Create(prop).WriteState is not NullabilityState.Nullable
                }
            )
            .Where(
                propDetails => propDetails.PropName != "EqualityContract"
                    && propDetails.NullabilityStateNotNullable
                    && string.IsNullOrEmpty(configSection[propDetails.PropName]?.ToString())
            )
            .ToArray();

        if (invalidNullables.Length != 0)
        {
            var propertyNames = string.Join(", ", invalidNullables.Select(prop => prop.PropName));

            throw new InvalidOperationException(
                $"Properties '{propertyNames}' are decorated as non-nullable but have no matching values passed."
            );
        }
    }
}
