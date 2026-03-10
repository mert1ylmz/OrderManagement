using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.AspNetCore.Mvc;
using ReportingService.API.Models;

[ApiController]
[Route("api/[controller]")]
public class ReportsController : ControllerBase
{
    private readonly string _connectionString;

    public ReportsController(IConfiguration config)
    {
        // ÖNEMLİ: OrderService ile aynı veritabanını kullanıyoruz.
        _connectionString = config.GetConnectionString("DefaultConnection");
    }

    [HttpGet("dashboard-stats")]
    public async Task<IActionResult> GetDashboardStats()
    {
        using var connection = new SqlConnection(_connectionString);

        // Ham SQL sorgusu (Raw SQL) - En yüksek performans!
        var sql = @"SELECT 
                        COUNT(*) as TotalOrders, 
                        ISNULL(SUM(TotalAmount), 0) as TotalRevenue,
                        ISNULL(AVG(TotalAmount), 0) as AverageOrderValue
                    FROM Orders WHERE IsDeleted = 0";

        // Dapper ile tek satırda veriyi çek ve DTO'ya eşle
        var stats = await connection.QuerySingleOrDefaultAsync<OrderReportDto>(sql);

        return Ok(new
        {
            GeneratedAt = DateTime.Now,
            Data = stats
        });
    }
}