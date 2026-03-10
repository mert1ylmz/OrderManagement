using Microsoft.EntityFrameworkCore;
using OrderService.Application.Interfaces;
using OrderService.Domain.Entities;
using OrderService.Infrastructure.Persistence;

namespace OrderService.Infrastructure.Repositories;

public class OrderRepository : IOrderRepository
{
    private readonly AppDbContext _context;

    public OrderRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Order>> GetAllAsync()
    {
        return await _context.Orders.ToListAsync();
    }

    public async Task<Order?> GetByIdAsync(Guid id)
    {
        return await _context.Orders.FindAsync(id);
    }

    public async Task AddAsync(Order order)
    {
        await _context.Orders.AddAsync(order);
        //await _context.SaveChangesAsync();
    }

    public async Task<(List<Order> items, int totalCount)> GetPagedOrdersAsync(int pageNumber, int pageSize)
    {
        var query = _context.Orders.Where(x => !x.IsDeleted);

        int totalCount = await query.CountAsync();

        var items = await query
            .Skip((pageNumber - 1) * pageSize) // Önceki sayfaları atla
            .Take(pageSize)                   // Sayfa boyutu kadar al
            .ToListAsync();

        return (items, totalCount);
    }
}