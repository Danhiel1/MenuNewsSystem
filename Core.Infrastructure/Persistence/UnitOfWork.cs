using System.Collections.Concurrent;
using Core.Application.Interfaces;
using Core.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore.Storage;

namespace Core.Infrastructure.Persistence
{
    /// <summary>
    /// TẠI SAO implement UnitOfWork tại Infrastructure layer?
    /// - IUnitOfWork (interface) ở Application layer → Handler chỉ biết interface
    /// - UnitOfWork (class) ở Infrastructure → phụ thuộc EF Core, DbContext
    /// - Đúng Clean Architecture: Application không biết EF Core tồn tại
    /// 
    /// TẠI SAO dùng ConcurrentDictionary cache repositories?
    /// - Mỗi lần gọi Repository&lt;Menu&gt;() → không tạo mới, lấy từ cache
    /// - Tiết kiệm memory, đảm bảo cùng 1 DbContext cho tất cả repos
    /// - ConcurrentDictionary → thread-safe (nhiều request đồng thời)
    /// 
    /// TẠI SAO dùng IDbContextTransaction?
    /// - EF Core wrap database transaction (SQL: BEGIN TRAN / COMMIT / ROLLBACK)
    /// - BeginTransaction → tất cả SaveChanges chỉ pending
    /// - Commit → ghi thật vào DB
    /// - Rollback → hủy tất cả → DB y nguyên như chưa có gì xảy ra
    /// </summary>
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        private readonly ConcurrentDictionary<Type, object> _repositories = new();
        private IDbContextTransaction? _transaction;
        private bool _disposed;

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
        }

        public IGenericRepository<T> Repository<T>() where T : class
        {
            // GetOrAdd: nếu chưa có → tạo mới, nếu có rồi → trả về cái cũ
            return (IGenericRepository<T>)_repositories.GetOrAdd(
                typeof(T),
                _ => new GenericRepository<T>(_context)
            );
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
        {
            // Tạo transaction mới → từ đây SaveChanges chỉ pending, chưa commit thật
            _transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
        }

        public async Task CommitAsync(CancellationToken cancellationToken = default)
        {
            if (_transaction != null)
            {
                await _transaction.CommitAsync(cancellationToken);
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public async Task RollbackAsync(CancellationToken cancellationToken = default)
        {
            if (_transaction != null)
            {
                // HỦY tất cả thay đổi → DB trở lại trạng thái trước BeginTransaction
                await _transaction.RollbackAsync(cancellationToken);
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                _transaction?.Dispose();
                _context.Dispose();
                _disposed = true;
            }
        }
    }
}
