// using System.Data;
// using Dapper;
// using Shared.MiWrap;
//
// namespace Queries.Features.Products;
//
// public record GetProducts : IHttpQuery;
//
//
// public class GetProductsEndpoint : IEndpoint
// {
//     public void RegisterEndpoint(IEndpointRouteBuilder builder) =>
//         builder.MapGet<GetProducts, GetProductsHandler>("")
//             .Produces(200);
// }
//
// internal class GetProductsHandler : IHttpQueryHandler<GetProducts>
// {
//     private readonly IDbConnection _connection;
//     public GetProductsHandler(IDbConnection connection)
//     {
//         _connection = connection;
//     }
//
//     public async Task<IResult> HandleAsync(GetProducts query, CancellationToken cancellationToken) =>
//         await _connection.QueryAsync<Person>("select * from People");
// }