using AutoMapper; // 1. Bu using satırı mutlaka olmalı
using MediatR;
using Hangfire; // 2. Hangfire için gerekli
using OrderService.Application.DTOs;
using OrderService.Application.Interfaces;
using OrderService.Domain.Entities;
using Microsoft.Extensions.Caching.Distributed;

namespace OrderService.Application.Features.Orders.Commands.CreateOrder;

public class CreateOrderHandler : IRequestHandler<CreateOrderCommand, OrderResponseDto>
{
    private readonly IOrderRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper; // 3. Tanımlama burada olmalı
    private readonly IEmailService _emailService;
    private readonly IBackgroundJobClient _backgroundJobClient;
    private readonly IDistributedCache _cache;

    // 4. Constructor (Yapıcı Metot) içinde tüm bu servisler şart
    public CreateOrderHandler(
        IOrderRepository repository,
        IUnitOfWork unitOfWork,
        IMapper mapper, // <-- Eksik olan buydu
        IEmailService emailService,
        IBackgroundJobClient backgroundJobClient,
        IDistributedCache cache)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _mapper = mapper; // 5. Atama burada yapılmalı
        _emailService = emailService;
        _backgroundJobClient = backgroundJobClient;
        _cache = cache;
    }

    public async Task<OrderResponseDto> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        
        var order = _mapper.Map<Order>(request);

        order.Id = Guid.NewGuid();
        order.CreatedDate = DateTime.Now;

        await _repository.AddAsync(order);
        await _unitOfWork.SaveChangesAsync();

        // Cache temizleme (Önceki adımda yapmıştık)
        await _cache.RemoveAsync("orders_p_1_s_10");

        // ARKA PLAN İŞİ (Hangfire)

        Console.WriteLine("İş kuyruğa alınıyor...");
        _backgroundJobClient.Enqueue<IEmailService>(x =>
            x.SendOrderConfirmationEmail(order.CustomerName, order.OrderNumber));

        return _mapper.Map<OrderResponseDto>(order);
    }
}