using Catalog.Application.Commands;
using Catalog.Application.Dtos;
using Catalog.Core.Entities;
using Catalog.Core.Repositories;
using MediatR;

namespace Catalog.Application.Handlers;

public class CreateProductHandler(IProductRepository repository) : IRequestHandler<CreateProductCommand, ProductDto>
{
    public async Task<ProductDto> Handle(CreateProductCommand command, CancellationToken cancellationToken)
    {
        var request = command.Product;
        var product = new Product
        {
            Name = request.Name,
            Description = request.Description,
            Category = request.Category,
            Price = request.Price,
            Stock = request.Stock
        };

        var created = await repository.CreateAsync(product, cancellationToken);
        return created.ToDto();
    }
}

public class UpdateProductHandler(IProductRepository repository) : IRequestHandler<UpdateProductCommand, bool>
{
    public Task<bool> Handle(UpdateProductCommand command, CancellationToken cancellationToken)
    {
        var request = command.Product;
        var product = new Product
        {
            Id = command.Id,
            Name = request.Name,
            Description = request.Description,
            Category = request.Category,
            Price = request.Price,
            Stock = request.Stock
        };

        return repository.UpdateAsync(product, cancellationToken);
    }
}

public class DeleteProductHandler(IProductRepository repository) : IRequestHandler<DeleteProductCommand, bool>
{
    public Task<bool> Handle(DeleteProductCommand command, CancellationToken cancellationToken) =>
        repository.DeleteAsync(command.Id, cancellationToken);
}
