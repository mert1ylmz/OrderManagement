namespace OrderService.Application.Interfaces;

public interface IUnitOfWork : IDisposable
{
    //Veritabanına değişiklikleri tek seferde yazar

    Task<int> SaveChangesAsync();
}