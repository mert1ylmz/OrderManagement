using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrderService.Application.Features.Orders.Commands.CreateOrder;
using OrderService.Application.Features.Orders.Queries.GetAllOrders;
using OrderService.Application.Interfaces; // ITokenService için

namespace OrderService.API.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class OrdersController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ITokenService _tokenService; // Token servisini buraya ekledik

    public OrdersController(IMediator mediator, ITokenService tokenService)
    {
        _mediator = mediator;
        _tokenService = tokenService;
    }
    //"token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1bmlxdWVfbmFtZSI6ImFkbWluIiwicm9sZSI6IkFkbWluIiwibmJmIjoxNzcyNjU5Mjg3LCJleHAiOjE3NzI2NjI4ODcsImlhdCI6MTc3MjY1OTI4NywiaXNzIjoiT3JkZXJTZXJ2aWNlQVBJIiwiYXVkIjoiT3JkZXJTZXJ2aWNlQ2xpZW50In0.4M0vELr2l9Mcfg2RXmJyPoq_xduKjVmAu90pQTc81UA"

    [HttpGet("login-test")]
    [AllowAnonymous] // Giriş yapmadan erişebilmek için
    public IActionResult LoginTest(string user = "admin")
    {
        // Token üret
        var token = _tokenService.GenerateToken(user, "Admin");
        return Ok(new { Token = token });
    }

    [HttpGet]
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] GetAllOrdersQuery query)
    {
        // [FromQuery] eklediğimizde .NET şunu anlar: 
        // "Bu sınıfın içindeki PageNumber ve PageSize'ı URL'den (Query String) oku."

        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateOrderCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }
}