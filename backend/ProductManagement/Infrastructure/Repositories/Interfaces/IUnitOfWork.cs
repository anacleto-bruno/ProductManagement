using ProductManagement.Entities;

namespace ProductManagement.Infrastructure.Repositories.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IProductRepository Products { get; }
    IRepository<Color> Colors { get; }
    IRepository<Size> Sizes { get; }
    IRepository<ProductColor> ProductColors { get; }
    IRepository<ProductSize> ProductSizes { get; }

    Task<int> SaveChangesAsync();
    Task BeginTransactionAsync();
    Task CommitTransactionAsync();
    Task RollbackTransactionAsync();
}