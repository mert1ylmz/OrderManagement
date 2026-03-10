using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace OrderService.Infrastructure.Persistence;

public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        // Buradaki bağlantı cümlesi sadece Migration oluşturmak içindir.
        optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=OrderDb;Trusted_Connection=True;");

        return new AppDbContext(optionsBuilder.Options);
    }
}