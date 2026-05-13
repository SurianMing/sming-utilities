using Microsoft.Extensions.DependencyInjection;

namespace SmingCode.Utilities.ServiceApiClient.Config;

public interface IServiceApiClientConfigurationBuilder<T> where T : class
{ }

internal class ServiceApiClientConfigurationBuilder<TService>(
    string _targetServiceDisplayName,
    string _targetServiceName,
    IServiceCollection _services
) : IServiceApiClientConfigurationBuilder<TService> where TService : class
{
    private readonly string _clientSpecificServiceKey = typeof(TService).Name;
    private Type[] _clientSpecificTypesRegistered = [];

    internal ApiClientDetail<TService> BuildApiClientDetail()
        => new(
            _targetServiceDisplayName,
            _targetServiceName,
            _clientSpecificServiceKey,
            _clientSpecificTypesRegistered
        );

    internal void AddApiClientSpecificMiddleware<TImplementation>()
    {
        _services.AddSingleton(new MiddlewareDetail(typeof(TImplementation), typeof(TService)));
    }

    internal void AddClientSpecificSingleton<T>() where T : class
    {
        _services.AddKeyedSingleton<T>(_clientSpecificServiceKey);

        _clientSpecificTypesRegistered = [
            .._clientSpecificTypesRegistered,
            typeof(T)
        ];
    }

    internal void AddClientSpecificSingleton<T>(T instance) where T : class
    {
        _services.AddKeyedSingleton(_clientSpecificServiceKey, instance);

        _clientSpecificTypesRegistered = [
            .._clientSpecificTypesRegistered,
            typeof(T)
        ];
    }

    internal void AddClientSpecificScoped<T>() where T : class
    {
        _services.AddKeyedScoped<T>(_clientSpecificServiceKey);

        _clientSpecificTypesRegistered = [
            .._clientSpecificTypesRegistered,
            typeof(T)
        ];
    }

    internal void AddGlobalSingleton<T>() where T : class
    {
        _services.AddSingleton<T>();
    }

    internal void AddGlobalSingleton<T>(T instance) where T : class
    {
        _services.AddSingleton(instance);
    }

    internal void AddGlobalScoped<T>() where T : class
    {
        _services.AddScoped<T>();
    }
}