namespace OrderService.Application.DTOs;

public class OrderResponseDto
{
    public Guid Id { get; set; }
    public string OrderNumber { get; set; }
    public string CustomerName { get; set; }
    public decimal TotalAmount { get; set; }
    public string Status { get; set; }
}

public class CreateOrderDto
{
    public string OrderNumber { get; set; }
    public string CustomerName { get; set; }
    public decimal TotalAmount { get; set; }
}