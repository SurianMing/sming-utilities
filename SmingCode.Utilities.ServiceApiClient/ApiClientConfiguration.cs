namespace SmingCode.Utilities.ServiceApiClient;

internal record ApiClientConfiguration<TService>(
    string ServiceDisplayName,
    string ServiceName
);
