// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.


using CleanArchitecture.Blazor.Application.Features.Products.Caching;
using CleanArchitecture.Blazor.Application.Features.Products.DTOs;
using CleanArchitecture.Blazor.Application.Features.Products.Queries.Specification;

namespace CleanArchitecture.Blazor.Application.Features.Products.Queries.Pagination;

public class ProductsWithPaginationQuery : PaginationFilterBase, ICacheableRequest<PaginatedData<ProductDto>>
{
    public string? Name { get; set; }
    public string? Brand { get; set; }
    public string? Unit { get; set; }
    public Range<decimal> Price { get; set; } = new();
    [CompareTo("Name", "Brand", "Description")] // <-- This filter will be applied to Name or Brand or Description.
    [StringFilterOptions(StringFilterOption.Contains)]
    public string? Keyword { get; set; }
    public override string ToString()
    {
        return $"Search:{Keyword},Name:{Name},Brand:{Brand},Unit:{Unit},MinPrice:{Price?.Min},MaxPrice:{Price?.Max},Sort:{Sort},SortBy:{SortBy},{Page},{PerPage}";
    }
    public string CacheKey => ProductCacheKey.GetPaginationCacheKey($"{this}");
    public MemoryCacheEntryOptions? Options => ProductCacheKey.MemoryCacheEntryOptions;
}

public class ProductsWithPaginationQueryHandler :
         IRequestHandler<ProductsWithPaginationQuery, PaginatedData<ProductDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IStringLocalizer<ProductsWithPaginationQueryHandler> _localizer;

    public ProductsWithPaginationQueryHandler(
        IApplicationDbContext context,
        IMapper mapper,
        IStringLocalizer<ProductsWithPaginationQueryHandler> localizer
        )
    {
        _context = context;
        _mapper = mapper;
        _localizer = localizer;
    }

    public async Task<PaginatedData<ProductDto>> Handle(ProductsWithPaginationQuery request, CancellationToken cancellationToken)
    {
        var data = await _context.Products.ApplyFilterWithoutPagination(request)
             .ProjectTo<ProductDto>(_mapper.ConfigurationProvider)
             .PaginatedDataAsync(request.Page, request.PerPage);
        return data;
    }
}