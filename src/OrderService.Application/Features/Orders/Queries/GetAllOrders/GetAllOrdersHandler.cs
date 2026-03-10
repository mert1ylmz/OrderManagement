using AutoMapper;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using OrderService.Application.DTOs;
using OrderService.Application.Features.Orders.Queries.GetAllOrders;
using OrderService.Application.Interfaces;
using System.Text.Json;

public class GetAllOrdersHandler : IRequestHandler<GetAllOrdersQuery, PagedResponseDto<OrderResponseDto>>
{
    private readonly IOrderRepository _repository;
    private readonly IMapper _mapper;
    private readonly IDistributedCache _cache; // Redis için eklendi

    public GetAllOrdersHandler(IOrderRepository repository, IMapper mapper, IDistributedCache cache)
    {
        _repository = repository;
        _mapper = mapper;
        _cache = cache;
    }

    public async Task<PagedResponseDto<OrderResponseDto>> Handle(GetAllOrdersQuery request, CancellationToken cancellationToken)
    {
        // 1. Cache anahtarı oluştur (Sayfa numarasına göre eşsiz olmalı)
        string cacheKey = $"orders_p_{request.PageNumber}_s_{request.PageSize}";

        // 2. Redis'ten veriyi oku
        var cachedData = await _cache.GetStringAsync(cacheKey);

        if (!string.IsNullOrEmpty(cachedData))
        {
            // Cache'te varsa: JSON'u objeye geri çevir ve dön
            return JsonSerializer.Deserialize<PagedResponseDto<OrderResponseDto>>(cachedData);
        }

        // 3. Cache'te yoksa: Veritabanından çek
        var (orders, totalCount) = await _repository.GetPagedOrdersAsync(request.PageNumber, request.PageSize);
        var dtos = _mapper.Map<List<OrderResponseDto>>(orders);
        var response = new PagedResponseDto<OrderResponseDto>(dtos, totalCount, request.PageNumber, request.PageSize);

        // 4. Redis'e kaydet (10 dakika boyunca sakla)
        var cacheOptions = new DistributedCacheEntryOptions()
            .SetAbsoluteExpiration(TimeSpan.FromMinutes(10));

        var serializedData = JsonSerializer.Serialize(response);
        await _cache.SetStringAsync(cacheKey, serializedData, cacheOptions);

        return response;
    }
}