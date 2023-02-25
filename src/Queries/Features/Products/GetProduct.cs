// using MiWrap.MiWrap;
//
// namespace Queries.Features.Products;
//
// internal record GetProduct(Guid Id) : IHttpQuery;
//
// public class GetProductEndpoint : IEndpoint
// {
//     public void RegisterEndpoint(IEndpointRouteBuilder builder) =>
//         builder.MapGet<GetProduct, GetProductHandler>("{id}")
//             .Produces(200)
//             .Produces(404);
// }
//
// internal class GetProductHandler : IHttpQueryHandler<GetProduct>
// {
//     private readonly InMemoryDb _db;
//
//     public GetProductHandler(InMemoryDb db) => _db = db;
//
//     public Task<IResult> HandleAsync(GetProduct query, CancellationToken cancellationToken)
//     {
//         if (_db.Products.TryGetValue(query.Id, out var product)) Task.FromResult(Results.Ok(product));
//
//         return Task.FromResult(Results.NotFound());
//     }
// }
//
// public class Test
// {
//     public async Task Get(IAmazonOpenSearchServerless client, string nextToken)
//     {
//         var filter = new CollectionFilters
//         {
//             Name = "test"
//
//         };
//         var req = new ListCollectionsRequest
//         {
//             CollectionFilters = filter,
//             MaxResults = 20,
//             NextToken = nextToken
//         };
//         var res = await client.ListCollectionsAsync(req);
//     }
// }