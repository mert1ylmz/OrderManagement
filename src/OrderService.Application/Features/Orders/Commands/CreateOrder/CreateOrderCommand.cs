using MediatR;
using OrderService.Application.DTOs;

namespace OrderService.Application.Features.Orders.Commands.CreateOrder;

public class CreateOrderCommand : IRequest<OrderResponseDto>
{
    public string OrderNumber { get; set; }
    public string CustomerName { get; set; }
    public decimal TotalAmount { get; set; }
}