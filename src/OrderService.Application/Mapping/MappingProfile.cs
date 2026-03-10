using AutoMapper;
using OrderService.Application.DTOs;
using OrderService.Domain.Entities;
using OrderService.Application.Features.Orders.Commands.CreateOrder;


namespace OrderService.Application.Mapping;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Order, OrderResponseDto>();

        CreateMap<CreateOrderDto, Order>();

        CreateMap<CreateOrderCommand, Order>();
    }
}