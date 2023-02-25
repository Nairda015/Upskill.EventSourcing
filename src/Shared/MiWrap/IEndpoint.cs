using Microsoft.AspNetCore.Routing;

namespace MiWrap;

public interface IEndpoint
{
    void RegisterEndpoint(IEndpointRouteBuilder builder);
}