using MediatR;
using OrderService.Application.DTOs;

namespace OrderService.Application.Features.Orders.Queries.GetAllOrders;

// BURAYA DİKKAT: IRequest içinde PagedResponseDto<OrderResponseDto> yazdığından emin ol
public class GetAllOrdersQuery : IRequest<PagedResponseDto<OrderResponseDto>>
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}