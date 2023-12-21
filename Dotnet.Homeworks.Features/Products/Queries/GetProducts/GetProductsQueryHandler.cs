using Dotnet.Homeworks.Domain.Abstractions.Repositories;
using Dotnet.Homeworks.Infrastructure.Cqrs.Queries;
using Dotnet.Homeworks.Shared.Dto;

namespace Dotnet.Homeworks.Features.Products.Queries.GetProducts;

internal sealed class GetProductsQueryHandler : IQueryHandler<GetProductsQuery, GetProductsDto> 
{
    private readonly IProductRepository _productRepository;

    public GetProductsQueryHandler(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<Result<GetProductsDto>> Handle(GetProductsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var products = await _productRepository
                .GetAllProductsAsync(cancellationToken);
            var prodDtos = products
                .Select(prod => new GetProductDto(prod.Id, prod.Name));
            return new Result<GetProductsDto>(new GetProductsDto(prodDtos), true);
        }
        catch (Exception e)
        {
            return new Result<GetProductsDto>(null, false, e.Message);
        }
    }
}