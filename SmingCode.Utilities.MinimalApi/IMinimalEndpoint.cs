using Microsoft.AspNetCore.Builder;

namespace SmingCode.Utilities.MinimalApi;

public interface IMinimalEndpoint
{
    void MapEndpoint(WebApplication app);
}
