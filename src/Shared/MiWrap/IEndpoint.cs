using Microsoft.AspNetCore.Routing;

namespace Shared.MiWrap;

public interface IEndpoint
{
    void RegisterEndpoint(IEndpointRouteBuilder builder);
}