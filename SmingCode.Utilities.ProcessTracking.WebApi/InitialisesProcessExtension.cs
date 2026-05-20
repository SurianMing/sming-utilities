using Microsoft.AspNetCore.Builder;

namespace SmingCode.Utilities.ProcessTracking.WebApi;
using Config;

public static class InitialisesProcessExtension
{
    public static RouteHandlerBuilder InitialisesProcess(
        this RouteHandlerBuilder routeHandlerBuilder,
        string processName
    )
    {
        routeHandlerBuilder.WithTags(
            $"{Constants.PROCESS_NAME_ENDPOINT_TAG}{processName}"
        );

        return routeHandlerBuilder;
    }
}