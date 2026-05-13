namespace SmingCode.Utilities.ServiceApiClient.Config;

internal record MiddlewareDetail(
    Type MiddlewareImplementation,
    Type? ServiceType = null
);
