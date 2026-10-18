using Catalog.Application.Dtos;
using MediatR;

namespace Catalog.Application.Queries;

public record GetProductsQuery : IRequest<IReadOnlyList<ProductDto>>;

public record GetProductByIdQuery(string Id) : IRequest<ProductDto?>;
