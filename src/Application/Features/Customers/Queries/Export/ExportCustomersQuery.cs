﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.


using CleanArchitecture.Blazor.Application.Features.Customers.DTOs;

namespace CleanArchitecture.Blazor.Application.Features.Customers.Queries.Export;

public class ExportCustomersQuery : OrderableFilterBase, IRequest<Result<byte[]>>
{
        [CompareTo("Name", "Description")] // <-- This filter will be applied to Name or Description.
        [StringFilterOptions(StringFilterOption.Contains)]
        public string? Keyword { get; set; }
}
    
public class ExportCustomersQueryHandler :
         IRequestHandler<ExportCustomersQuery, Result<byte[]>>
{
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IExcelService _excelService;
        private readonly IStringLocalizer<ExportCustomersQueryHandler> _localizer;
        private readonly CustomerDto _dto = new();
        public ExportCustomersQueryHandler(
            IApplicationDbContext context,
            IMapper mapper,
            IExcelService excelService,
            IStringLocalizer<ExportCustomersQueryHandler> localizer
            )
        {
            _context = context;
            _mapper = mapper;
            _excelService = excelService;
            _localizer = localizer;
        }

        public async Task<Result<byte[]>> Handle(ExportCustomersQuery request, CancellationToken cancellationToken)
        {
            // TODO: Implement ExportCustomersQueryHandler method 
            var data = await _context.Customers.ApplyOrder(request)
                       .ProjectTo<CustomerDto>(_mapper.ConfigurationProvider)
                       .ToListAsync(cancellationToken);
            var result = await _excelService.ExportAsync(data,
                new Dictionary<string, Func<CustomerDto, object?>>()
                {
                    // TODO: Define the fields that should be exported, for example:
                    {_localizer[_dto.GetMemberDescription("Id")],item => item.Id}, 
{_localizer[_dto.GetMemberDescription("Name")],item => item.Name}, 
{_localizer[_dto.GetMemberDescription("Description")],item => item.Description}, 

                }
                , _localizer["Customers"]);
            return await Result<byte[]>.SuccessAsync(result);;
        }
}


public class CustomersExportSpecification : Specification<Customer>
{
    public CustomersExportSpecification(ExportCustomersQuery request)
    {
        Criteria = q => q.Name != null;
        if (!string.IsNullOrEmpty(request.Keyword))
        {
            And(x => x.Name.Contains(request.Keyword));
        }
       
    }
}