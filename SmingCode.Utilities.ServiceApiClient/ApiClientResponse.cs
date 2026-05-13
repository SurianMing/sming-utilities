namespace SmingCode.Utilities.ServiceApiClient;

public record ApiClientResponse(
    int ResponseCode,
    HeaderEntryCollection Headers
);

public record ApiClientResponse<TBody>(
    int ResponseCode,
    HeaderEntryCollection Headers,
    TBody Body
) : ApiClientResponse(
    ResponseCode,
    Headers
);