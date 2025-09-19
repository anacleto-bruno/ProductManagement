using Microsoft.EntityFrameworkCore.Storage;
using ProductManagement.Entities;
using ProductManagement.Infrastructure.Data;
using ProductManagement.Infrastructure.Repositories.Interfaces;

namespace ProductManagement.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly ProductManagementContext _context;
    private IDbContextTransaction? _transaction;

    private IProductRepository? _products;
    private IRepository<Color>? _colors;
    private IRepository<Size>? _sizes;
    private IRepository<ProductColor>? _productColors;
    private IRepository<ProductSize>? _productSizes;

    public UnitOfWork(ProductManagementContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public IProductRepository Products => _products ??= new ProductRepository(_context);
    public IRepository<Color> Colors => _colors ??= new Repository<Color>(_context);
    public IRepository<Size> Sizes => _sizes ??= new Repository<Size>(_context);
    public IRepository<ProductColor> ProductColors => _productColors ??= new Repository<ProductColor>(_context);
    public IRepository<ProductSize> ProductSizes => _productSizes ??= new Repository<ProductSize>(_context);

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }

    public async Task BeginTransactionAsync()
    {
        _transaction = await _context.Database.BeginTransactionAsync();
    }

    public async Task CommitTransactionAsync()
    {
        if (_transaction != null)
        {
            await _transaction.CommitAsync();
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public async Task RollbackTransactionAsync()
    {
        if (_transaction != null)
        {
            await _transaction.RollbackAsync();
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public void Dispose()
    {
        _transaction?.Dispose();
        _context.Dispose();
    }
}