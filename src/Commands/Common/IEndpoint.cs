using Microsoft.AspNetCore.Routing;

namespace Commands.Common;

public interface IEndpoint
{
    void RegisterEndpoint(IEndpointRouteBuilder builder);
}