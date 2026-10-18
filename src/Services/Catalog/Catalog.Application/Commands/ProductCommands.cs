using Catalog.Application.Dtos;
using MediatR;

namespace Catalog.Application.Commands;

public record CreateProductCommand(ProductRequest Product) : IRequest<ProductDto>;

public record UpdateProductCommand(string Id, ProductRequest Product) : IRequest<bool>;

public record DeleteProductCommand(string Id) : IRequest<bool>;
