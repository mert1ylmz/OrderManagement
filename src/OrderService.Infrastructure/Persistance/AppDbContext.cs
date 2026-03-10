using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using OrderService.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace OrderService.Infrastructure.Persistence;

// DbContext: Uygulama ile Veritabanı arasındaki ana köprüdür.
public class AppDbContext : IdentityDbContext<AppUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    // Hangi sınıfların veritabanında tablo olacağını belirtiyoruz.
    public DbSet<Order> Orders { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Fluent API: Veritabanı kurallarını burada yazarız.
        // Neden burada? Domain katmanını [Key] gibi veritabanı "attribute"ları ile kirletmemek için.
        modelBuilder.Entity<Order>(entity =>
        {
            
            entity.HasKey(e => e.Id);
            entity.Property(e => e.OrderNumber).IsRequired().HasMaxLength(50);
            entity.Property(e => e.CustomerName).IsRequired().HasMaxLength(200);
            entity.Property(e => e.TotalAmount).HasColumnType("decimal(18,2)");
            entity.HasQueryFilter(e => !e.IsDeleted);
        });

        base.OnModelCreating(modelBuilder);
    }
}

