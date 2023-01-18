// using SimpleApp.Common;
// using SimpleApp.Entities;
// using SimpleApp.Persistence;
//
// namespace SimpleApp.Features.Products;
//
// internal record AddProduct(AddProduct.AddProductBody Body) : IHttpCommand
// {
//     internal record AddProductBody(string Name, decimal Price);
// }
//
// public class AddProductEndpoint : IEndpoint
// {
//     public void RegisterEndpoint(IEndpointRouteBuilder builder) =>
//         builder.MapPost<AddProduct, AddProductHandler>("brands")
//             .Produces(201);
// }
//
// internal class AddProductHandler : IHttpCommandHandler<AddProduct>
// {
//     private readonly InMemoryDb _db;
//
//     public AddProductHandler(InMemoryDb db) => _db = db;
//
//     public Task<IResult> HandleAsync(AddProduct command, CancellationToken cancellationToken = default)
//     {
//         var product = new Product(Guid.NewGuid(), command.Body.Name, command.Body.Price);
//
//         _db.Add(product);
//         return Task.FromResult(Results.CreatedAtRoute(nameof(GetProduct),new {product.Id}));
//     }
// }