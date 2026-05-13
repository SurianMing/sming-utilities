using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Shouldly;
using SmingCode.Utilities.Testing.AutoDomainData;

namespace SmingCode.Utilities.Nullability.Tests;

public class NullabilityHelperTests
{
    [Theory]
    [AutoDomainData]
    public void AddOptionsWithNullCheck_ShouldThrow_InvalidOperationException_IfNonNullableStringPropertyHasEmptyMatchedConfigEntry(
        int requiredNative
    )
    {
        // Arrange
        var configurationManager = new ConfigurationManager();
        configurationManager.AddInMemoryCollection([
            new KeyValuePair<string, string?>($"Kafka:{nameof(NullableTestClass.RequiredString)}", ""),
            new KeyValuePair<string, string?>($"Kafka:{nameof(NullableTestClass.RequiredNative)}", requiredNative.ToString())
        ]);

        var host = Host.CreateApplicationBuilder(new HostApplicationBuilderSettings
        {
            Configuration = configurationManager
        });

        // Act
        var addOptionsWithNullCheckCall = () => host.Services.AddOptionsWithNullCheck<NullableTestClass>(host.Configuration, "Kafka");

        // Assert
        addOptionsWithNullCheckCall.ShouldThrow<InvalidOperationException>();
    }

    [Theory]
    [AutoDomainData]
    public void AddOptionsWithNullCheck_ShouldThrow_InvalidOperationException_IfNonNullableStringPropertyHasNoMatchedConfigEntry(
        int requiredNative
    )
    {
        // Arrange
        var configurationManager = new ConfigurationManager();
        configurationManager.AddInMemoryCollection([
            new KeyValuePair<string, string?>($"Kafka:{nameof(NullableTestClass.RequiredNative)}", requiredNative.ToString())
        ]);

        var host = Host.CreateApplicationBuilder(new HostApplicationBuilderSettings
        {
            Configuration = configurationManager
        });

        // Act
        var addOptionsWithNullCheckCall = () => host.Services.AddOptionsWithNullCheck<NullableTestClass>(host.Configuration, "Kafka");

        // Assert
        addOptionsWithNullCheckCall.ShouldThrow<InvalidOperationException>();
    }

    [Theory]
    [AutoDomainData]
    public void AddOptionsWithNullCheck_ShouldThrow_InvalidOperationException_IfNonNullableNativePropertyHasEmptyMatchedConfigEntry(
        string requiredString
    )
    {
        // Arrange
        var configurationManager = new ConfigurationManager();
        configurationManager.AddInMemoryCollection([
            new KeyValuePair<string, string?>($"Kafka:{nameof(NullableTestClass.RequiredString)}", requiredString),
            new KeyValuePair<string, string?>($"Kafka:{nameof(NullableTestClass.RequiredNative)}", "")
        ]);

        var host = Host.CreateApplicationBuilder(new HostApplicationBuilderSettings
        {
            Configuration = configurationManager
        });

        // Act
        var addOptionsWithNullCheckCall = () => host.Services.AddOptionsWithNullCheck<NullableTestClass>(host.Configuration, "Kafka");

        // Assert
        addOptionsWithNullCheckCall.ShouldThrow<InvalidOperationException>();
    }

    [Theory]
    [AutoDomainData]
    public void AddOptionsWithNullCheck_ShouldThrow_InvalidOperationException_IfNonNullableNativePropertyHasNoMatchedConfigEntry(
        string requiredString
    )
    {
        // Arrange
        var configurationManager = new ConfigurationManager();
        configurationManager.AddInMemoryCollection([
            new KeyValuePair<string, string?>($"Kafka:{nameof(NullableTestClass.RequiredString)}", requiredString),
        ]);

        var host = Host.CreateApplicationBuilder(new HostApplicationBuilderSettings
        {
            Configuration = configurationManager
        });

        // Act
        var addOptionsWithNullCheckCall = () => host.Services.AddOptionsWithNullCheck<NullableTestClass>(host.Configuration, "Kafka");

        // Assert
        addOptionsWithNullCheckCall.ShouldThrow<InvalidOperationException>();
    }

    [Theory]
    [AutoDomainData]
    public void AddOptionsWithNullCheck_ShouldAddInstanceOfTypeToOptions_WithCorrectConfigValues_IfAllRequiredValuesAreMatched(
        string requiredString,
        int requiredNative
    )
    {
        // Arrange
        var configurationManager = new ConfigurationManager();
        configurationManager.AddInMemoryCollection([
            new KeyValuePair<string, string?>($"Kafka:{nameof(NullableTestClass.RequiredString)}", requiredString),
            new KeyValuePair<string, string?>($"Kafka:{nameof(NullableTestClass.RequiredNative)}", requiredNative.ToString()),
        ]);

        var host = Host.CreateApplicationBuilder(new HostApplicationBuilderSettings
        {
            Configuration = configurationManager
        });

        // Act
        _ = host.Services.AddOptionsWithNullCheck<NullableTestClass>(host.Configuration, "Kafka");

        // Assert
        var app = host.Build();
        var nullableOptions = app.Services.GetRequiredService<IOptions<NullableTestClass>>();
        nullableOptions.Value.RequiredNative.ShouldBe(requiredNative);
        nullableOptions.Value.RequiredString.ShouldBe(requiredString);
    }

    [Theory]
    [AutoDomainData]
    public void AddAndReturnOptionsWithNullCheck_ShouldThrow_InvalidOperationException_IfNonNullableStringPropertyHasEmptyMatchedConfigEntry(
        int requiredNative
    )
    {
        // Arrange
        var configurationManager = new ConfigurationManager();
        configurationManager.AddInMemoryCollection([
            new KeyValuePair<string, string?>($"Kafka:{nameof(NullableTestClass.RequiredString)}", ""),
            new KeyValuePair<string, string?>($"Kafka:{nameof(NullableTestClass.RequiredNative)}", requiredNative.ToString())
        ]);

        var host = Host.CreateApplicationBuilder(new HostApplicationBuilderSettings
        {
            Configuration = configurationManager
        });

        // Act
        var addOptionsWithNullCheckCall = () => host.Services.AddAndReturnOptionsWithNullCheck<NullableTestClass>(host.Configuration, "Kafka");

        // Assert
        addOptionsWithNullCheckCall.ShouldThrow<InvalidOperationException>();
    }

    [Theory]
    [AutoDomainData]
    public void AddAndReturnOptionsWithNullCheck_ShouldThrow_InvalidOperationException_IfNonNullableStringPropertyHasNoMatchedConfigEntry(
        int requiredNative
    )
    {
        // Arrange
        var configurationManager = new ConfigurationManager();
        configurationManager.AddInMemoryCollection([
            new KeyValuePair<string, string?>($"Kafka:{nameof(NullableTestClass.RequiredNative)}", requiredNative.ToString())
        ]);

        var host = Host.CreateApplicationBuilder(new HostApplicationBuilderSettings
        {
            Configuration = configurationManager
        });

        // Act
        var addOptionsWithNullCheckCall = () => host.Services.AddAndReturnOptionsWithNullCheck<NullableTestClass>(host.Configuration, "Kafka");

        // Assert
        addOptionsWithNullCheckCall.ShouldThrow<InvalidOperationException>();
    }

    [Theory]
    [AutoDomainData]
    public void AddAndReturnOptionsWithNullCheck_ShouldThrow_InvalidOperationException_IfNonNullableNativePropertyHasEmptyMatchedConfigEntry(
        string requiredString
    )
    {
        // Arrange
        var configurationManager = new ConfigurationManager();
        configurationManager.AddInMemoryCollection([
            new KeyValuePair<string, string?>($"Kafka:{nameof(NullableTestClass.RequiredString)}", requiredString),
            new KeyValuePair<string, string?>($"Kafka:{nameof(NullableTestClass.RequiredNative)}", "")
        ]);

        var host = Host.CreateApplicationBuilder(new HostApplicationBuilderSettings
        {
            Configuration = configurationManager
        });

        // Act
        var addOptionsWithNullCheckCall = () => host.Services.AddAndReturnOptionsWithNullCheck<NullableTestClass>(host.Configuration, "Kafka");

        // Assert
        addOptionsWithNullCheckCall.ShouldThrow<InvalidOperationException>();
    }

    [Theory]
    [AutoDomainData]
    public void AddAndReturnOptionsWithNullCheck_ShouldThrow_InvalidOperationException_IfNonNullableNativePropertyHasNoMatchedConfigEntry(
        string requiredString
    )
    {
        // Arrange
        var configurationManager = new ConfigurationManager();
        configurationManager.AddInMemoryCollection([
            new KeyValuePair<string, string?>($"Kafka:{nameof(NullableTestClass.RequiredString)}", requiredString),
        ]);

        var host = Host.CreateApplicationBuilder(new HostApplicationBuilderSettings
        {
            Configuration = configurationManager
        });

        // Act
        var addOptionsWithNullCheckCall = () => host.Services.AddAndReturnOptionsWithNullCheck<NullableTestClass>(host.Configuration, "Kafka");

        // Assert
        addOptionsWithNullCheckCall.ShouldThrow<InvalidOperationException>();
    }

    [Theory]
    [AutoDomainData]
    public void AddAndReturnOptionsWithNullCheck_ShouldAddInstanceOfTypeToOptions_AndReturnInstance_WithCorrectConfigValues_IfAllRequiredValuesAreMatched(
        string requiredString,
        int requiredNative
    )
    {
        // Arrange
        var configurationManager = new ConfigurationManager();
        configurationManager.AddInMemoryCollection([
            new KeyValuePair<string, string?>($"Kafka:{nameof(NullableTestClass.RequiredString)}", requiredString),
            new KeyValuePair<string, string?>($"Kafka:{nameof(NullableTestClass.RequiredNative)}", requiredNative.ToString()),
        ]);

        var host = Host.CreateApplicationBuilder(new HostApplicationBuilderSettings
        {
            Configuration = configurationManager
        });

        // Act
        var result = host.Services.AddAndReturnOptionsWithNullCheck<NullableTestClass>(host.Configuration, "Kafka");

        // Assert
        result.RequiredNative.ShouldBe(requiredNative);
        result.RequiredString.ShouldBe(requiredString);

        var app = host.Build();
        var nullableOptions = app.Services.GetRequiredService<IOptions<NullableTestClass>>();
        nullableOptions.Value.RequiredNative.ShouldBe(requiredNative);
        nullableOptions.Value.RequiredString.ShouldBe(requiredString);
    }

    [Theory]
    [AutoDomainData]
    public void GetOptionsWithNullCheck_ShouldThrow_InvalidOperationException_IfNonNullableStringPropertyHasEmptyMatchedConfigEntry(
        int requiredNative
    )
    {
        // Arrange
        var configurationManager = new ConfigurationManager();
        configurationManager.AddInMemoryCollection([
            new KeyValuePair<string, string?>($"Kafka:{nameof(NullableTestClass.RequiredString)}", ""),
            new KeyValuePair<string, string?>($"Kafka:{nameof(NullableTestClass.RequiredNative)}", requiredNative.ToString())
        ]);

        var host = Host.CreateApplicationBuilder(new HostApplicationBuilderSettings
        {
            Configuration = configurationManager
        });

        // Act
        var addOptionsWithNullCheckCall = () => host.Configuration.GetOptionsWithNullCheck<NullableTestClass>("Kafka");

        // Assert
        addOptionsWithNullCheckCall.ShouldThrow<InvalidOperationException>();
    }

    [Theory]
    [AutoDomainData]
    public void GetOptionsWithNullCheck_ShouldThrow_InvalidOperationException_IfNonNullableStringPropertyHasNoMatchedConfigEntry(
        int requiredNative
    )
    {
        // Arrange
        var configurationManager = new ConfigurationManager();
        configurationManager.AddInMemoryCollection([
            new KeyValuePair<string, string?>($"Kafka:{nameof(NullableTestClass.RequiredNative)}", requiredNative.ToString())
        ]);

        var host = Host.CreateApplicationBuilder(new HostApplicationBuilderSettings
        {
            Configuration = configurationManager
        });

        // Act
        var addOptionsWithNullCheckCall = () => host.Configuration.GetOptionsWithNullCheck<NullableTestClass>("Kafka");

        // Assert
        addOptionsWithNullCheckCall.ShouldThrow<InvalidOperationException>();
    }

    [Theory]
    [AutoDomainData]
    public void GetOptionsWithNullCheck_ShouldThrow_InvalidOperationException_IfNonNullableNativePropertyHasEmptyMatchedConfigEntry(
        string requiredString
    )
    {
        // Arrange
        var configurationManager = new ConfigurationManager();
        configurationManager.AddInMemoryCollection([
            new KeyValuePair<string, string?>($"Kafka:{nameof(NullableTestClass.RequiredString)}", requiredString),
            new KeyValuePair<string, string?>($"Kafka:{nameof(NullableTestClass.RequiredNative)}", "")
        ]);

        var host = Host.CreateApplicationBuilder(new HostApplicationBuilderSettings
        {
            Configuration = configurationManager
        });

        // Act
        var addOptionsWithNullCheckCall = () => host.Configuration.GetOptionsWithNullCheck<NullableTestClass>("Kafka");

        // Assert
        addOptionsWithNullCheckCall.ShouldThrow<InvalidOperationException>();
    }

    [Theory]
    [AutoDomainData]
    public void GetOptionsWithNullCheck_ShouldThrow_InvalidOperationException_IfNonNullableNativePropertyHasNoMatchedConfigEntry(
        string requiredString
    )
    {
        // Arrange
        var configurationManager = new ConfigurationManager();
        configurationManager.AddInMemoryCollection([
            new KeyValuePair<string, string?>($"Kafka:{nameof(NullableTestClass.RequiredString)}", requiredString),
        ]);

        var host = Host.CreateApplicationBuilder(new HostApplicationBuilderSettings
        {
            Configuration = configurationManager
        });

        // Act
        var addOptionsWithNullCheckCall = () => host.Configuration.GetOptionsWithNullCheck<NullableTestClass>("Kafka");

        // Assert
        addOptionsWithNullCheckCall.ShouldThrow<InvalidOperationException>();
    }

    [Theory]
    [AutoDomainData]
    public void GetOptionsWithNullCheck_ShouldReturn_InstanceOfType_WithCorrectConfigValues_IfAllRequiredValuesAreMatched(
        string requiredString,
        int requiredNative
    )
    {
        // Arrange
        var configurationManager = new ConfigurationManager();
        configurationManager.AddInMemoryCollection([
            new KeyValuePair<string, string?>($"Kafka:{nameof(NullableTestClass.RequiredString)}", requiredString),
            new KeyValuePair<string, string?>($"Kafka:{nameof(NullableTestClass.RequiredNative)}", requiredNative.ToString()),
        ]);

        var host = Host.CreateApplicationBuilder(new HostApplicationBuilderSettings
        {
            Configuration = configurationManager
        });

        // Act
        var result = host.Configuration.GetOptionsWithNullCheck<NullableTestClass>("Kafka");

        // Assert
        result.RequiredString.ShouldBe(requiredString);
        result.RequiredNative.ShouldBe(requiredNative);
    }

    internal class NullableTestClass
    {
        public required string RequiredString { get; set; }
        public required int RequiredNative { get; set; }
        public string? OptionalString { get; set; }
        public int? OptionalNative { get; set; }
    }
}
