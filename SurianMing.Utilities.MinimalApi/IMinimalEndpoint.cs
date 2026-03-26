using Microsoft.AspNetCore.Builder;

namespace SurianMing.Utilities.MinimalApi;

public interface IMinimalEndpoint
{
    void MapEndpoint(WebApplication app);
}
