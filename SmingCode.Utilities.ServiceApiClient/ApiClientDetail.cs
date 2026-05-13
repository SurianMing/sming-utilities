using System.Text.Json;

namespace SmingCode.Utilities.ServiceApiClient;

internal record ApiClientDetail<TService>(
    string ServiceDisplayName,
    string ServiceName,
    string ClientSpecificServiceKey,
    Type[] ClientSpecificRegisteredTypes
) : ApiClientDetail(
    ServiceDisplayName,
    ServiceName,
    ClientSpecificServiceKey,
    ClientSpecificRegisteredTypes,
    JsonSerializerOptions.Web
) where TService : class;

internal record ApiClientDetail(
    string ServiceDisplayName,
    string ServiceName,
    string ClientSpecificServiceKey,
    Type[] ClientSpecificRegisteredTypes,
    JsonSerializerOptions JsonSerializerOptions
);
